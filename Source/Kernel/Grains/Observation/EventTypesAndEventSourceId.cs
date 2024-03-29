// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;

namespace Aksio.Cratis.Kernel.Grains.Observation;

#pragma warning disable CA1819 // Allow arrays to be used for filtering

/// <summary>
/// Represents the filter data for the event types and event source id filter.
/// </summary>
/// <param name="EventTypes">EventTypes to filter on.</param>
/// <param name="EventSourceId">EventSourceId to filter on.</param>
public record EventTypesAndEventSourceId(EventType[] EventTypes, EventSourceId EventSourceId);
