// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using System.Text.RegularExpressions;
using Aksio.Cratis.Events.Store;
using Aksio.Cratis.Properties;

namespace Aksio.Cratis.Events.Projections.Expressions;

/// <summary>
/// Represents a <see cref="IPropertyMapperExpressionResolver"/> for adding value on a model with the value for a property on the content of an <see cref="AppendedEvent"/>.
/// </summary>
public class SubtractExpressionResolver : IPropertyMapperExpressionResolver
{
    static readonly Regex _regularExpression = new("\\$subtract\\((?<property>[A-Za-z.]*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

    /// <inheritdoc/>
    public bool CanResolve(PropertyPath targetProperty, string expression) => _regularExpression.Match(expression).Success;

    /// <inheritdoc/>
    public PropertyMapper<AppendedEvent, ExpandoObject> Resolve(PropertyPath targetProperty, string expression)
    {
        var match = _regularExpression.Match(expression);
        return PropertyMappers.SubtractWithEventValueProvider(targetProperty, EventValueProviders.FromEventContent(match.Groups["property"].Value));
    }
}
