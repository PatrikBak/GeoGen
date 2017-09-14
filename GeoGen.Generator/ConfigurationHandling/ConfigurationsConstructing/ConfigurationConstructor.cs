using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing
{
    internal class ConfigurationConstructor : IConfigurationConstructor
    {
        private readonly ILeastConfigurationFinder _leastConfigurationFinder;

        private readonly IIdsFixer _idsFixer;

        private readonly IArgumentsContainerFactory _argumentsContainerFactory;

        public ConfigurationConstructor
        (
            ILeastConfigurationFinder leastConfigurationFinder,
            IIdsFixer idsFixer,
            IArgumentsContainerFactory argumentsContainerFactory
        )
        {
            _leastConfigurationFinder = leastConfigurationFinder ?? throw new ArgumentNullException(nameof(leastConfigurationFinder));
            _idsFixer = idsFixer ?? throw new ArgumentNullException(nameof(idsFixer));
            _argumentsContainerFactory = argumentsContainerFactory ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
        }

        public ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput)
        {
            if (constructorOutput == null)
                throw new ArgumentNullException(nameof(constructorOutput));

            var configuration = constructorOutput.InitialConfiguration;

            var newObjects = constructorOutput.ConstructedObjects;

            var looseObjects = configuration.Configuration.LooseObjects;

            var allConstructedObjects = configuration.Configuration
                    .ConstructedObjects
                    .Union(newObjects)
                    .ToList();

            var currentConfiguration = new Configuration(looseObjects, allConstructedObjects);

            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(currentConfiguration);

            currentConfiguration = CloneConfiguration(currentConfiguration, leastResolver);

            var forbiddenArguments = CreateNewArguments(leastResolver, configuration.ForbiddenArguments, newObjects);

            var typeToObjectsMap = configuration.ConfigurationObjectsMap.CloneWithNewObjects(newObjects);

            return new ConfigurationWrapper
            {
                Configuration = currentConfiguration,
                ForbiddenArguments = forbiddenArguments,
                ConfigurationObjectsMap = typeToObjectsMap
            };
        }

        private Configuration CloneConfiguration(Configuration configuration, DictionaryObjectIdResolver resolver)
        {
            // Loose objects are going to be still the same
            var looseObjects = configuration.LooseObjects;

            // We let the fixer fix the constructed objects
            var constructedObjects = configuration.ConstructedObjects
                    .Select(obj => _idsFixer.FixObject(obj, resolver))
                    .Cast<ConstructedConfigurationObject>()
                    .ToList();

            // And return the result
            return new Configuration(looseObjects, constructedObjects);
        }

        private Dictionary<int, IArgumentsContainer> CreateNewArguments
        (
            DictionaryObjectIdResolver resolver,
            Dictionary<int, IArgumentsContainer> forbiddenArguments,
            IEnumerable<ConstructedConfigurationObject> newObjects
        )
        {
            var result = new Dictionary<int, IArgumentsContainer>();

            List<ConstructionArgument> FixArguments(IEnumerable<ConstructionArgument> arguments)
            {
                return arguments.Select(arg => _idsFixer.FixArgument(arg, resolver)).ToList();
            }

            foreach (var pair in forbiddenArguments)
            {
                var id = pair.Key;
                var newContainer = _argumentsContainerFactory.CreateContainer();

                foreach (var arguments in pair.Value)
                {
                    var fixedArguments = FixArguments(arguments);
                    newContainer.AddArguments(fixedArguments);
                }

                result.Add(id, newContainer);
            }

            foreach (var constructedObject in newObjects)
            {
                var id = constructedObject.Construction.Id ?? throw new GeneratorException("Contruction must have an id.");

                if (!result.ContainsKey(id))
                {
                    var newContainer = _argumentsContainerFactory.CreateContainer();
                    result.Add(id, newContainer);
                }

                var fixedArguments = FixArguments(constructedObject.PassedArguments);
                result[id].AddArguments(fixedArguments);
            }

            return result;
        }
    }
}