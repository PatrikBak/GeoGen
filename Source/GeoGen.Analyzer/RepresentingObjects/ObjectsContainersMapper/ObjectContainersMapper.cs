using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IObjectContainersMapper"/>.
    /// </summary>
    public class ObjectContainersMapper : IObjectContainersMapper
    {
        #region Private fields

        /// <summary>
        /// The factory for creating <see cref="IObjectsContainersManager"/>.
        /// </summary>
        private readonly IObjectsContainersManagerFactory _factory;

        /// <summary>
        /// The dictionary mapping ids of configurations to the particular managers.
        /// </summary>
        private readonly Dictionary<int, IObjectsContainersManager> _configurationIdToManagers;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="factory">The factory for creating new managers.</param>
        public ObjectContainersMapper(IObjectsContainersManagerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _configurationIdToManagers = new Dictionary<int, IObjectsContainersManager>();
        }

        #endregion

        #region IObjectContainersMapper implementation

        /// <summary>
        /// Gets the manager corresponding to a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager.</returns>
        public IObjectsContainersManager Get(Configuration configuration)
        {
            try
            {
                // Try return the result
                return _configurationIdToManagers[configuration.Id];
            }
            catch (KeyNotFoundException)
            {
                throw new AnalyzerException("No manager has been created for this configuration.");
            }
        }

        /// <summary>
        /// Creates a new manager for a given configuration and returns it.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager.</returns>
        public IObjectsContainersManager Create(Configuration configuration)
        {
            try
            {
                // Create a new manager
                var manager = _factory.Create();

                // Try to add the id and the new manager
                _configurationIdToManagers.Add(configuration.Id, manager);

                // Return the manager
                return manager;
            }
            catch (ArgumentException)
            {
                throw new AnalyzerException("The configuration with this id already has the manager.");
            }
        } 

        #endregion
    }
}