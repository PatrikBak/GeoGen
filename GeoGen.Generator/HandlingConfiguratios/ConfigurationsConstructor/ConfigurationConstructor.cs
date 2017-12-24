using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationConstructor"/>. This sealed class
    /// is thread-safe (assuming that all it's dependencies are like that).
    /// </summary>
    internal sealed class ConfigurationConstructor : IConfigurationConstructor
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

        /// <summary>
        /// The default object id resolver.
        /// </summary>
        private readonly IDefaultObjectIdResolver _resolver;

        /// <summary>
        /// The last configuration id.
        /// </summary>
        private int _lastId;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration constructor. This constructor uses
        /// the least configuration finder to obtain the symmetry-sealed class representant, 
        /// ids fixer factory to fix ids according to this representant, and
        /// arguments list container factory to obtain argument containers for
        /// forbidden arguments.
        /// TODO
        /// </summary>
        /// <param name="finder">The least configuration finder.</param>
        /// <param name="factory">The ids fixer factory.</param>
        public ConfigurationConstructor
        (
                ILeastConfigurationFinder finder,
                IIdsFixerFactory factory,
                IDefaultObjectIdResolver resolver
        )
        {
            _leastConfigurationFinder = finder ?? throw new ArgumentNullException(nameof(finder));
            _idsFixerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resolver = resolver;
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

            var initialWrapper = CreateWrapperFromOutput(constructorOutput);

            //Check(initialWrapper);

            // Let the resolver find it's symmetry class representant
            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(initialWrapper);

            var finalWrapper = CreateFinalWrapper(initialWrapper, leastResolver);

            //Check(finalWrapper);

            return finalWrapper;
        }

        private ConfigurationWrapper CreateWrapperFromOutput(ConstructorOutput constructorOutput)
        {
            // Pull initial configuration wrapper
            var originalConfigurationWrapper = constructorOutput.OriginalConfiguration;

            // Pull original configuration
            var originalConfiguration = originalConfigurationWrapper.Configuration;

            // Construct original objects list
            var originalObjects = originalConfigurationWrapper.AllObjectsMap.AllObjects().ToList();

            // Merge original constructed objects with the new ones
            var allConstructedObjects = originalConfiguration
                    .ConstructedObjects
                    .Concat(constructorOutput.ConstructedObjects)
                    .ToList();

            // Create the new configuration
            var newConfiguration = new Configuration(originalConfiguration.LooseObjects, allConstructedObjects);

            // Create the new wrapper for the resolver
            return new ConfigurationWrapper
            {
                    Id = _lastId++,
                    Configuration = newConfiguration,
                    AllObjectsMap = new ConfigurationObjectsMap(newConfiguration),
                    PreviousConfiguration = originalConfigurationWrapper,
                    OriginalObjects = originalObjects,
                    LastAddedObjects = constructorOutput.ConstructedObjects,
                    ResolverToMinimalForm = _resolver,
                    Excluded = false
            };
        }

        private ConfigurationWrapper CreateFinalWrapper(ConfigurationWrapper initialWrapper, DictionaryObjectIdResolver resolver)
        {
            // Get the ids fixer for this resolver from the factory
            var idsFixer = _idsFixerFactory.CreateFixer(resolver);

            // Fix the configuration
            var newConfiguration = FixConfiguration(initialWrapper.Configuration, idsFixer);

            // Find new objects
            var newObjects = FindNewObjects(newConfiguration, initialWrapper.LastAddedObjects.Count);

            // Create new objects map
            var typeToObjectsMap = new ConfigurationObjectsMap(newConfiguration);

            // Create original objects
            var originalObjects = typeToObjectsMap.AllObjects().Where(o => !newObjects.Contains(o)).ToList();

            // Return the new wrapper
            return new ConfigurationWrapper
            {
                    Id = initialWrapper.Id,
                    Configuration = newConfiguration,
                    ResolverToMinimalForm = resolver,
                    PreviousConfiguration = initialWrapper.PreviousConfiguration,
                    AllObjectsMap = typeToObjectsMap,
                    LastAddedObjects = newObjects,
                    OriginalObjects = originalObjects,
                    Excluded = false
            };
        }

        private void Check(ConfigurationWrapper wrapper)
        {
            // All objects = Last added + Original
            var allObjectsSet = wrapper.AllObjectsMap.AllObjects().ToSet();

            var theoreticalAllObjectss = wrapper.OriginalObjects.Concat(wrapper.LastAddedObjects).ToSet();

            if (!allObjectsSet.SetEquals(theoreticalAllObjectss))
            {
                Console.WriteLine();
            }

            // All objects = All objects of configuration
            var allObjects2 = wrapper
                    .Configuration
                    .ConstructedObjects
                    .Cast<ConfigurationObject>()
                    .Concat(wrapper.Configuration.LooseObjects)
                    .ToSet();

            if (!allObjectsSet.SetEquals(allObjects2))
            {
                Console.WriteLine();
            }

            // Original objects = All objects from previous configuration
            var allFromPrevious = wrapper.PreviousConfiguration == null
                    ? new HashSet<ConfigurationObject>()
                    : new ConfigurationObjectsMap(wrapper.PreviousConfiguration.AllObjectsMap).AllObjects().ToSet();

            if (!wrapper.OriginalObjects.ToSet().SetEquals(allFromPrevious))
            {
                Console.WriteLine();
            }
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
                    Id = _lastId++,
                    Configuration = initialConfiguration,
                    AllObjectsMap = objectsMap,
                    LastAddedObjects = new List<ConstructedConfigurationObject>(),
                    OriginalObjects = objectsMap.AllObjects().ToList(),
                    Excluded = false,
                    PreviousConfiguration = null,
                    ResolverToMinimalForm = _resolver
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

        private static List<ConstructedConfigurationObject> FindNewObjects(Configuration newConfiguration, int newObjectsCount)
        {
            var allConstructed = newConfiguration.ConstructedObjects;

            var list = allConstructed.Skip(allConstructed.Count - newObjectsCount).ToList();

            return list;
        }

        #endregion
    }
}