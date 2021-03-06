// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Orleans;

namespace Aksio.Cratis.Events.Projections.Grains;

/// <summary>
/// Defines a projection.
/// </summary>
public interface IProjection : IGrainWithGuidCompoundKey
{
    /// <summary>
    /// Get a model instance by its <see cref="EventSourceId">identifier</see>.
    /// </summary>
    /// <param name="eventSourceId"><see cref="EventSourceId"/> to get for.</param>
    /// <returns>A projected model in the form of a <see cref="JsonObject"/>.</returns>
    Task<JsonObject> GetModelInstanceById(EventSourceId eventSourceId);

    /// <summary>
    /// Ensure the projection exists and is started.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Ensure();

    /// <summary>
    /// Rewind projection.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Rewind();
}
