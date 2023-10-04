// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Grains.Jobs;

/// <summary>
/// Represents an event that occurred for a <see cref="IJob{TRequest}"/>.
/// </summary>
public class JobEvent
{
    /// <summary>
    /// Gets or sets the <see cref="JobStatus"/>.
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when the event occurred.
    /// </summary>
    public DateTimeOffset Occurred { get; set; }
}