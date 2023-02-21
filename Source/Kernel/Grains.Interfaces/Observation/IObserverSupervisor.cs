// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.Kernel.Observation;
using Aksio.Cratis.Observation;
using Orleans;

namespace Aksio.Cratis.Kernel.Grains.Observation;

/// <summary>
/// Defines an observer of an event sequence.
/// </summary>
public interface IObserverSupervisor : IGrainWithGuidCompoundKey
{
    /// <summary>
    /// Set metadata associated with the observer.
    /// </summary>
    /// <param name="name">Friendly name of the observer.</param>
    /// <param name="type"><see cref="ObserverType"/>.</param>
    /// <returns>Awaitable task.</returns>
    Task SetNameAndType(ObserverName name, ObserverType type);

    /// <summary>
    /// Subscribe to observer.
    /// </summary>
    /// <typeparam name="TObserverSubscriber">Type of <see cref="IObserverSubscriber"/> to subscribe.</typeparam>
    /// <param name="eventTypes">Collection of <see cref="EventType">event types</see> to subscribe to.</param>
    /// <param name="subscriberArgs">Optional arguments associated with the subscription.</param>
    /// <returns>Awaitable task.</returns>
    Task Subscribe<TObserverSubscriber>(IEnumerable<EventType> eventTypes, object? subscriberArgs = default)
        where TObserverSubscriber : IObserverSubscriber;

    /// <summary>
    /// Get the current subscription.
    /// </summary>
    /// <returns><see cref="ObserverSubscription"/>.</returns>
    Task<ObserverSubscription> GetCurrentSubscription();

    /// <summary>
    /// Unsubscribe from the observer.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Unsubscribe();

    /// <summary>
    /// Rewind the observer.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Rewind();

    /// <summary>
    /// Try to resume the partition.
    /// </summary>
    /// <param name="partition">The partition to try to resume.</param>
    /// <returns>Awaitable task.</returns>
    Task TryResumePartition(EventSourceId partition);

    /// <summary>
    /// Notify that catch-up is complete.
    /// </summary>
    /// <param name="failedPartitions">Collection of any <see cref="FailedPartition">failed partitions</see>.</param>
    /// <returns>Awaitable task.</returns>
    Task NotifyCatchUpComplete(IEnumerable<FailedPartition> failedPartitions);

    /// <summary>
    /// Notify that replay is complete.
    /// </summary>
    /// <param name="failedPartitions">Collection of any <see cref="FailedPartition">failed partitions</see>.</param>
    /// <returns>Awaitable task.</returns>
    Task NotifyReplayComplete(IEnumerable<FailedPartition> failedPartitions);

    /// <summary>
    /// Notify that failed partition has run to completion.
    /// </summary>
    /// <param name="partition">Partition that has recovered.</param>
    /// <param name="lastProcessedEvent">The EventSequenceNumber of the last event that the worked processed when declaring itself complete.</param>
    /// <returns>Awaitable task.</returns>
    Task NotifyFailedPartitionRecoveryComplete(EventSourceId partition, EventSequenceNumber lastProcessedEvent);

    /// <summary>
    /// Notify that the partition has failed.
    /// </summary>
    /// <param name="partition">The partition that failed.</param>
    /// <param name="sequenceNumber">The sequence number of the failure.</param>
    /// <param name="exceptionMessages">All exception messages.</param>
    /// <param name="exceptionStackTrace">The exception stacktrace.</param>
    /// <returns>Awaitable task.</returns>
    Task PartitionFailed(EventSourceId partition, EventSequenceNumber sequenceNumber, IEnumerable<string> exceptionMessages, string exceptionStackTrace);
}