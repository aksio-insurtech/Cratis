// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Grains.Operations;

/// <summary>
/// Represents details about a task.
/// </summary>
/// <param name="Value">The actual value.</param>
public record OperationDetails(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// The name of a job that is not set.
    /// </summary>
    public static readonly OperationDetails NotSet = "[Not set]";

    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="OperationDetails"/>.
    /// </summary>
    /// <param name="value">String to convert from.</param>
    public static implicit operator OperationDetails(string value) => new(value);

    /// <summary>
    /// Implicitly convert from <see cref="OperationDetails"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="value"><see cref="OperationDetails"/> to convert from.</param>
    public static implicit operator string(OperationDetails value) => value.Value;
}
