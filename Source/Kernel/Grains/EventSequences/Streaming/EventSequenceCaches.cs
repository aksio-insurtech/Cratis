// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.Configuration;

namespace Aksio.Cratis.Kernel.Grains.EventSequences.Streaming;

/// <summary>
/// Represents an implementation of <see cref="IEventSequenceCaches"/>.
/// </summary>
[Singleton]
public class EventSequenceCaches : IEventSequenceCaches
{
    readonly ConcurrentDictionary<(MicroserviceId, TenantId, EventSequenceId), IEventSequenceCache> _caches = new();
    readonly IEventSequenceCacheFactory _eventSequenceCacheFactory;
    readonly KernelConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequenceCaches"/> class.
    /// </summary>
    /// <param name="eventSequenceCacheFactory"><see cref="IEventSequenceCacheFactory"/> for creating <see cref="IEventSequenceCache"/> instances.</param>
    /// <param name="configuration">The <see cref="KernelConfiguration"/>.</param>
    public EventSequenceCaches(
        IEventSequenceCacheFactory eventSequenceCacheFactory,
        KernelConfiguration configuration)
    {
        _eventSequenceCacheFactory = eventSequenceCacheFactory;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public IEventSequenceCache GetFor(MicroserviceId microserviceId, TenantId tenantId, EventSequenceId eventSequenceId)
    {
        var key = (microserviceId, tenantId, eventSequenceId);
        if (!_caches.TryGetValue(key, out var cache))
        {
            cache = _eventSequenceCacheFactory.Create(microserviceId, tenantId, eventSequenceId);
            _caches.TryAdd(key, cache);
        }

        return cache;
    }

    /// <inheritdoc/>
    public bool IsUnderPressure() => _caches.Values.Any(_ => _.IsUnderPressure());

    /// <inheritdoc/>
    public async Task PrimeAll()
    {
        foreach (var (microserviceId, microservice) in _configuration.Microservices)
        {
            foreach (var (tenantId, _) in _configuration.Tenants)
            {
                if (!_configuration.Storage.Microservices.ContainsKey(microserviceId) ||
                    !_configuration.Storage.Microservices.Get(microserviceId).Tenants.ContainsKey(tenantId))
                {
                    continue;
                }

                await GetFor(microserviceId, tenantId, EventSequenceId.Log).PrimeWithTailWindow();
            }
        }
    }

    /// <inheritdoc/>
    public void Purge()
    {
        foreach (var cache in _caches.Values.Where(_ => _.IsUnderPressure()))
        {
            cache.Purge();
        }
    }
}
