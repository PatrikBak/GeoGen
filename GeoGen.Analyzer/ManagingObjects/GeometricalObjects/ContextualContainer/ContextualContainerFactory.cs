using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Default implementation of <see cref="IContextualContainerFactory"/> that uses 
    /// <see cref="IObjectsContainersManager"/> to safely handle possible inconsistencies.
    /// </summary>
    internal class ContextualContainerFactory : IContextualContainerFactory
    {
        #region Private fields

        /// <summary>
        /// The containers manager to be passed to the constructor and to
        /// safely handle possible inconsistencies while creating a container.
        /// </summary>
        private readonly IObjectsContainersManager _manager;

        /// <summary>
        /// The analytical helper to be passed to the container's constructor.
        /// </summary>
        private readonly IAnalyticalHelper _helper;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="manager">The object manager that safely creates the container.</param>
        /// <param name="helper">The analytical helper to be passed to the container.</param>
        public ContextualContainerFactory(IObjectsContainersManager manager, IAnalyticalHelper helper)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        #endregion

        #region IContextualContainerFactory implementation

        /// <summary>
        /// Creates a new contextual container filled with given configuration objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns>The container.</returns>
        public IContextualContainer Create(IEnumerable<ConfigurationObject> objects)
        {
            // Let the manager safely create the container's instance
            return _manager.ExecuteAndResolvePossibleIncosistencies(() => new ContextualContainer(objects, _manager, _helper));
        } 

        #endregion
    }
}