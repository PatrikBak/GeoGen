using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;

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

        /// <summary>
        /// The arguments list container factory.
        /// </summary>
        private readonly IArgumentsListContainerFactory _argumentsListContainerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration constructor. This constructor uses
        /// the least configuration finder to obtain the symmetry-class representant, 
        /// ids fixer factory to fix ids according to this representant, and
        /// arguments list container factory to obtain argument containers for
        /// forbidden arguments.
        /// </summary>
        /// <param name="leastConfigurationFinder">The least configuration finder.</param>
        /// <param name="idsFixerFactory">The ids fixer factory.</param>
        /// <param name="argumentsListContainerFactory">The arguments list container factory.</param>
        public ConfigurationConstructor
        (
            ILeastConfigurationFinder leastConfigurationFinder,
            IIdsFixerFactory idsFixerFactory,
            IArgumentsListContainerFactory argumentsListContainerFactory
        )
        {
            _leastConfigurationFinder = leastConfigurationFinder ?? throw new ArgumentNullException(nameof(leastConfigurationFinder));
            _idsFixerFactory = idsFixerFactory ?? throw new ArgumentNullException(nameof(idsFixerFactory));
            _argumentsListContainerFactory = argumentsListContainerFactory ?? throw new ArgumentNullException(nameof(argumentsListContainerFactory));
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
                    .Union(newObjects)
                    .ToList();

            // Create the new configuration
            var newConfiguration = new Configuration(currentConfiguration.LooseObjects, allConstructedObjects);

            // Let the resolver find it's symmetry-class representant
            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(newConfiguration);

            // Get the ids fixer for this resolver from the factory
            var idsFixer = _idsFixerFactory.CreateFixer(leastResolver);

            // Fix the configuration
            newConfiguration = FixConfiguration(newConfiguration, idsFixer);

            // Create new forbidden arguments
            var forbiddenArguments = CreateNewArguments(idsFixer, wrapper.ForbiddenArguments, newObjects);

            // Create new objects map
            var typeToObjectsMap = new ConfigurationObjectsMap(newConfiguration);

            // Return the new wrapper
            return new ConfigurationWrapper
            {
                Configuration = newConfiguration,
                ForbiddenArguments = forbiddenArguments,
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

            // Create forbidden arguments dictionary
            var forbiddenArguments = CreateForbiddenArguments(initialConfiguration);

            // Create wrapper
            var configurationWrapper = new ConfigurationWrapper
            {
                Configuration = initialConfiguration,
                ConfigurationObjectsMap = objectsMap,
                ForbiddenArguments = forbiddenArguments
            };

            // Return it
            return configurationWrapper;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a dictionary mapping a construction id to an arguments container
        /// that contains all forbidden arguments for this construction. This is 
        /// supposed to be used for an initial configuration. At that stage we can
        /// only forbid all constructed objects that are already contained within
        /// the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary.</returns>
        private Dictionary<int, IArgumentsListContainer> CreateForbiddenArguments(Configuration configuration)
        {
            // First create the resulting dictionary
            var result = new Dictionary<int, IArgumentsListContainer>();

            // Now we iterate over the constructed objects
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Pull the construction id. It must be set
                var id = constructedObject.Construction.Id ?? throw new GeneratorException("Construction id must be set");

                // If the result doesn't contain the id yet
                if (!result.ContainsKey(id))
                {
                    // Then we ask the factory for a new container
                    var container = _argumentsListContainerFactory.CreateContainer();

                    // And update the result
                    result.Add(id, container);
                }

                // And add the arguments to the result
                result[id].AddArguments(constructedObject.PassedArguments);
            }

            // And return the dictionary
            return result;
        }

        /// <summary>
        /// Fixes a given configurations and returns the fixed version.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="idsFixer">The ids fixer.</param>
        /// <returns>The fixed configuration.</returns>
        private Configuration FixConfiguration(Configuration configuration, IIdsFixer idsFixer)
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

        /// <summary>
        /// Creates a new arguments dictionary from a given one and new objects
        /// using a given ids fixer.
        /// </summary>
        /// <param name="idsFixer">The ids fixer.</param>
        /// <param name="forbiddenArguments">The current forbidden arguments.</param>
        /// <param name="newObjects">The new objects enumerable.</param>
        /// <returns></returns>
        private Dictionary<int, IArgumentsListContainer> CreateNewArguments
        (
            IIdsFixer idsFixer,
            Dictionary<int, IArgumentsListContainer> forbiddenArguments,
            IEnumerable<ConstructedConfigurationObject> newObjects
        )
        {
            // Create the dictionary that's going to be returned
            var result = new Dictionary<int, IArgumentsListContainer>();

            // A local helper function to fix an enumerable of arguments
            List<ConstructionArgument> FixArguments(IEnumerable<ConstructionArgument> arguments)
            {
                return arguments.Select(idsFixer.FixArgument).ToList();
            }

            // First we iterate over the current forbidden arguments
            foreach (var pair in forbiddenArguments)
            {
                // Pull the constructor id
                var id = pair.Key;

                // Let the factory create a new container
                var newContainer = _argumentsListContainerFactory.CreateContainer();

                // Now we iterate over the arguments in the current container
                foreach (var arguments in pair.Value)
                {
                    // Fix the current one
                    var fixedArguments = FixArguments(arguments);

                    // Add it to the new container
                    newContainer.AddArguments(fixedArguments);
                }

                // And finally update the result dictionary
                result.Add(id, newContainer);
            }

            // Now we need to resolve also the new configuration objects
            foreach (var constructedObject in newObjects)
            {
                // Pull the construction id. It must be set
                var id = constructedObject.Construction.Id ?? throw new GeneratorException("Construction must have an id.");

                // If we don't have this id in the container
                if (!result.ContainsKey(id))
                {
                    // We need to ask a factory for a new container
                    var newContainer = _argumentsListContainerFactory.CreateContainer();

                    // And add id to the result with this id
                    result.Add(id, newContainer);
                }

                // Now we fix the arguments of the constructed object
                var fixedArguments = FixArguments(constructedObject.PassedArguments);

                // And update the result
                result[id].AddArguments(fixedArguments);
            }

            // And finally return the result
            return result;
        }

        #endregion
    }
}