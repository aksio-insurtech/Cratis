// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.Metrics;

namespace Aksio.Cratis.Kernel.Grains.EventSequences;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable MA0048 // File name must match type name

internal static partial class EventSequenceMetrics
{
    [Counter<int>("cratis-event-sequences-appended-events", "Number of events appended to the event sequence")]
    internal static partial void AppendedEvent(this IMeterScope<EventSequence> scope, EventSourceId eventSourceId, string eventName);

    [Counter<int>("cratis-event-sequences-duplicate-event-sequence-numbers", "Number of duplicate event sequence numbers")]
    internal static partial void DuplicateEventSequenceNumber(this IMeterScope<EventSequence> scope, EventSourceId eventSourceId, string eventName);

    [Counter<int>("cratis-event-sequences-failed-appended-events", "Number of events that failed to be appended to the event sequence")]
    internal static partial void FailedAppending(this IMeterScope<EventSequence> scope, EventSourceId eventSourceId, string eventName);
}

internal static class EventSequenceMetricsScopes
{
    internal static IMeterScope<EventSequence> BeginEventSequenceScope(this IMeter<EventSequence> meter, MicroserviceId microserviceId, TenantId tenantId) =>
        meter.BeginScope(new Dictionary<string, object>
        {
            ["MicroserviceId"] = microserviceId,
            ["TenantId"] = tenantId
        });
}
