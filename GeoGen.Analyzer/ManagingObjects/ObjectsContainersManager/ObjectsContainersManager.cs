using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public const int NumberOfContainers = 8;

        #endregion

        #region Private fields

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
        /// <param name="factory">The factory for creating an empty objects containers.</param>
        /// <param name="tracker">The tracker for marking occurrences of a <see cref="InconsistentContainersException"/>.</param>
        public ObjectsContainersManager(IObjectsContainerFactory factory, IInconsistenciesTracker tracker = null)
        {
            _tracker = tracker;

            // Create the default number of containers
            _containers = Enumerable.Range(0, NumberOfContainers)
                    .Select(i => factory?.CreateContainer() ?? throw new ArgumentNullException(nameof(factory)))
                    .ToList();
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