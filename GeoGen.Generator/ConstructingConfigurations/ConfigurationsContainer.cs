using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsContainer"/>.
    /// </summary>
    internal sealed class ConfigurationsContainer : StringBasedContainer<Configuration>, IConfigurationsContainer
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

        /// <summary>
        /// The set of ids that are forbidden to be in configurations.
        /// </summary>
        private readonly HashSet<int> _forbiddenObjectsId;

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
            _forbiddenObjectsId = new HashSet<int>();
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
        /// Processes a new layer of constructors outputs and returns the configurations.
        /// </summary>
        /// <param name="output">The new layer output.</param>
        /// <returns>The configuration wrappers.</returns>
        public IEnumerable<ConfigurationWrapper> AddLayer(IEnumerable<ConstructorOutput> output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            // Initialize the list of items that's gonna be the new layer items
            var items = new List<ConfigurationWrapper>();

            foreach (var currentOutput in output)
            {
                // First pull initial constructed objects so we can compare
                // them with new ones
                var initialObjects = currentOutput.InitialConfiguration.Configuration.ConstructedObjects;

                // Initialize a boolean value indicating if we should skip this 
                // configuration
                var skipConfiguration = false;

                // Initialize the new constructed objects list (with objects with set id)
                var newObjects = new List<ConstructedConfigurationObject>();

                // Iterate over constructed objects
                foreach (var constructedObject in currentOutput.ConstructedObjects)
                {
                    // Get the result from the container
                    var containerResult = _configurationObjectsContainer.Add(constructedObject);

                    // If the object is currently in the configuration
                    if (initialObjects.Contains(containerResult))
                    {
                        // Set the flag indicating to skip the configuration
                        skipConfiguration = true;

                        // Break the cycle
                        break;
                    }

                    // Pull id
                    var id = containerResult.Id ?? throw new GeneratorException("Id must be set");

                    // If the object with this id is forbidden
                    if (_forbiddenObjectsId.Contains(id))
                    {
                        // Set the flag indicating to skip the configuration
                        skipConfiguration = true;

                        // Break the cycle
                        break;
                    }

                    // Otherwise we add the result to the new objects
                    newObjects.Add(containerResult);
                }

                // If we should skip the configuration, skip it
                if (skipConfiguration)
                    continue;

                // Otherwise re-assign the output
                currentOutput.ConstructedObjects = newObjects;

                // Let the constructor create a new wrapper
                var configuration = _configurationConstructor.ConstructWrapper(currentOutput);

                // We need to check again if the objects ids aren't forbidden, cause
                // they might have changed
                // TODO: Isn't it enough to check new objects? I'm not sure
                var notForbidden = configuration.AllObjectsMap
                        .AllObjects()
                        .Select(o => o.Id ?? throw new GeneratorException("Id must be set"))
                        .All(id => !_forbiddenObjectsId.Contains(id));

                // If there is a new forbidden object
                if (!notForbidden)
                {
                    // Skip it
                    continue;
                }

                // Add the configuration to the container.
                if (!Add(configuration.Configuration))
                {
                    // If the content hasn't changed, continue
                    continue;
                }

                // Otherwise we have a new configuration. We add it to the items list
                items.Add(configuration);

                // And yield it
                yield return configuration;
            }

            // Update the current layer
            CurrentLayer.SetItems(items);
        }

        /// <summary>
        /// Forbids all configurations that contains a given configuration object.
        /// Use cases for these are when we have a non-constructible or duplicate objects.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        public void ForbidConfigurationsContaining(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            var id = configurationObject.Id ?? throw new GeneratorException("Id must be set");

            if (!_forbiddenObjectsId.Add(id))
                throw new GeneratorException("The object has been already forbidden");
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