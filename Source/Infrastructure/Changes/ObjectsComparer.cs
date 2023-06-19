// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Reflection;
using Aksio.Collections;
using Aksio.Concepts;
using Aksio.Cratis.Properties;
using Aksio.Reflection;

namespace Aksio.Cratis.Changes;

/// <summary>
/// Represents an implementation of <see cref="IObjectsComparer"/>.
/// </summary>
public class ObjectsComparer : IObjectsComparer
{
    /// <inheritdoc/>
    public bool Equals(object? left, object? right, out IEnumerable<PropertyDifference> differences)
    {
        var allDifferences = new List<PropertyDifference>();
        differences = allDifferences;

        var type = left?.GetType() ?? right?.GetType();
        if (type is null)
        {
            return true;
        }

        if (type.IsAssignableTo(typeof(IDictionary<string, object>)))
        {
            CompareDictionaryValues((left as IDictionary<string, object>)!, (right as IDictionary<string, object>)!, new PropertyPath(string.Empty), allDifferences);
        }
        else
        {
            ComparePropertiesFor(type, left, right, new PropertyPath(string.Empty), allDifferences);
        }

        return allDifferences.Count == 0;
    }

    void ComparePropertiesFor(Type type, object? left, object? right, PropertyPath currentPropertyPath, List<PropertyDifference> differences)
    {
        foreach (var property in type.GetProperties())
        {
            var leftValue = left != null ? property.GetValue(left) : null;
            var rightValue = right != null ? property.GetValue(right) : null;
            if (leftValue is null && rightValue is null) continue;

            var propertyPath = currentPropertyPath + (type.IsEnumerable() ? $"[{property.Name}]" : property.Name);
            CompareValues(property.PropertyType, leftValue, rightValue, propertyPath, differences);
        }
    }

    void CompareDictionaryValues(IDictionary<string, object> left, IDictionary<string, object> right, PropertyPath currentPropertyPath, List<PropertyDifference> differences)
    {
        var keys = left.Keys.ToList();
        keys.AddRange(right.Keys);

        foreach (var key in keys.Distinct())
        {
            var leftValue = left!.ContainsKey(key) ? left[key] : null;
            var rightValue = right!.ContainsKey(key) ? right[key] : null;

            var type = leftValue?.GetType() ?? rightValue?.GetType();
            if (type is null)
            {
                continue;
            }

            var propertyPath = currentPropertyPath + (type.IsEnumerable() ? $"[{key}]" : key);
            CompareValues(type, leftValue, rightValue, propertyPath, differences);
        }
    }

    void CompareValues(Type type, object? leftValue, object? rightValue, PropertyPath propertyPath, List<PropertyDifference> differences)
    {
        if (leftValue is null && rightValue is null) return;

        if ((leftValue is null && rightValue is not null) ||
          (leftValue is not null && rightValue is null))
        {
            differences.Add(new PropertyDifference(propertyPath, leftValue, rightValue));
        }
        else if (type.IsAssignableTo(typeof(IDictionary<string, object>)))
        {
            CompareDictionaryValues((leftValue as IDictionary<string, object>)!, (rightValue as IDictionary<string, object>)!, propertyPath, differences);
        }
        else if (!type.IsPrimitive &&
          type != typeof(Guid) &&
          type != typeof(string) &&
          !type.IsConcept() &&
          !type.IsEnumerable() &&
          !type.IsComparable())
        {
            ComparePropertiesFor(type, leftValue, rightValue, propertyPath, differences);
        }
        else if (leftValue is not null && rightValue is not null && type.IsEnumerable())
        {
            var leftValueAsEnumerable = (leftValue as IEnumerable)!;
            var rightValueAsEnumerable = (rightValue as IEnumerable)!;
            if (leftValueAsEnumerable.CountElements() != rightValueAsEnumerable.CountElements())
            {
                differences.Add(new PropertyDifference(propertyPath, leftValue, rightValue));
            }
            else
            {
                var leftElementType = leftValueAsEnumerable.GetType().GetElementType();
                var rightElementType = rightValueAsEnumerable.GetType().GetElementType();
                var leftElements = leftValueAsEnumerable.ToObjectArray();
                var rightElements = rightValueAsEnumerable.ToObjectArray();

                if (leftElementType == rightElementType &&
                    (leftElementType?.IsPrimitive == true || leftElementType == typeof(string)))
                {
                    for (var i = 0; i < leftElements.Length; i++)
                    {
                        var elementDifferences = new List<PropertyDifference>();
                        CompareValues(leftElementType, leftElements[i], rightElements[i], propertyPath, elementDifferences);
                        differences.AddRange(elementDifferences);

                        if (elementDifferences.Count > 0) break;
                    }
                }
                else
                {
                    for (var i = 0; i < leftElements.Length; i++)
                    {
                        var elementDifferences = new List<PropertyDifference>();
                        CompareValues(
                            leftElements[i]?.GetType() ?? rightElements[i]?.GetType() ?? typeof(object),
                            leftElements[i],
                            rightElements[i],
                            propertyPath,
                            elementDifferences);
                        differences.AddRange(elementDifferences);

                        if (elementDifferences.Count > 0) break;
                    }
                }
            }
        }
        else
        {
            var different = false;

            if (leftValue!.GetType() != rightValue!.GetType())
            {
                differences.Add(new PropertyDifference(propertyPath, leftValue, rightValue));
                return;
            }

            if (type.IsComparable())
            {
                if (type.Implements(typeof(IComparable)))
                {
                    different = (leftValue as IComparable)!.CompareTo(rightValue) != 0;
                }
                else
                {
                    var comparableInterface = type.GetInterface(typeof(IComparable<>).Name);
                    var compareToMethod = comparableInterface!.GetMethod(nameof(IComparable<object>.CompareTo), BindingFlags.Public | BindingFlags.Instance);
                    different = ((int)compareToMethod!.Invoke(leftValue, new[] { rightValue })!) != 0;
                }
            }
            else if (!leftValue!.Equals(rightValue))
            {
                different = true;
            }

            if (different)
            {
                differences.Add(new PropertyDifference(propertyPath, leftValue, rightValue));
            }
        }
    }
}
