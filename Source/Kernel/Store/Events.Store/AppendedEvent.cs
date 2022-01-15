// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Events.Store
{
    /// <summary>
    /// Represents an event that has been appended to an event log.
    /// </summary>
    /// <param name="Metadata">The <see cref="EventMetadata"/>.</param>
    /// <param name="EventContext">The <see cref="EventContext"/>.</param>
    /// <param name="Content">The content in the form of a JSON string.</param>
    public record AppendedEvent(EventMetadata Metadata, EventContext EventContext, string Content);
}