// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Observation.Replaying;

/// <summary>
/// Represents a reason type for a <see cref="ReplayCandidate"/>.
/// </summary>
/// <param name="Value">The inner value.</param>
public record ReplayCandidateReasonType(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="ReplayCandidateReasonType"/> for when event types has changed.
    /// </summary>
    public static readonly ReplayCandidateReasonType EventTypesChanged = "EventTypesChanged";

    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="ReplayCandidateReasonType"/>.
    /// </summary>
    /// <param name="value">String to convert from.</param>
    public static implicit operator ReplayCandidateReasonType(string value) => new(value);
}
