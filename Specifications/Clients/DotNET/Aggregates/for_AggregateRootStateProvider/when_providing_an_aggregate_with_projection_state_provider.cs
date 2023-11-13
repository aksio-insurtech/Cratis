// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Aksio.Cratis.Properties;

namespace Aksio.Cratis.Aggregates.for_AggregateRootStateProvider;

public class when_providing_an_aggregate_with_projection_state_provider : given.an_aggregate_root_state_manager_and_two_events
{
    StateForAggregateRoot state;
    IEnumerable<AppendedEvent> result;

    void Establish()
    {
        reducers_registrar.Setup(_ => _.HasReducerFor(typeof(StateForAggregateRoot))).Returns(false);
        immediate_projections.Setup(_ => _.HasProjectionFor(typeof(StateForAggregateRoot))).Returns(true);
        state = new StateForAggregateRoot("Something");
        immediate_projections.Setup(_ => _.GetInstanceById(aggregate_root.StateType, aggregate_root._eventSourceId)).Returns(Task.FromResult(new ImmediateProjectionResult(state, Enumerable.Empty<PropertyPath>(), 0)));
        event_sequence
            .Setup(_ => _.GetForEventSourceIdAndEventTypes(aggregate_root._eventSourceId, event_types))
            .ReturnsAsync(ImmutableList<AppendedEvent>.Empty);
    }

    async Task Because() => result = await manager.Provide(aggregate_root, event_sequence.Object);

    [Fact] void should_set_state() => aggregate_root._state.ShouldEqual(state);
    [Fact] void should_not_get_events() => event_sequence.Verify(_ => _.GetForEventSourceIdAndEventTypes(aggregate_root._eventSourceId, event_types), Never());
    [Fact] void should_return_no_events() => result.ShouldBeEmpty();
}
