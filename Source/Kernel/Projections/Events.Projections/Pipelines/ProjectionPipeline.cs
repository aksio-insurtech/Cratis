// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Cratis.Events.Projections.Pipelines.JobSteps;
using Cratis.Reactive;
using Microsoft.Extensions.Logging;

namespace Cratis.Events.Projections.Pipelines
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionPipeline"/>.
    /// </summary>
    public class ProjectionPipeline : IProjectionPipeline
    {
        readonly ConcurrentDictionary<ProjectionResultStoreConfigurationId, IProjectionResultStore> _resultStores = new();
        readonly ConcurrentDictionary<ProjectionResultStoreConfigurationId, ISubject<Event>> _subjectsPerConfiguration = new();
        readonly ConcurrentDictionary<ProjectionResultStoreConfigurationId, IDisposable> _subscriptionsPerConfiguration = new();
        readonly IProjectionPipelineHandler _handler;
        readonly IProjectionPipelineJobs _pipelineJobs;
        readonly ILogger<ProjectionPipeline> _logger;
        readonly BehaviorSubject<ProjectionState> _state = new(ProjectionState.Registering);
        readonly ObservableCollection<IProjectionPipelineJob> _jobs = new();

        /// <inheritdoc/>
        public IProjection Projection { get; }

        /// <inheritdoc/>
        public IProjectionEventProvider EventProvider { get; }

        /// <inheritdoc/>
        public IDictionary<ProjectionResultStoreConfigurationId, IProjectionResultStore> ResultStores => _resultStores;

        /// <inheritdoc/>
        public IObservable<ProjectionState> State => _state;

        /// <inheritdoc/>
        public IObservable<IReadOnlyDictionary<ProjectionResultStoreConfigurationId, EventLogSequenceNumber>> Positions => _handler.Positions;

        /// <inheritdoc/>
        public ProjectionState CurrentState => _state.Value;

        /// <inheritdoc/>
        public IObservableCollection<IProjectionPipelineJob> Jobs => _jobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="IProjectionPipeline"/>.
        /// </summary>
        /// <param name="projection">The <see cref="IProjection"/> the pipeline is for.</param>
        /// <param name="eventProvider"><see cref="IProjectionEventProvider"/> to use.</param>
        /// <param name="handler"><see cref="IProjectionPipelineHandler"/> to use.</param>
        /// <param name="jobs"><see cref="IProjectionPipelineJobs"/> for creating jobs.</param>
        /// <param name="logger"><see cref="ILogger{T}"/> for logging.</param>
        public ProjectionPipeline(
            IProjection projection,
            IProjectionEventProvider eventProvider,
            IProjectionPipelineHandler handler,
            IProjectionPipelineJobs jobs,
            ILogger<ProjectionPipeline> logger)
        {
            EventProvider = eventProvider;
            _handler = handler;
            _pipelineJobs = jobs;
            Projection = projection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Start()
        {
            _logger.Starting(Projection.Identifier);
            _state.OnNext(ProjectionState.CatchingUp);
            await AddAndRunJobs(_pipelineJobs.Catchup(this));
            _state.OnNext(ProjectionState.Active);

            await SetupHandling();
        }

        /// <inheritdoc/>
        public async Task Pause()
        {
            _logger.Pausing(Projection.Identifier);

            await StopAllJobs();
            foreach (var (_, subscription) in _subscriptionsPerConfiguration)
            {
                subscription.Dispose();
            }
            _subscriptionsPerConfiguration.Clear();
            _state.OnNext(ProjectionState.Paused);
        }

        /// <inheritdoc/>
        public async Task Resume()
        {
            if (CurrentState == ProjectionState.Active ||
                CurrentState == ProjectionState.CatchingUp ||
                CurrentState == ProjectionState.Rewinding)
            {
                return;
            }

            _logger.Resuming(Projection.Identifier);
            _state.OnNext(ProjectionState.CatchingUp);
            await AddAndRunJobs(_pipelineJobs.Catchup(this));
            await SetupHandling();
            _state.OnNext(ProjectionState.Active);
        }

        /// <inheritdoc/>
        public async Task Rewind()
        {
            ThrowIfRewindAlreadyInProgress();

            _logger.Rewinding(Projection.Identifier);
            _state.OnNext(ProjectionState.Rewinding);
            await AddAndRunJobs(_pipelineJobs.Rewind(this));
            _state.OnNext(ProjectionState.Active);
        }

        /// <inheritdoc/>
        public async Task Rewind(ProjectionResultStoreConfigurationId configurationId)
        {
            ThrowIfRewindAlreadyInProgressForConfiguration(configurationId);

            _logger.RewindingForConfiguration(Projection.Identifier, configurationId);
            _state.OnNext(ProjectionState.Rewinding);
            await AddAndRunJobs(new[] { _pipelineJobs.Rewind(this, configurationId) });
            _state.OnNext(ProjectionState.Active);
        }

        /// <inheritdoc/>
        public async Task Suspend(string reason)
        {
            _logger.Suspended(Projection.Identifier, reason);
            await StopAllJobs();
            foreach (var (_, subscription) in _subscriptionsPerConfiguration)
            {
                subscription.Dispose();
            }
            _subscriptionsPerConfiguration.Clear();
            _state.OnNext(ProjectionState.Suspended);
        }

        /// <inheritdoc/>
        public void StoreIn(ProjectionResultStoreConfigurationId configurationId, IProjectionResultStore resultStore)
        {
            _resultStores[configurationId] = resultStore;
            _subjectsPerConfiguration[configurationId] = new ReplaySubject<Event>();
            _handler.InitializeFor(this, configurationId);
        }

        async Task AddAndRunJobs(IEnumerable<IProjectionPipelineJob> jobs)
        {
            foreach (var job in jobs)
            {
                _jobs.Add(job);
            }

            foreach (var job in jobs)
            {
                await job.Run();
                _jobs.Remove(job);
            }
        }

        async Task StopAllJobs()
        {
            foreach (var job in _jobs)
            {
                await job.Stop();
            }
            _jobs.Clear();
        }

        async Task SetupHandling()
        {
            foreach (var (configurationId, subject) in _subjectsPerConfiguration)
            {
                var resultStore = _resultStores[configurationId];
                await EventProvider.ProvideFor(this, subject);

                if (_subscriptionsPerConfiguration.ContainsKey(configurationId))
                {
                    _subscriptionsPerConfiguration[configurationId].Dispose();
                    _subscriptionsPerConfiguration.Remove(configurationId, out _);
                }
                _subscriptionsPerConfiguration[configurationId] = Projection
                    .FilterEventTypes(subject)
                    .Subscribe(@event => _handler.Handle(@event, this, resultStore, configurationId).Wait());
            }
        }

        void ThrowIfRewindAlreadyInProgress()
        {
            if (_jobs.Any(_ => _.Name == ProjectionPipelineJobs.RewindJob))
            {
                throw new RewindAlreadyInProgress(this);
            }
        }

        void ThrowIfRewindAlreadyInProgressForConfiguration(ProjectionResultStoreConfigurationId configurationId)
        {
            var rewindJob = _jobs.FirstOrDefault(_ => _.Name == ProjectionPipelineJobs.RewindJob);
            if (rewindJob != default)
            {
                foreach (var step in rewindJob.Steps)
                {
                    if (step is Rewind rewind && rewind.ConfigurationId == configurationId)
                    {
                        throw new RewindAlreadyInProgressForConfiguration(this, configurationId);
                    }
                }
            }
        }
    }
}