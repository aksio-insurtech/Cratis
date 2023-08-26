// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Client;

/// <summary>
/// Defines the client for Cratis.
/// </summary>
public interface IClient
{
    /// <summary>
    /// Connect to Cratis kernel.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Connect();

    /// <summary>
    /// Gets the <see cref="IEventSequences"/>.
    /// </summary>
    /// <param name="tenantId">Optional <see cref="TenantId"/>.</param>
    /// <returns></returns>
    /// <remarks>
    /// If the client is configured as a single tenant client, it will return the default one, even if <paramref name="tenantId"/> is specified.
    /// If the client is configured as a multi tenant client, it will return the one for the specified <paramref name="tenantId"/>.
    /// </remarks>
    IEventSequences GetEventSequences(TenantId? tenantId = default);
}
