// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Projections;

/// <summary>
/// Exception that gets thrown when removal has already been defined.
/// </summary>
public class RemovalAlreadyDefined : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemovalAlreadyDefined"/> class.
    /// </summary>
    /// <param name="type">The type of projection.</param>
    public RemovalAlreadyDefined(Type type) : base($"Removal already defined for projection '{type.FullName}'. You can only define one event type to be the removal event type.")
    {
    }
}
