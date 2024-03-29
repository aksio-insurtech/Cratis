// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Observation.Replaying;

/// <summary>
/// Represents the base for a reason for a replay candidate.
/// </summary>
/// <param name="Type">The <see cref="ReplayCandidateReasonType"/>.</param>
public record ReplayCandidateReason(ReplayCandidateReasonType Type);
