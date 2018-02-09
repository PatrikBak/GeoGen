using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsContainersManager"/>.
    /// </summary>
    internal class ObjectsContainersManager : IObjectsContainersManager
    {
        #region Public constants 

        /// <summary>
        /// The default number of container that this manager manages.
        /// </summary>
        public const int NumberOfContainers = 10;

        #endregion

        #region Private fields

        /// <summary>
        /// The loose objects constructor.
        /// </summary>
        private readonly ILooseObjectsConstructor _constructor;

        /// <summary>
        /// The list of all objects containers.
        /// </summary>
        private readonly List<IObjectsContainer> _containers;

        /// <summary>
        /// The tracker of possible inconsistencies and the number of attempts needed to 
        /// solve it. 
        /// </summary>
        private readonly IInconsistenciesTracker _tracker;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="looseObjects">The identified loose objects that are used to initialize the containers.</param>
        /// <param name="factory">The factory for creating an empty objects containers.</param>
        /// <param name="constructor">The constructor for initializing the containers with the loose objects.</param>
        /// <param name="tracker">The tracker for marking occurrences of a <see cref="InconsistentContainersException"/>.</param>
        public ObjectsContainersManager
        (
            IEnumerable<LooseConfigurationObject> looseObjects,
            IObjectsContainerFactory factory,
            ILooseObjectsConstructor constructor,
            IInconsistenciesTracker tracker = null
        )
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _tracker = tracker;

            // Create the default number of containers
            _containers = Enumerable.Range(0, NumberOfContainers)
                    .Select(i => factory.CreateContainer())
                    .ToList();

            // Initialize all of them with the passed loose objects
            Initialize(looseObjects);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes all containers with given loose objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        private void Initialize(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            // Enumerate the loose objects
            var looseObjectsList = looseObjects.ToList();

            // Find their ids (which must exist)
            var ids = looseObjectsList.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"))
                    .Distinct()
                    .ToList();

            // Check if there are objects with duplicate ids.
            if (ids.Count != looseObjectsList.Count)
                throw new ArgumentException("Duplicate objects");

            // Add loose objects to all containers
            foreach (var container in _containers)
            {
                // Add the objects to the container using the loose objects constructor
                container.Add(looseObjectsList, c => _constructor.Construct(looseObjectsList));
            }
        }

        #endregion

        #region IObjectsContainersManager methods

        /// <summary>
        /// Performs a given function and handles the <see cref="InconsistentContainersException"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The result of the function.</returns>
        public T ExecuteAndResolvePossibleIncosistencies<T>(Func<T> function)
        {
            // Try to perform the function until there is no inconsistency
            while (true)
            {
                try
                {
                    // Try to call the function
                    return function();
                }
                catch (InconsistentContainersException)
                {
                    // If an inconsistency occurs, mark it
                    _tracker?.MarkInconsistency();

                    // And reconstruct all containers
                    foreach (var container in _containers)
                    {
                        container.Reconstruct();
                    }
                }
            }
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IObjectsContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}