// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.EventSequences.Caching;

/// <summary>
/// Represents an implementation of <see cref="IEventSequenceCache"/>.
/// </summary>
public class EventSequenceCache : IEventSequenceCache
{
    readonly SortedList<EventSequenceNumber, AppendedEvent> _events;
    readonly IEventSequenceStorageProvider _eventSequenceStorageProvider;

    /// <inheritdoc/>
    public EventSequenceId EventSequenceId { get; }

    /// <inheritdoc/>
    public int RangeSize { get; }

    /// <inheritdoc/>
    public EventSequenceCacheRange CurrentRange { get; private set; }

    /// <inheritdoc/>
    public IEnumerable<AppendedEvent> Content
    {
        get
        {
            lock (_events)
            {
                return _events.Values;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequenceCache"/> class.
    /// </summary>
    /// <param name="eventSequenceId"><see cref="EventSequenceId"/> the cache is for.</param>
    /// <param name="rangeSize">Size of the range to keep cached.</param>
    /// <param name="eventSequenceStorageProvider"><see cref="IEventSequenceStorageProvider"/> for working with the event sequences.</param>
    public EventSequenceCache(
        EventSequenceId eventSequenceId,
        int rangeSize,
        IEventSequenceStorageProvider eventSequenceStorageProvider)
    {
        EventSequenceId = eventSequenceId;
        RangeSize = rangeSize;
        _events = new(new EventSequenceNumberComparer());
        _eventSequenceStorageProvider = eventSequenceStorageProvider;
        CurrentRange = new(0, 0);

        Populate(eventSequenceId, rangeSize);
    }

    /// <inheritdoc/>
    public void Feed(IEnumerable<AppendedEvent> events)
    {
        lock (_events)
        {
            events = events.Take(RangeSize + 1);

            var firstSequenceNumber = events.First().Metadata.SequenceNumber;
            var lastSequenceNumber = events.Last().Metadata.SequenceNumber;
            var evictToStartOf = EventSequenceNumber.Unavailable;
            var evictFromEndOf = EventSequenceNumber.Unavailable;

            if (lastSequenceNumber > CurrentRange.End)
            {
                if (firstSequenceNumber < CurrentRange.End)
                {
                    firstSequenceNumber = CurrentRange.End;
                }

                var total = lastSequenceNumber - CurrentRange.Start;
                if (total >= (ulong)RangeSize)
                {
                    evictToStartOf = lastSequenceNumber - RangeSize;
                }
            }

            if (firstSequenceNumber < CurrentRange.Start)
            {
                if (lastSequenceNumber > CurrentRange.Start)
                {
                    lastSequenceNumber = CurrentRange.Start - 1;
                }

                var total = CurrentRange.End - firstSequenceNumber;
                if (total > (ulong)RangeSize)
                {
                    evictFromEndOf = firstSequenceNumber + RangeSize;
                }
            }

            events = events.Where(_ => _.Metadata.SequenceNumber >= firstSequenceNumber && _.Metadata.SequenceNumber <= lastSequenceNumber);

            foreach (var @event in events)
            {
                if (!_events.ContainsKey(@event.Metadata.SequenceNumber))
                {
                    _events.Add(@event.Metadata.SequenceNumber, @event);
                }
            }

            if (evictToStartOf != EventSequenceNumber.Unavailable)
            {
                foreach (var sequenceNumber in _events.Where(_ => _.Key <= evictToStartOf).Select(_ => _.Key).ToArray())
                {
                    _events.Remove(sequenceNumber);
                }
            }

            if (evictFromEndOf != EventSequenceNumber.Unavailable)
            {
                foreach (var sequenceNumber in _events.Where(_ => _.Key >= evictFromEndOf).Select(_ => _.Key).ToArray())
                {
                    _events.Remove(sequenceNumber);
                }
            }

            CurrentRange = new(_events.First().Key, _events.Last().Key);
        }
    }

    /// <inheritdoc/>
    public IEventCursor GetFrom(EventSequenceNumber sequenceNumber) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEventCursor GetRange(EventSequenceNumber start, EventSequenceNumber end) => throw new NotImplementedException();

    void Populate(EventSequenceId eventSequenceId, int rangeSize)
    {
        Task.Run(async () =>
        {
            var cursor = await _eventSequenceStorageProvider.GetRange(eventSequenceId, 0, (ulong)rangeSize - 1);
            while (await cursor.MoveNext())
            {
                Feed(cursor.Current);
            }
        }).Wait();
    }
}
