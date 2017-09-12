using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.ConfigurationHandling.SymetricConfigurationsHandler;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationContainer"/>.
    /// </summary>
    internal class ConfigurationContainer : StringBasedContainer<Configuration>, IConfigurationContainer
    {
        #region Private fields

        /// <summary>
        /// The arguments container factory
        /// </summary>
        private readonly IArgumentsContainerFactory _argumentsContainerFactory;

        /// <summary>
        /// The symetric configurations handler
        /// </summary>
        private readonly ISymetricConfigurationsHandler _symetricConfigurationsHandler;

        /// <summary>
        /// The configuration to string provider
        /// </summary>
        private readonly IConfigurationToStringProvider _configurationToStringProvider;

        /// <summary>
        /// The configurations container
        /// </summary>
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        #endregion

        #region IConfigurationContainer properties

        /// <summary>
        /// Gets the current layer of unprocessed configurations
        /// </summary>
        public List<ConfigurationWrapper> CurrentLayer { get; } = new List<ConfigurationWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration container with a given
        /// arguments container facory, a symetric configurations 
        /// handler, a configuration to string provider,
        /// and a configuration objects container.
        /// </summary>
        /// <param name="argumentsContainerFactory">The arguments container factory.</param>
        /// <param name="symetricConfigurationsHandler">The symetrc configurations handler.</param>
        /// <param name="configurationToStringProvider">The configuration to string provider.</param>
        /// <param name="configurationObjectsContainer">The configuration objects container.</param>
        public ConfigurationContainer(IArgumentsContainerFactory argumentsContainerFactory,
            ISymetricConfigurationsHandler symetricConfigurationsHandler,
            IConfigurationToStringProvider configurationToStringProvider,
            IConfigurationObjectsContainer configurationObjectsContainer)
        {
            _argumentsContainerFactory = argumentsContainerFactory ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
            _symetricConfigurationsHandler = symetricConfigurationsHandler ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
            _configurationToStringProvider = configurationToStringProvider ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
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

            // Initialize container with the loose objects
            _configurationObjectsContainer.Initialize(initialConfiguration.LooseObjects);

            // Add all constructed objects. 
            foreach (var constructedObject in initialConfiguration.ConstructedObjects)
            {
                // Resulting object should be the same as this one
                var result = _configurationObjectsContainer.Add(constructedObject);

                // If it's not, we have corruped data
                if (result != constructedObject)
                    throw new GeneratorException("Constructed objects contain two equal objects.");
            }

            // Let the base method add the initial configuration
            Add(initialConfiguration);

            // Create type lookup
            var objectTypeLookup = CreateTypeDictionary(initialConfiguration);

            // Create forbidden arguments dictionary
            var forbiddenArguments = CreateForbiddenArguments(initialConfiguration);

            // Create wrapper
            var configurationWrapper = new ConfigurationWrapper
            {
                Configuration = initialConfiguration,
                ObjectTypeToObjects = objectTypeLookup,
                ConstructionIdToForbiddenArguments = forbiddenArguments
            };

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

            // take the output
            var newLayer = newLayerOutput
                    // paralelize
                    .AsParallel()
                    // get new configurations
                    .Select
                    (
                        output =>
                        {
                            // Add objects to container and get identified versions
                            var newObjects = output.ConstructedObjects
                                    .Select(o => _configurationObjectsContainer.Add(o))
                                    .ToList();

                            // Re-assign the output
                            output.ConstructedObjects = newObjects;

                            // Pull the parent configuration
                            var configuration = output.InitialConfiguration;

                            // Get the symetry class representant
                            configuration = _symetricConfigurationsHandler.CreateSymetryClassRepresentant(configuration, newObjects);

                            // Add the representant to the container
                            var result = Add(configuration.Configuration);

                            // return the anonymous type wrapping the change result and the object
                            return new {Change = result, Object = configuration};
                        }
                    )
                    // take only objects that caused change of the container (i.e. new ones)
                    .Where(arg => arg.Change)
                    // take the resulting wrapper from them
                    .Select(arg => arg.Object);

            // Set the new layer (which will enumerate the query)
            CurrentLayer.SetItems(newLayer);
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
            var objectToStringProvider = _configurationObjectsContainer.ConfigurationObjectToStringProvider;

            return _configurationToStringProvider.ConvertToString(item, objectToStringProvider);
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Creates a dictionary mapping a construction id to an arguments container
        /// that contains all forbidden arguments for this construction. This is 
        /// supposed to be used for an initial configuration. At that stage we can
        /// only forbid all constructed objects that are already contained within
        /// the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary.</returns>
        private Dictionary<int, IArgumentsContainer> CreateForbiddenArguments(Configuration configuration)
        {
            var result = new Dictionary<int, IArgumentsContainer>();

            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                var id = constructedObject.Construction.Id ?? throw new GeneratorException("Construction id must be set");

                if (!result.ContainsKey(id))
                {
                    var container = _argumentsContainerFactory.CreateContainer();
                    result.Add(id, container);
                }

                result[id].AddArguments(constructedObject.PassedArguments);
            }

            return result;
        }

        /// <summary>
        /// Creates an object type to objects dictionary from a given 
        /// configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary mapping object types to all objects of that type.</returns>
        private static Dictionary<ConfigurationObjectType, List<ConfigurationObject>> CreateTypeDictionary(Configuration configuration)
        {
            var result = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>();

            var objects = configuration.LooseObjects.Cast<ConfigurationObject>().Union(configuration.ConstructedObjects);

            foreach (var configurationObject in objects)
            {
                var type = configurationObject.ObjectType;

                if (!result.ContainsKey(type))
                    result.Add(type, new List<ConfigurationObject>());

                result[type].Add(configurationObject);
            }

            return result;
        }

        #endregion
    }
}