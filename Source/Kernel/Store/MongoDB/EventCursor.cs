// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Aksio.Cratis.Compliance;
using Aksio.Cratis.Events.Schemas;
using MongoDB.Driver;

namespace Aksio.Cratis.Events.Store.MongoDB
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventCursor"/> for handling events from event log.
    /// </summary>
    public class EventCursor : IEventCursor
    {
        readonly ISchemaStore _schemaStore;
        readonly IJsonComplianceManager _jsonComplianceManager;
        readonly IAsyncCursor<Event>? _innerCursor;

        /// <inheritdoc/>
        public IEnumerable<AppendedEvent> Current { get; private set; } = Array.Empty<AppendedEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCursor"/> class.
        /// </summary>
        /// <param name="schemaStore"><see cref="ISchemaStore"/> for event schemas.</param>
        /// <param name="jsonComplianceManager"><see cref="IJsonComplianceManager"/> for handling compliance on events.</param>
        /// <param name="innerCursor">The underlying MongoDB cursor.</param>
        public EventCursor(
            ISchemaStore schemaStore,
            IJsonComplianceManager jsonComplianceManager,
            IAsyncCursor<Event>? innerCursor)
        {
            _schemaStore = schemaStore;
            _jsonComplianceManager = jsonComplianceManager;
            _innerCursor = innerCursor;
        }

        /// <inheritdoc/>
        public async Task<bool> MoveNext()
        {
            if (_innerCursor is null) return false;
            var result = await _innerCursor.MoveNextAsync();
            if (_innerCursor.Current is not null)
            {
                Current = await Task.WhenAll(_innerCursor.Current.Select(@event => ToAppendedEvent(@event)));
            }
            else
            {
                Current = Array.Empty<AppendedEvent>();
            }
            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _innerCursor?.Dispose();
        }

        async Task<AppendedEvent> ToAppendedEvent(Event @event)
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