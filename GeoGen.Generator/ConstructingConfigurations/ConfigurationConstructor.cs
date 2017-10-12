using System;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationConstructor"/>. This class
    /// is thread-safe (assuming that all it's dependencies are like that).
    /// </summary>
    internal class ConfigurationConstructor : IConfigurationConstructor
    {
        #region Private fields

        /// <summary>
        /// The least configuration finder.
        /// </summary>
        private readonly ILeastConfigurationFinder _leastConfigurationFinder;

        /// <summary>
        /// The ids fixer factory.
        /// </summary>
        private readonly IIdsFixerFactory _idsFixerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration constructor. This constructor uses
        /// the least configuration finder to obtain the symmetry-class representant, 
        /// ids fixer factory to fix ids according to this representant, and
        /// arguments list container factory to obtain argument containers for
        /// forbidden arguments.
        /// </summary>
        /// <param name="finder">The least configuration finder.</param>
        /// <param name="factory">The ids fixer factory.</param>
        public ConfigurationConstructor(ILeastConfigurationFinder finder, IIdsFixerFactory factory)
        {
            _leastConfigurationFinder = finder ?? throw new ArgumentNullException(nameof(finder));
            _idsFixerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region IConfigurationConstructor methods

        /// <summary>
        /// Constructs a configuration wrapper from a given constructor output.
        /// </summary>
        /// <param name="constructorOutput">The constructor output.</param>
        /// <returns>The wrapper of the new configuration.</returns>
        public ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput)
        {
            if (constructorOutput == null)
                throw new ArgumentNullException(nameof(constructorOutput));

            // Pull origin configuration wrapper
            var wrapper = constructorOutput.InitialConfiguration;

            // Pull new objects
            var newObjects = constructorOutput.ConstructedObjects;

            // Pull original configuration
            var currentConfiguration = wrapper.Configuration;

            // Merge original constructed objects with the new ones
            var allConstructedObjects = currentConfiguration
                    .ConstructedObjects
                    .Concat(newObjects)
                    .ToList();


            // Create the new configuration
            var newConfiguration = new Configuration(currentConfiguration.LooseObjects, allConstructedObjects);

            // Let the resolver find it's symmetry-class representant
            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(newConfiguration);

            // Get the ids fixer for this resolver from the factory
            var idsFixer = _idsFixerFactory.CreateFixer(leastResolver);

            // Fix the configuration
            newConfiguration = FixConfiguration(newConfiguration, idsFixer);

            // Create new objects map
            var typeToObjectsMap = new ConfigurationObjectsMap(newConfiguration);

            // Return the new wrapper
            return new ConfigurationWrapper
            {
                Configuration = newConfiguration,
                ConfigurationObjectsMap = typeToObjectsMap
            };
        }

        /// <summary>
        /// Constructs a configuration wrapper from a given configuration (meant
        /// to be as initial).
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <returns>The wrapper of the initial configuration.</returns>
        public ConfigurationWrapper ConstructWrapper(Configuration initialConfiguration)
        {
            // Create type object map
            var objectsMap = new ConfigurationObjectsMap(initialConfiguration);

            // Create wrapper
            var configurationWrapper = new ConfigurationWrapper
            {
                Configuration = initialConfiguration,
                ConfigurationObjectsMap = objectsMap
            };

            // Return it
            return configurationWrapper;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Fixes a given configurations and returns the fixed version.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="idsFixer">The ids fixer.</param>
        /// <returns>The fixed configuration.</returns>
        private static Configuration FixConfiguration(Configuration configuration, IIdsFixer idsFixer)
        {
            // Loose objects are going to be still the same
            var looseObjects = configuration.LooseObjects;

            // We let the fixer fix the constructed objects
            var constructedObjects = configuration.ConstructedObjects
                    .Select(idsFixer.FixObject)
                    .Cast<ConstructedConfigurationObject>()
                    .ToList();

            // And return the result
            return new Configuration(looseObjects, constructedObjects);
        }

        #endregion
    }
}