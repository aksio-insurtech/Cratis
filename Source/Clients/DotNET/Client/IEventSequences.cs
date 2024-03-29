// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.EventSequences;

namespace Aksio.Cratis.Client;

/// <summary>
/// Defines the event sequences.
/// </summary>
public interface IEventSequences
{
    /// <summary>
    /// Gets the <see cref="IEventLog"/> event sequence.
    /// </summary>
    IEventLog EventLog { get; }

    /// <summary>
    /// Gets the <see cref="IEventOutbox"/> event sequence.
    /// </summary>
    IEventOutbox Outbox { get; }

    /// <summary>
    /// Gets the <see cref="IEventSequence"/> for a specific event sequence.
    /// </summary>
    /// <param name="eventSequenceId"><see cref="EventSequenceId"/> to get for.</param>
    /// <returns><see cref="IEventSequence"/> instance.</returns>
    IEventSequence GetEventSequence(EventSequenceId eventSequenceId);
}
