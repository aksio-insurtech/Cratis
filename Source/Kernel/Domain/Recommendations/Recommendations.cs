// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Grains.Recommendations;
using Aksio.Cratis.Recommendations;
using Microsoft.AspNetCore.Mvc;

namespace Aksio.Cratis.Kernel.Domain.Recommendations;

/// <summary>
/// Represents the API for working with recommendations.
/// </summary>
[Route("/api/events/store/{microserviceId}/{tenantId}/recommendations")]
public class Recommendations : ControllerBase
{
    readonly IGrainFactory _grainFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Recommendations"/> class.
    /// </summary>
    /// <param name="grainFactory"><see cref="IGrainFactory"/> for getting grains.</param>
    public Recommendations(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    /// <summary>
    /// Perform a recommendation.
    /// </summary>
    /// <param name="microserviceId">The <see cref="MicroserviceId"/> for the recommendation.</param>
    /// <param name="tenantId">The <see cref="TenantId"/> for the recommendation.</param>
    /// <param name="recommendationId">The <see cref="RecommendationId"/> of the recommendation to perform.</param>
    /// <returns>Awaitable task.</returns>
    [HttpPost("{recommendationId}/perform")]
    public async Task Perform(
        [FromRoute] MicroserviceId microserviceId,
        [FromRoute] TenantId tenantId,
        [FromRoute] RecommendationId recommendationId)
    {
        await GetRecommendationsManager(microserviceId, tenantId).Perform(recommendationId);
    }

    /// <summary>
    /// Ignore a recommendation.
    /// </summary>
    /// <param name="microserviceId">The <see cref="MicroserviceId"/> for the recommendation.</param>
    /// <param name="tenantId">The <see cref="TenantId"/> for the recommendation.</param>
    /// <param name="recommendationId">The <see cref="RecommendationId"/> of the recommendation to ignore.</param>
    /// <returns>Awaitable task.</returns>
    [HttpPost("{recommendationId}/ignore")]
    public async Task Ignore(
        [FromRoute] MicroserviceId microserviceId,
        [FromRoute] TenantId tenantId,
        [FromRoute] RecommendationId recommendationId)
    {
        await GetRecommendationsManager(microserviceId, tenantId).Ignore(recommendationId);
    }

    IRecommendationsManager GetRecommendationsManager(MicroserviceId microserviceId, TenantId tenantId) =>
        _grainFactory.GetGrain<IRecommendationsManager>(0, new RecommendationsManagerKey(microserviceId, tenantId));
}
