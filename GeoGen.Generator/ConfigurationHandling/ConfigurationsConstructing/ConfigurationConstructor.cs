using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
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

        private readonly IIdsFixerFactory _idsFixerFactory;

        private readonly IArgumentsContainerFactory _argumentsContainerFactory;

        public ConfigurationConstructor
        (
            ILeastConfigurationFinder leastConfigurationFinder,
            IIdsFixerFactory idsFixerFactory,
            IArgumentsContainerFactory argumentsContainerFactory
        )
        {
            _leastConfigurationFinder = leastConfigurationFinder ?? throw new ArgumentNullException(nameof(leastConfigurationFinder));
            _idsFixerFactory = idsFixerFactory ?? throw new ArgumentNullException(nameof(idsFixerFactory));
            _argumentsContainerFactory = argumentsContainerFactory ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
        }

        public static Stopwatch s_balast = new Stopwatch();
        public static Stopwatch s_leastResolver = new Stopwatch();
        public static Stopwatch s_cloningConfig = new Stopwatch();
        public static Stopwatch s_arguments = new Stopwatch();
        public static Stopwatch s_typeMap = new Stopwatch();

        public ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput)
        {
            s_balast.Start();
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
            s_balast.Stop();
            s_leastResolver.Start();
            var leastResolver = _leastConfigurationFinder.FindLeastConfiguration(currentConfiguration);

            var idsFixer = _idsFixerFactory.CreateFixer(leastResolver);
            s_leastResolver.Stop();
            s_cloningConfig.Start();
            currentConfiguration = CloneConfiguration(currentConfiguration, idsFixer);
            s_cloningConfig.Stop();
            s_arguments.Start();
            var forbiddenArguments = CreateNewArguments(idsFixer, configuration.ForbiddenArguments, newObjects);
            s_arguments.Stop();
            s_typeMap.Start();
            var typeToObjectsMap = new ConfigurationObjectsMap(currentConfiguration);
            s_typeMap.Stop();
            return new ConfigurationWrapper
            {
                Configuration = currentConfiguration,
                ForbiddenArguments = forbiddenArguments,
                ConfigurationObjectsMap = typeToObjectsMap
            };
        }

        private static Configuration CloneConfiguration(Configuration configuration, IIdsFixer idsFixer)
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

        private Dictionary<int, IArgumentsContainer> CreateNewArguments
        (
            IIdsFixer idsFixer,
            Dictionary<int, IArgumentsContainer> forbiddenArguments,
            IEnumerable<ConstructedConfigurationObject> newObjects
        )
        {
            var result = new Dictionary<int, IArgumentsContainer>();

            List<ConstructionArgument> FixArguments(IEnumerable<ConstructionArgument> arguments)
            {
                return arguments.Select(idsFixer.FixArgument).ToList();
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