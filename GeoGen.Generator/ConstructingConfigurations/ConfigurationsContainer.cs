using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsContainer"/>.
    /// </summary>
    internal class ConfigurationsContainer : StringBasedContainer<Configuration>, IConfigurationsContainer
    {
        #region Private fields

        /// <summary>
        /// The configuration constructor.
        /// </summary>
        private readonly IConfigurationConstructor _configurationConstructor;

        /// <summary>
        /// The configuration to string provider.
        /// </summary>
        private readonly IConfigurationToStringProvider _configurationToStringProvider;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        /// <summary>
        /// The default full object to string provider.
        /// </summary>
        private readonly DefaultFullObjectToStringProvider _objectToStringProvider;

        #endregion

        #region IConfigurationContainer properties

        /// <summary>
        /// Gets the current layer of unprocessed configurations
        /// </summary>
        public List<ConfigurationWrapper> CurrentLayer { get; } = new List<ConfigurationWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration container. This container needs to use 
        /// <see cref="IConfigurationConstructor"/>, for constructing 
        /// <see cref="ConfigurationWrapper"/>s from the initial configuration and the
        /// constructor output. Then it uses a <see cref="ConfigurationObjectsContainer"/>,
        /// for adding new objects from the output. Since it's string bases, it also needs a
        /// <see cref="IConfigurationToStringProvider"/>, which needs an implementation
        /// of <see cref="IObjectToStringProvider"/> to be passed. This container uses the
        /// <see cref="DefaultFullObjectToStringProvider"/>.
        /// </summary>
        /// <param name="configurationConstructor">The configuration constructor.</param>
        /// <param name="configurationToStringProvider">The configuration to string provider.</param>
        /// <param name="configurationObjectsContainer">The configuration objects container.</param>
        /// <param name="objectToStringProvider">The default full object to string provider.</param>
        public ConfigurationsContainer
        (
            IConfigurationConstructor configurationConstructor,
            IConfigurationToStringProvider configurationToStringProvider,
            IConfigurationObjectsContainer configurationObjectsContainer,
            DefaultFullObjectToStringProvider objectToStringProvider
        )
        {
            _configurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationToStringProvider = configurationToStringProvider ?? throw new ArgumentNullException(nameof(configurationToStringProvider));
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(configurationObjectsContainer));
            _objectToStringProvider = objectToStringProvider ?? throw new ArgumentNullException(nameof(objectToStringProvider));
        }

        #endregion

        #region IConfigurationContainer methods

        /// <summary>
        /// Initializes the container with a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        public void Initialize(Configuration initialConfiguration)
        {
            if (initialConfiguration == null)
                throw new ArgumentNullException(nameof(initialConfiguration));

            // Clear the current state of the container
            Items.Clear();

            // Initialize the objects container with this configuration
            _configurationObjectsContainer.Initialize(initialConfiguration);

            // Add the initial configuration to the container
            Add(initialConfiguration);

            // Let the constructor construct wrapper for the initial configuration.
            var configurationWrapper = _configurationConstructor.ConstructWrapper(initialConfiguration);

            // Set the current layer items
            CurrentLayer.SetItems(configurationWrapper.SingleItemAsEnumerable());
        }

        /// <summary>
        /// Processes a new layer of a constructor output.
        /// </summary>
        /// <param name="newLayerOutput">The new layer output.</param>
        public void AddLayer(List<ConstructorOutput> newLayerOutput)
        {
            if (newLayerOutput == null)
                throw new ArgumentNullException(nameof(newLayerOutput));

            // Clear the current layer
            CurrentLayer.Clear();

            foreach (var output in newLayerOutput)
            {
                // Add objects to the container and get their identified versions
                var newObjects = output.ConstructedObjects
                        .Select(o => _configurationObjectsContainer.Add(o))
                        .ToList();

                // Pull initial constructed objects
                var initialObjects = output.InitialConfiguration.Configuration.ConstructedObjects;

                // Check if the configuration doesn't contain any of the new objects
                // If yes, we then the configuration shouldn't be taken into account
                if (initialObjects.Any(s => newObjects.Contains(s)))
                    continue;

                // Otherwise re-assign the output
                output.ConstructedObjects = newObjects;

                // Let the constructor create a new wrapper
                var configuration = _configurationConstructor.ConstructWrapper(output);

                // Add the representant to the container
                var result = Add(configuration.Configuration);

                // If the container changed its content, then we can add the configuration
                // to the new layer
                if (result)
                    CurrentLayer.Add(configuration);
            }
        }

        #endregion

        #region StringBasedContainer abstract methods

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The given item.</param>
        /// <returns>The string representation.</returns>
        protected override string ItemToString(Configuration item)
        {
            return _configurationToStringProvider.ConvertToString(item, _objectToStringProvider);
        }

        #endregion
    }
}