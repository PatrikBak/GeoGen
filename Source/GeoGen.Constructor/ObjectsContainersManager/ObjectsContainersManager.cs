using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IObjectsContainersManager"/>. 
    /// </summary>
    public class ObjectsContainersManager : IObjectsContainersManager
    {
        #region Private fields

        /// <summary>
        /// The list of all the objects containers managed by this manager.
        /// </summary>
        private readonly List<IObjectsContainer> _containers;

        /// <summary>
        /// The settings for the manager.
        /// </summary>
        private readonly ObjectsContainersManagerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsContainersManager"/> class.
        /// </summary>
        /// <param name="settings">The settings for the manager.</param>
        /// <param name="factory">The factory for creating empty objects containers.</param>
        public ObjectsContainersManager(ObjectsContainersManagerSettings settings, IObjectsContainerFactory factory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Create the requested number of containers
            _containers = Enumerable.Range(0, settings.NumberOfContainers).Select(i => factory.CreateContainer()).ToList();
        }

        #endregion

        #region IObjectsContainersManager implementation

        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentContainersException"/> and tries 
        /// to handle it. If the exception couldn't be handled, throws an <see cref="UnresolvableInconsistencyException"/>.
        /// </summary>
        /// <param name="function">The action to be executed.</param>
        /// <param name="exceptionCallback">The action called after an <see cref="InconsistentContainersException"/> occurs.</param>
        public void ExecuteAndResolvePossibleIncosistencies(Action action, Action<InconsistentContainersException> exceptionCallback)
        {
            // Prepare a variable holding the number of attempts to reconstruct all the containers
            var attempts = 0;

            // Try to perform the function until we reach the maximal number of attempts
            while (attempts < _settings.MaximalAttemptsToReconstructAllContainers)
            {
                try
                {
                    // Call the action
                    action();

                    // If we got here, we're happy and we can break the cycle
                    break;
                }
                // If there was an inconsistency...
                catch (InconsistentContainersException e)
                {
                    // Call the callback
                    exceptionCallback(e);

                    // Mark an attempt 
                    attempts++;

                    // Try to reconstruct all the containers
                    TryReconstructContainers();
                }
            }

            // If we reached the maximal number of reconstructions, we've failed as well
            if (attempts == _settings.MaximalAttemptsToReconstructAllContainers)
                throw new UnresolvableInconsistencyException(
                    "The reconstruction of the containers failed, because the maximum number of attempts " +
                   $"({_settings.MaximalAttemptsToReconstructAllContainers}) to do so has been reached.");
        }

        /// <summary>
        /// Tries to reconstruct all the containers that this manager manages. 
        /// If the exception couldn't be handled, throws an <see cref="UnresolvableInconsistencyException"/>.
        /// </summary>
        public void TryReconstructContainers()
        {
            // Go through all the containers
            foreach (var container in _containers)
            {
                // Reconstruct the given one
                TryReconstruct(container, out var success);

                // If it failed, the whole reconstruction has
                if (!success)
                    throw new UnresolvableInconsistencyException(
                        "The reconstruction of the containers failed, because the maximum number of attempts " +
                       $"({_settings.MaximalAttemptsToReconstructOneContainer}) to reconstructed a container has been reached.");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tries to reconstruct a single container for the maximal number of allowed times.
        /// </summary>
        /// <param name="container">The container to be reconstructed.</param>
        /// <param name="success">The parameter indicating if the reconstruction was successful.</param>
        private void TryReconstruct(IObjectsContainer container, out bool success)
        {
            // Prepare a variable holding the number of attempts to reconstruct
            var attempts = 0;

            // Try until we reach the maximal allowed number of attempts
            while (attempts < _settings.MaximalAttemptsToReconstructOneContainer)
            {
                // Mark an attempt
                attempts++;

                // Try to reconstruct
                container.TryReconstruct(out var successful);

                // If it went fine, we can break
                if (successful)
                    break;
            }

            // Set whether we've succeeded based on the maximal number of reconstructions...
            success = attempts != _settings.MaximalAttemptsToReconstructOneContainer;
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