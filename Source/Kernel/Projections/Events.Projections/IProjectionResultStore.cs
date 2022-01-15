// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Cratis.Changes;

namespace Cratis.Events.Projections
{
    /// <summary>
    /// Defines the storage for <see cref="IProjection">projections</see>.
    /// </summary>
    public interface IProjectionResultStore
    {
        /// <summary>
        /// Gets the <see cref="ProjectionResultStoreTypeId"/> that identifies the store.
        /// </summary>
        ProjectionResultStoreTypeId TypeId { get; }

        /// <summary>
        /// Gets the display friendly <see cref="ProjectionResultStoreTypeName"/>.
        /// </summary>
        ProjectionResultStoreTypeName Name { get; }

        /// <summary>
        /// Find a model by key, or return an empty object if not found.
        /// </summary>
        /// <param name="key">Key of the model to find.</param>
        /// <returns>A model instance with the data from the source, or an empty object.</returns>
        Task<ExpandoObject> FindOrDefault(object key);

        /// <summary>
        /// Update or insert model based on key.
        /// </summary>
        /// <param name="key">Key of the model to upsert.</param>
        /// <param name="changeset">All changes in the form of a <see cref="Changeset{Event, ExpandoObject}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ApplyChanges(object key, IChangeset<Event, ExpandoObject> changeset);

        /// <summary>
        /// Prepare the store for an initial run.
        /// </summary>
        /// <remarks>
        /// Typically the store will clear out any existing data. This is to be able to guarantee
        /// the idempotency of the projection.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PrepareInitialRun();

        /// <summary>
        /// Begins rewind mode for the result store.
        /// </summary>
        /// <returns><see cref="IProjectionResultStoreRewindScope"/>.</returns>
        /// <remarks>
        /// The rewind scope returned is a disposable. When rewind is done, one should
        /// dispose of this. Depending on the implementation of it, it will perform
        /// necessary cleanup after a rewind has been performed.
        /// </remarks>
        /// <returns>A <see cref="IProjectionResultStoreRewindScope"/>.</returns>.
        IProjectionResultStoreRewindScope BeginRewind();
    }
}