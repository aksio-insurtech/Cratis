// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Concepts;

public record PersonId(Guid Value) : ConceptAs<Guid>(Value)
{
    public static implicit operator PersonId(Guid value) => new(value);
    public static implicit operator PersonId(string value) => new(Guid.Parse(value));
    public static implicit operator PersonId(EventSourceId value) => new(Guid.Parse(value.Value));
    public static implicit operator EventSourceId(PersonId value) => new(value.ToString());
}
