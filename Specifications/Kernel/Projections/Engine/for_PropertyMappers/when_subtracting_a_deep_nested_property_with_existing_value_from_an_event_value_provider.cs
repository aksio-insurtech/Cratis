// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using System.Text.Json.Nodes;
using Aksio.Cratis.Events.Store;
using Aksio.Cratis.Properties;

namespace Aksio.Cratis.Events.Projections.for_PropertyMappers
{
    public class when_subtracting_a_deep_nested_property_with_existing_value_from_an_event_value_provider : Specification
    {
        PropertyMapper<AppendedEvent, ExpandoObject> property_mapper;
        AppendedEvent @event;
        ExpandoObject result;
        AppendedEvent provided_event;

        void Establish()
        {
            result = new();
            @event = new(new(0, new("02405794-91e7-4e4f-8ad1-f043070ca297", 1)), new("2f005aaf-2f4e-4a47-92ea-63687ef74bd4", DateTimeOffset.UtcNow), new JsonObject());

            dynamic target = result;
            target.deep = new ExpandoObject();
            target.deep.nested = new ExpandoObject();
            target.deep.nested.property = 42d;
            property_mapper = PropertyMappers.SubtractWithEventValueProvider("deep.nested.property", _ =>
            {
                provided_event = _;
                return 2d;
            });
        }

        void Because() => property_mapper(@event, result);

        [Fact] void should_result_in_expected_value() => ((object)((dynamic)result).deep.nested.property).ShouldEqual(40d);
        [Fact] void should_pass_the_event_to_the_value_provider() => provided_event.ShouldEqual(@event);
    }
}