// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Events.Projections.Definitions;
using Cratis.Events.Projections.Pipelines;
using Cratis.Properties;
using Microsoft.Extensions.Logging;

namespace Cratis.Events.Projections.for_Projections.given
{
    public class no_projections : all_dependencies
    {
        protected ProjectionId projection_identifier = "c9b07c5d-d092-41b0-b9ee-1b0134b10a65";
        protected Projections projections;

        protected ProjectionDefinition projection_definition;
        protected ProjectionPipelineDefinition pipeline_definition;

        protected Mock<IProjection> projection;
        protected Mock<IProjectionPipeline> pipeline;

        void Establish()
        {
            projections = new(
            projection_definitions.Object,
            pipeline_definitions.Object,
            projection_factory.Object,
            pipeline_factory.Object,
            Mock.Of<ILogger<Projections>>());

            projection = new();
            projection.SetupGet(_ => _.Identifier).Returns(projection_identifier);
            pipeline = new();
            pipeline.SetupGet(_ => _.Projection).Returns(projection.Object);

            projection_definition = new(
                projection_identifier,
                "My Projection",
                new ModelDefinition("Some Model", "{}"),
                new Dictionary<EventType, FromDefinition>(),
                new Dictionary<PropertyPath, ChildrenDefinition>(),
                null);

            pipeline_definition = new(
                projection_identifier,
                "dc5366eb-d453-43c9-859f-64989a858e7c",
                Array.Empty<ProjectionResultStoreDefinition>());

            projection_factory.Setup(_ => _.CreateFrom(projection_definition)).Returns(projection.Object);
            pipeline_factory.Setup(_ => _.CreateFrom(projection.Object, pipeline_definition)).Returns(pipeline.Object);
        }
    }
}