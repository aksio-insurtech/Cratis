// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Projections.Definitions;

namespace Aksio.Cratis.Projections.Json;

/// <summary>
/// Defines a parser for JSON definition of a projection pipeline.
/// </summary>
public interface IJsonProjectionPipelineSerializer
{
    /// <summary>
    /// Deserialize a JSON string definition into <see cref="ProjectionPipelineDefinition"/>.
    /// </summary>
    /// <param name="json">JSON to parse.</param>
    /// <returns><see cref="ProjectionPipelineDefinition"/> instance.</returns>
    ProjectionPipelineDefinition Deserialize(string json);

    /// <summary>
    /// Serialize a <see cref="ProjectionPipelineDefinition"/>.
    /// </summary>
    /// <param name="definition"><see cref="ProjectionPipelineDefinition"/> to serialize.</param>
    /// <returns>JSON representation.</returns>
    string Serialize(ProjectionPipelineDefinition definition);
}
