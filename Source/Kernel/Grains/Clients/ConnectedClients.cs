// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Aksio.Cratis.Connections;
using Aksio.Cratis.Kernel.Orleans.Observers;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Grains.Clients;

/// <summary>
/// Represents an implementation of <see cref="IConnectedClients"/>.
/// </summary>
public class ConnectedClients : Grain, IConnectedClients
{
    /// <summary>
    /// Gets the name of the HTTP client for connected clients.
    /// </summary>
    public const string ConnectedClientsHttpClient = "connected-clients";

    readonly IList<ConnectedClient> _clients = new List<ConnectedClient>();
    readonly ILogger<ConnectedClients> _logger;
    readonly IConnectedClientsMetricsFactory _metricsFactory;
    readonly ObserverManager<INotifyClientDisconnected> _clientDisconnectedObservers;
    IConnectedClientsMetrics? _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectedClients"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    /// <param name="metricsFactory"><see cref="IConnectedClientsMetricsFactory"/> for creating metrics.</param>
    public ConnectedClients(
        ILogger<ConnectedClients> logger,
        IConnectedClientsMetricsFactory metricsFactory)
    {
        _logger = logger;
        _metricsFactory = metricsFactory;
        _clientDisconnectedObservers = new(TimeSpan.FromMinutes(1), logger, "ClientDisconnectedObservers");
    }

    /// <inheritdoc/>
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _metrics = _metricsFactory.Create(this.GetPrimaryKey());
        RegisterTimer(ReviseConnectedClients, null!, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnClientConnected(
        ConnectionId connectionId,
        Uri clientUri,
        string version,
        bool isRunningWithDebugger,
        bool isMultiTenanted)
    {
        var microserviceId = (MicroserviceId)this.GetPrimaryKey();

        _logger.ClientConnected(microserviceId, connectionId);
        _clients.Where(_ => _.ClientUri == clientUri).ToList().ForEach(_ => _clients.Remove(_));
        _clients.Add(new ConnectedClient(
            connectionId,
            clientUri,
            version,
            DateTimeOffset.UtcNow,
            isRunningWithDebugger,
            isMultiTenanted));
        _metrics?.SetConnectedClients(_clients.Count);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnClientDisconnected(ConnectionId connectionId, string reason)
    {
        var microserviceId = (MicroserviceId)this.GetPrimaryKey();
        _logger.ClientDisconnected(microserviceId, connectionId, reason);

        var client = _clients.FirstOrDefault(_ => _.ConnectionId == connectionId);
        if (client is not null)
        {
            _clients.Remove(client);
            _clientDisconnectedObservers.Notify(_ => _.OnClientDisconnected(client));
        }
        _metrics?.SetConnectedClients(_clients.Count);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> OnClientPing(ConnectionId connectionId)
    {
        var client = _clients.FirstOrDefault(_ => _.ConnectionId == connectionId);
        if (client is not null)
        {
            _clients.Where(_ => _.ClientUri == client.ClientUri).ToList().ForEach(_ => _clients.Remove(_));
            _clients.Add(client with { LastSeen = DateTimeOffset.UtcNow });
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<ConnectedClient>> GetAllConnectedClients() => Task.FromResult(_clients.AsEnumerable());

    /// <inheritdoc/>
    public Task SubscribeDisconnected(INotifyClientDisconnected subscriber)
    {
        _clientDisconnectedObservers.Subscribe(subscriber, subscriber);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UnsubscribeDisconnected(INotifyClientDisconnected subscriber)
    {
        _clientDisconnectedObservers.Unsubscribe(subscriber);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsConnected(ConnectionId connectionId) => Task.FromResult(_clients.Any(_ => _.ConnectionId == connectionId));

    /// <inheritdoc/>
    public Task<ConnectedClient> GetConnectedClient(ConnectionId connectionId) => Task.FromResult(_clients.First(_ => _.ConnectionId == connectionId));

    async Task ReviseConnectedClients(object state)
    {
        if (Debugger.IsAttached) return;

        foreach (var connectedClient in _clients.ToArray())
        {
            if (connectedClient.IsRunningWithDebugger) continue;

            if (connectedClient.LastSeen < DateTimeOffset.UtcNow.AddSeconds(-5))
            {
                await OnClientDisconnected(connectedClient.ConnectionId, "Last seen was more than 5 seconds ago");
            }
        }
    }
}
