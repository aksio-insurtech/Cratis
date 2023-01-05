// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;

namespace Aksio.Cratis.Events.Schemas.Api;

/// <summary>
/// Representation of an event type registration.
/// </summary>
/// <param name="Type">Type of event.</param>
/// <param name="FriendlyName">A friendly name.</param>
/// <param name="Schema">The actual schema of the event type.</param>
public record EventTypeRegistration(EventType Type, string FriendlyName, JsonNode Schema);
