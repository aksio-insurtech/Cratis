// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Orleans;
using Orleans.Providers;

namespace Aksio.Cratis.Configuration.Grains.Tenants;

/// <summary>
/// Represents an implementation of <see cref="ITenantConfiguration"/>.
/// </summary>
[StorageProvider(ProviderName = TenantConfigurationState.StorageProvider)]
public class TenantConfiguration : Grain<TenantConfigurationState>, ITenantConfiguration
{
    /// <inheritdoc/>
    public async Task Set(string key, string value)
    {
        State[key] = value;
        await WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task Set(IDictionary<string, string> collection)
    {
        foreach (var (key, value) in collection)
        {
            State[key] = value;
        }
        await WriteStateAsync();
    }

    /// <inheritdoc/>
    public Task<TenantConfigurationState> All() => Task.FromResult(State);
}
