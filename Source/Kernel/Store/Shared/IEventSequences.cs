// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;

namespace Aksio.Cratis.Events.Store;

/// <summary>
/// Defines the system for working with event sequences.
/// </summary>
public interface IEventSequences
{
    /// <summary>
    /// Append a single event to the event store.
    /// </summary>
    /// <param name="eventSequenceId">The <see cref="EventSequenceId"/> representing the event sequence to append to.</param>
    /// <param name="sequenceNumber">The unique <see cref="EventSequenceNumber">sequence number</see> within the event sequence.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/> to append for.</param>
    /// <param name="eventType">The <see cref="EventType">type of event</see> to append.</param>
    /// <param name="content">The JSON payload of the event.</param>
    /// <returns>Awaitable <see cref="Task"/>.</returns>
    Task Append(EventSequenceId eventSequenceId, EventSequenceNumber sequenceNumber, EventSourceId eventSourceId, EventType eventType, JsonObject content);

    /// <summary>
    /// Compensate a single event to the event store.
    /// </summary>
    /// <param name="eventSequenceId">The <see cref="EventSequenceId"/> representing the event sequence to append to.</param>
    /// <param name="sequenceNumber">The unique <see cref="EventSequenceNumber">sequence number</see> within the event sequence.</param>
    /// <param name="eventType">The <see cref="EventType">type of event</see> to append.</param>
    /// <param name="content">The JSON payload of the event.</param>
    /// <returns>Awaitable <see cref="Task"/>.</returns>
    Task Compensate(EventSequenceId eventSequenceId, EventSequenceNumber sequenceNumber, EventType eventType, JsonObject content);

    /// <summary>
    /// Find <see cref="AppendedEvent">appended events</see> from the event sequence.
    /// </summary>
    /// <param name="eventSequenceId">The <see cref="EventSequenceId"/> representing the event sequence to find from.</param>
    /// <param name="eventSourceId"><see cref="EventSourceId"/> to find for.</param>
    /// <returns>Awaitable <see cref="Task"/> with <see cref="IEventStoreFindResult"/>.</returns>
    Task<IEventStoreFindResult> FindFor(EventSequenceId eventSequenceId, EventSourceId eventSourceId);
}
