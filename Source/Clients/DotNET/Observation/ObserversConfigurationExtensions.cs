// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Aksio.Cratis.Observation;

/// <summary>
/// Extension methods for configuring observers.
/// </summary>
public static class ObserversConfigurationExtensions
{
    /// <summary>
    /// Add observers to the service collection.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add to.</param>
    /// <param name="clientArtifacts">Optional <see cref="IClientArtifactsProvider"/> for the client artifacts.</param>
    /// <returns>Continuation.</returns>
    public static IServiceCollection AddObservers(this IServiceCollection services, IClientArtifactsProvider clientArtifacts)
    {
        clientArtifacts.Observers.ForEach(_ => services.AddTransient(_));
        return services;
    }
}
