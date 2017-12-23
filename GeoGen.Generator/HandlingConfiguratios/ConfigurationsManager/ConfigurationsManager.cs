using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsManager"/>.
    /// </summary>
    internal sealed class ConfigurationsManager : IConfigurationsManager
    {
        #region Private fields

        /// <summary>
        /// The configuration constructor.
        /// </summary>
        private readonly IConfigurationConstructor _configurationConstructor;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

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
        /// Constructs a configurations manager that is initialized with a given
        /// configuration. It takes care of processing the constructor output.
        /// It uses the configuration objects container for adding new objects,
        /// configuration constructor for constructing configuration wrappers
        /// and configurations container for recognizing equal configurations.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <param name="configurationConstructor">The configuration constructor.</param>
        /// <param name="configurationObjectsContainer">The configuration objects container.</param>
        /// <param name="configurationsContainer">The configurations container.</param>
        public ConfigurationsManager
        (
                Configuration initialConfiguration,
                IConfigurationConstructor configurationConstructor,
                IConfigurationObjectsContainer configurationObjectsContainer,
                IConfigurationsContainer configurationsContainer
        )
        {
            _configurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(configurationObjectsContainer));
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _forbiddenObjectsId = new HashSet<int>();
            Initialize(initialConfiguration);
        }

        #endregion

        #region IConfigurationContainer methods

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

            // Iterate over all constructor outputs
            foreach (var currentOutput in output)
            {
                // Let the helper method construct new objects and find out if some 
                // of them aren't forbidden or duplicate
                var newObjects = ConstructNewObjects(currentOutput, out var skipConfiguration);

                // If we should skip the configuration, skip it
                if (skipConfiguration)
                    continue;

                // Otherwise re-assign the output
                currentOutput.ConstructedObjects = newObjects;

                // Let the constructor create a new wrapper
                var configuration = _configurationConstructor.ConstructWrapper(currentOutput);

                // We need to check again if the objects ids aren't forbidden, cause
                // they might have changed
                var notForbidden = configuration
                        .LastAddedObjects
                        .Select(o => o.Id ?? throw new GeneratorException("Id must be set"))
                        .All(id => !_forbiddenObjectsId.Contains(id));

                // If there is a new forbidden object
                if (!notForbidden)
                {
                    // Skip it
                    continue;
                }

                // If everything's fine, then we can add the configuration to the container.
                if (!_configurationsContainer.Add(configuration))
                {
                    // If the content hasn't changed (i.e. the configuration is already present), we skip it
                    continue;
                }

                // Otherwise we have a new configuration. We add it to the items list
                items.Add(configuration);

                // And yield it
                yield return configuration;
            }

            // Update the current layer
            CurrentLayer.SetItems(items);

            // Clear the container (the new iteration should yield different configurations)
            _configurationsContainer.Clear();
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

            var id = configurationObject.Id ?? throw GeneratorException.ObjectsIdNotSet();

            if (!_forbiddenObjectsId.Add(id))
                throw new GeneratorException("The object has been already forbidden");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the container with a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        private void Initialize(Configuration initialConfiguration)
        {
            if (initialConfiguration == null)
                throw new ArgumentNullException(nameof(initialConfiguration));

            // Let the constructor construct wrapper for the initial configuration.
            var configurationWrapper = _configurationConstructor.ConstructWrapper(initialConfiguration);

            // Set the current layer items
            CurrentLayer.SetItems(configurationWrapper.SingleItemAsEnumerable());
        }

        /// <summary>
        /// Constructs new objects from a given output and determines if we
        /// correct objects (i.e. not duplicate or forbidden ones). It might 
        /// happen that some object from the output won't be added to the
        /// container at all. 
        /// </summary>
        /// <param name="output">The constructor output.</param>
        /// <param name="correctObjects">The correct objects flag.</param>
        /// <returns>The list of new constructed objects.</returns>
        private List<ConstructedConfigurationObject> ConstructNewObjects(ConstructorOutput output, out bool correctObjects)
        {
            // Initialize result
            var result = new List<ConstructedConfigurationObject>();

            // Pull ids of initial objects and cast them to set
            var initialIds = output
                    .InitialConfiguration
                    .AllObjectsMap
                    .AllObjects()
                    .Select(obj => obj.Id ?? throw GeneratorException.ObjectsIdNotSet())
                    .ToSet();

            // Iterate over all constructed objects
            foreach (var constructedObject in output.ConstructedObjects)
            {
                // Get the result from the container
                var containerResult = _configurationObjectsContainer.Add(constructedObject);

                // If the object is currently in the configuration
                if (initialIds.Contains(containerResult.Id ?? throw GeneratorException.ObjectsIdNotSet()))
                {
                    // Set the flag indicating that there is an incorrect object
                    correctObjects = true;

                    // Terminate
                    return null;
                }

                // Pull id
                var id = containerResult.Id ?? throw GeneratorException.ObjectsIdNotSet();

                // If the object with this id is forbidden
                if (_forbiddenObjectsId.Contains(id))
                {
                    // Set the flag indicating that there is an incorrect object
                    correctObjects = true;

                    // Terminate
                    return null;
                }

                // Otherwise we add the result to the new objects
                result.Add(containerResult);
            }

            // If we got here, then we have only correct objects
            correctObjects = false;

            // Therefore we can return the objects
            return result;
        }

        #endregion
    }
}