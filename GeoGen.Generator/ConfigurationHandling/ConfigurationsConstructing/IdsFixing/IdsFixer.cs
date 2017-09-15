using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing
{
    internal class IdsFixer : IIdsFixer
    {
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        private readonly DictionaryObjectIdResolver _resolver;

        private readonly Dictionary<int, ConstructedConfigurationObject> _cache;

        public IdsFixer(IConfigurationObjectsContainer configurationObjectsContainer, DictionaryObjectIdResolver resolver)
        {
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(configurationObjectsContainer));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _cache = new Dictionary<int, ConstructedConfigurationObject>();
        }

        public ConstructionArgument FixArgument(ConstructionArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (_resolver == null)
                throw new ArgumentNullException(nameof(_resolver));

            if (argument is ObjectConstructionArgument objectArgument)
            {
                var passedObject = objectArgument.PassedObject;
                var fixedObject = FixObject(passedObject);

                return new ObjectConstructionArgument(fixedObject);
            }

            var setArgument = argument as SetConstructionArgument ?? throw new GeneratorException("Impossible");

            var interiorArguments = setArgument.PassedArguments
                    .Select(FixArgument)
                    .ToSet();

            return new SetConstructionArgument(interiorArguments);
        }

        public ConfigurationObject FixObject(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (_resolver == null)
                throw new ArgumentNullException(nameof(_resolver));

            if (configurationObject is LooseConfigurationObject looseObject)
            {
                var resolvedId = _resolver.ResolveId(looseObject);

                return _configurationObjectsContainer[resolvedId];
            }

            var id = configurationObject.Id ?? throw new GeneratorException("Impossible");

            if (_cache.ContainsKey(id))
                return _cache[id];

            var constructedObject = configurationObject as ConstructedConfigurationObject ?? throw new GeneratorException("Impossible");

            var newArguments = constructedObject.PassedArguments
                    .Select(FixArgument)
                    .ToList();

            var index = constructedObject.Index;
            var construction = constructedObject.Construction;

            var fixedObject = new ConstructedConfigurationObject(construction, newArguments, index);
            fixedObject = _configurationObjectsContainer.Add(fixedObject);
            _cache.Add(configurationObject.Id ?? throw new GeneratorException("Impossible"), fixedObject);

            return fixedObject;
        }
    }
}