using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The default implementation of <see cref="IObjectsContainersManager"/>. This class uses
    /// <see cref="IInconsistentContainersTracer"/> to trace all the interesting information 
    /// about inconsistencies.
    /// </summary>
    public class ObjectsContainersManager : IObjectsContainersManager
    {
        #region Dependencies

        /// <summary>
        /// The tracer of occurrences of inconsistencies between containers.
        /// </summary>
        private readonly IInconsistentContainersTracer _tracer;

        #endregion

        #region Private fields

        /// <summary>
        /// The list of all the objects containers managed by this manager.
        /// </summary>
        private readonly List<IObjectsContainer> _containers;

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.
        /// </summary>
        private readonly int _maximalAttemptsToReconstructAllContainers;

        /// <summary>
        /// The maximal number of attempts to reconstruct a single objects container.
        /// </summary>
        private readonly int _maximalAttemptsToReconstructOneContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsContainersManager"/> class.
        /// </summary>
        /// <param name="numberOfContainers">The number of containers to be used by the manager.</param>
        /// <param name="maximalAttemptsToReconstructAllContainers">The maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.</param>
        /// <param name="maximalAttemptsToReconstructOneContainer">The maximal number of attempts to reconstruct a single objects container.</param>
        /// <param name="factory">The factory for creating empty objects containers.</param>
        /// <param name="tracer">The tracer of occurrences of inconsistencies between containers.</param>
        public ObjectsContainersManager(int numberOfContainers, int maximalAttemptsToReconstructOneContainer, int maximalAttemptsToReconstructAllContainers, IObjectsContainerFactory factory, IInconsistentContainersTracer tracer = null)
        {
            // Make sure all the passed values are > 0
            Ensure.IsGreaterThanZero(numberOfContainers, nameof(numberOfContainers));
            Ensure.IsGreaterThanZero(maximalAttemptsToReconstructOneContainer, nameof(maximalAttemptsToReconstructOneContainer));
            Ensure.IsGreaterThanZero(maximalAttemptsToReconstructAllContainers, nameof(maximalAttemptsToReconstructAllContainers));

            // Assign maximal attempts
            _maximalAttemptsToReconstructOneContainer = maximalAttemptsToReconstructOneContainer;
            _maximalAttemptsToReconstructAllContainers = maximalAttemptsToReconstructAllContainers;

            // Create the requested number of containers
            _containers = Enumerable.Range(0, numberOfContainers).Select(i => factory.CreateContainer()).ToList();

            // Assign tracer
            _tracer = tracer;
        }

        #endregion

        #region IObjectsContainersManager methods

        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentContainersException"/> and tries 
        /// to handle it. If the exception couldn't be handled, throws an <see cref="UnresolvableInconsistencyException"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the function to be executed.</typeparam>
        /// <param name="function">The function to be executed.</param>
        /// <returns>The returned result of the executed function.</returns>
        public T ExecuteAndResolvePossibleIncosistencies<T>(Func<T> function)
        {
            // Prepare a variable holding the number of attempts to reconstruct all the containers
            var attempts = 0;

            // Prepare a variable holding the final result
            var result = default(T);

            // Try to perform the function until we reach the maximal number of attempts
            while (attempts < _maximalAttemptsToReconstructAllContainers)
            {
                try
                {
                    // Call the function
                    result = function();

                    // If we got here, we're happy and we can break the cycle
                    break;
                }
                // If there was an inconsistency...
                catch (InconsistentContainersException)
                {
                    // Mark an attempt 
                    attempts++;

                    // Try to reconstruct all the containers
                    _containers.ForEach(TryReconstruct);
                }
            }

            // If we reached the maximal number of reconstructions...
            if (attempts == _maximalAttemptsToReconstructAllContainers)
            {
                // Then we want to trace it 
                _tracer?.TraceReachingMaximalNumberOfAttemptsToReconstructAllContainers(this);

                // And let the caller know
                throw new UnresolvableInconsistencyException();
            }

            // If there was an inconsistency, i.e. we needed to reconstruct 
            // all the containers at least once, then we want to trace it
            if (attempts != 0)
                _tracer?.TraceResolvedInconsistency(this, attempts);

            // And finally return the result
            return result;
        }

        /// <summary>
        /// Tries to reconstruct a single container for the maximal number of allowed times.
        /// </summary>
        /// <param name="container">The container to be reconstructed.</param>
        private void TryReconstruct(IObjectsContainer container)
        {
            // Prepare a variable holding the number of attempts to reconstruct
            var attempts = 0;

            // Try until we reach the maximal allowed number of attempts
            while (attempts < _maximalAttemptsToReconstructOneContainer)
            {
                // Mark an attempt
                attempts++;

                // Try to reconstruct
                container.TryReconstruct(out var successful);

                // If it went fine, we can break
                if (successful)
                    break;
            }

            // If we reached the maximal number of reconstructions...
            if (attempts == _maximalAttemptsToReconstructOneContainer)
            {
                // Then we want to trace it 
                _tracer?.TraceReachingMaximalNumberOfAttemptsToReconstructOneContainer(this, container);

                // And let the caller know
                throw new UnresolvableInconsistencyException();
            }

            // If there were some unsuccessful attempts, trace them. 
            // One was successful, so there are 'attempts-1' of them
            if (attempts != 1)
                _tracer?.TraceUnsuccessfulAtemptsToReconstructOneContainer(this, container, attempts - 1);
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<IObjectsContainer> GetEnumerator() => _containers.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}