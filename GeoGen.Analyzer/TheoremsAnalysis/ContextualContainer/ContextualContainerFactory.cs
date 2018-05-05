using System;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Default implementation of <see cref="IContextualContainerFactory"/> that uses 
    /// <see cref="IObjectsContainersManager"/> to safely handle possible inconsistencies.
    /// This factory creates instances of <see cref="ContextualContainer"/>.
    /// </summary>
    internal class ContextualContainerFactory : IContextualContainerFactory
    {
        #region Private fields

        /// <summary>
        /// The mapper of configurations to their containers managers.
        /// </summary>
        private readonly IObjectContainersMapper _mapper;

        /// <summary>
        /// The analytical helper to be passed to the container's constructor.
        /// </summary>
        private readonly IAnalyticalHelper _helper;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mapper">The mapper used to find containers managers for configurations.</param>
        /// <param name="helper">The analytical helper to be passed to the container.</param>
        public ContextualContainerFactory(IObjectContainersMapper mapper, IAnalyticalHelper helper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        #endregion

        #region IContextualContainerFactory implementation

        /// <summary>
        /// Creates a new contextual container that represents a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The container.</returns>
        public IContextualContainer Create(Configuration configuration)
        {
            // Pull the right containers manager for this configuration
            var manager = _mapper.Get(configuration);

            // Let the manager safely create the container's instance
            return manager.ExecuteAndResolvePossibleIncosistencies(() => new ContextualContainer(configuration, manager, _helper));
        } 

        #endregion
    }
}