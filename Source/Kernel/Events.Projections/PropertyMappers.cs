// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Cratis.Dynamic;
using Cratis.Properties;

namespace Cratis.Events.Projections
{
    /// <summary>
    /// Represents utilities for creating <see cref="PropertyMapper{Event}"/> for different scenarios.
    /// </summary>
    public static class PropertyMappers
    {
        /// <summary>
        /// Create a <see cref="PropertyMapper{Event}"/> that can copies content provided by a <see cref="ValueProvider{Event}"/> to a target property.
        /// </summary>
        /// <param name="targetProperty">Target property.</param>
        /// <param name="eventValueProvider"><see cref="ValueProvider{Event}"/> to use as source.</param>
        /// <returns>A new <see cref="PropertyMapper{Event}"/>.</returns>
        public static PropertyMapper<Event> FromEventValueProvider(Property targetProperty, ValueProvider<Event> eventValueProvider)
        {
            return (Event @event, ExpandoObject target) =>
            {
                var actualTarget = target.EnsurePath(targetProperty) as IDictionary<string, object>;
                actualTarget[targetProperty.LastSegment] = eventValueProvider(@event);
            };
        }

        /// <summary>
        /// Create a <see cref="PropertyMapper{Event}"/> that can add a property with a value provided by a <see cref="ValueProvider{Event}"/> onto a target property.
        /// </summary>
        /// <param name="targetProperty">Target property.</param>
        /// <param name="eventValueProvider"><see cref="ValueProvider{Event}"/> to use as source.</param>
        /// <returns>A new <see cref="PropertyMapper{Event}"/>.</returns>
        public static PropertyMapper<Event> AddWithEventValueProvider(Property targetProperty, ValueProvider<Event> eventValueProvider)
        {
            return (Event @event, ExpandoObject target) =>
            {
                var lastSegment = targetProperty.LastSegment;
                var actualTarget = target.EnsurePath(targetProperty) as IDictionary<string, object>;
                if (!actualTarget.ContainsKey(lastSegment))
                {
                    actualTarget[lastSegment] = (double)0;
                }
                var value = (double)actualTarget[lastSegment];
                value += (double)eventValueProvider(@event);
                actualTarget[lastSegment] = value;
            };
        }

        /// <summary>
        /// Create a <see cref="PropertyMapper{Event}"/> that can add a property with a value provided by a <see cref="ValueProvider{Event}"/> onto a target property.
        /// </summary>
        /// <param name="targetProperty">Target property.</param>
        /// <param name="eventValueProvider"><see cref="ValueProvider{Event}"/> to use as source.</param>
        /// <returns>A new <see cref="PropertyMapper{Event}"/>.</returns>
        public static PropertyMapper<Event> SubtractWithEventValueProvider(Property targetProperty, ValueProvider<Event> eventValueProvider)
        {
            return (Event @event, ExpandoObject target) =>
            {
                var lastSegment = targetProperty.LastSegment;
                var actualTarget = target.EnsurePath(targetProperty) as IDictionary<string, object>;
                if (!actualTarget.ContainsKey(lastSegment))
                {
                    actualTarget[lastSegment] = (double)0;
                }
                var value = (double)actualTarget[lastSegment];
                value -= (double)eventValueProvider(@event);
                actualTarget[lastSegment] = value;
            };
        }
    }
}
