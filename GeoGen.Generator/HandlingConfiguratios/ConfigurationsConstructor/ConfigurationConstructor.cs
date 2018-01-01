using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

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
        /// The default object id resolver.
        /// </summary>
        private readonly IDefaultObjectIdResolver _defaultResolver;

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
        public ConfigurationConstructor(ILeastConfigurationFinder finder, IDefaultObjectIdResolver defaultResolver)
        {
            _leastConfigurationFinder = finder ?? throw new ArgumentNullException(nameof(finder));
            _defaultResolver = defaultResolver;
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

            var wrapper = CreateWrapperFromOutput(constructorOutput);

            // Let the resolver find it's symmetry class representant
            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(wrapper);

            wrapper.ResolverToMinimalForm = leastResolver;

            return wrapper;
        }

        private ConfigurationWrapper CreateWrapperFromOutput(ConstructorOutput constructorOutput)
        {
            // Pull original configuration
            var originalConfiguration = constructorOutput.OriginalConfiguration.Configuration;

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
                PreviousConfiguration = constructorOutput.OriginalConfiguration,
                OriginalObjects = constructorOutput.OriginalConfiguration.Configuration.ObjectsMap.AllObjects,
                LastAddedObjects = constructorOutput.ConstructedObjects,
                ResolverToMinimalForm = _defaultResolver,
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
                Id = _lastId++,
                Configuration = initialConfiguration,
                LastAddedObjects = new List<ConstructedConfigurationObject>(),
                OriginalObjects = objectsMap.AllObjects,
                PreviousConfiguration = null,
                ResolverToMinimalForm = _defaultResolver
            };

            // Return it
            return configurationWrapper;
        }

        #endregion
    }
}