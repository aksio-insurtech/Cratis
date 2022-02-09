// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Aksio.Cratis.Compliance;
using Aksio.Cratis.Events.Schemas;

namespace Aksio.Cratis.Events.Store.MongoDB
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventConverter"/>.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        readonly ISchemaStore _schemaStore;
        readonly IJsonComplianceManager _jsonComplianceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverter"/> class.
        /// </summary>
        /// <param name="schemaStore"><see cref="ISchemaStore"/> for event schemas.</param>
        /// <param name="jsonComplianceManager"><see cref="IJsonComplianceManager"/> for handling compliance on events.</param>
        public EventConverter(
            ISchemaStore schemaStore,
            IJsonComplianceManager jsonComplianceManager)
        {
            _schemaStore = schemaStore;
            _jsonComplianceManager = jsonComplianceManager;
        }

        /// <inheritdoc/>
        public async Task<AppendedEvent> ToAppendedEvent(Event @event)
        {
            var eventType = new EventType(@event.Type, EventGeneration.First);
            var content = (JsonNode.Parse(@event.Content[EventGeneration.First.ToString()].ToString()) as JsonObject)!;
            var eventSchema = await _schemaStore.GetFor(eventType.Id, eventType.Generation);
            var releasedContent = await _jsonComplianceManager.Release(eventSchema.Schema, @event.EventSourceId, content);

            return new AppendedEvent(
                new EventMetadata(@event.SequenceNumber, eventType),
                new EventContext(@event.EventSourceId, @event.Occurred),
                releasedContent);
        }
    }
}