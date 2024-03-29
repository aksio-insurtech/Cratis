// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.Observation;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Storage.MongoDB.Observation;

internal static partial class ObserverStorageLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "Getting tail sequence number for observer {ObserverId} for tenant {TenantId} and microservice {MicroserviceId}")]
    internal static partial void GettingTailSequenceNumber(this ILogger<ObserverStorage> logger, ObserverId observerId, TenantId tenantId, MicroserviceId microserviceId);

    [LoggerMessage(1, LogLevel.Trace, "Got tail sequence number {TailSequenceNumber} for observer {ObserverId} for tenant {TenantId} and microservice {MicroserviceId}")]
    internal static partial void GotTailSequenceNumber(this ILogger<ObserverStorage> logger, ObserverId observerId, TenantId tenantId, MicroserviceId microserviceId, EventSequenceNumber tailSequenceNumber);

    [LoggerMessage(2, LogLevel.Trace, "Getting next sequence number greater or equal than {SequenceNumber} for observer {ObserverId} for tenant {TenantId} and microservice {MicroserviceId}")]
    internal static partial void GettingNextSequenceNumberGreaterOrEqualThan(this ILogger<ObserverStorage> logger, ObserverId observerId, TenantId tenantId, MicroserviceId microserviceId, EventSequenceNumber sequenceNumber);

    [LoggerMessage(3, LogLevel.Trace, "Got next sequence number {NextSequenceNumber} for observer {ObserverId} for tenant {TenantId} and microservice {MicroserviceId}")]
    internal static partial void GotNextSequenceNumber(this ILogger<ObserverStorage> logger, ObserverId observerId, TenantId tenantId, MicroserviceId microserviceId, EventSequenceNumber nextSequenceNumber);
}
