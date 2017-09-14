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

        public IdsFixer(IConfigurationObjectsContainer configurationObjectsContainer)
        {
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(configurationObjectsContainer));
        }

        public ConstructionArgument FixArgument(ConstructionArgument argument, DictionaryObjectIdResolver resolver)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            if (argument is ObjectConstructionArgument objectArgument)
            {
                var passedObject = objectArgument.PassedObject;
                var fixedObject = FixObject(passedObject, resolver);

                return new ObjectConstructionArgument(fixedObject);
            }

            var setArgument = argument as SetConstructionArgument ?? throw new GeneratorException("Impossible");

            var interiorArguments = setArgument.PassedArguments
                    .Select(arg => FixArgument(arg, resolver))
                    .ToSet();

            return new SetConstructionArgument(interiorArguments);
        }

        public ConfigurationObject FixObject(ConfigurationObject configurationObject, DictionaryObjectIdResolver resolver)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            if (configurationObject is LooseConfigurationObject looseObject)
            {
                var resolvedId = resolver.ResolveId(looseObject);

                return _configurationObjectsContainer[resolvedId];
            }

            var constructedObject = configurationObject as ConstructedConfigurationObject ?? throw new GeneratorException("Impossible");

            var newArguments = constructedObject.PassedArguments
                    .Select(arg => FixArgument(arg, resolver))
                    .ToList();

            var index = constructedObject.Index;
            var construction = constructedObject.Construction;

            var fixedObject = new ConstructedConfigurationObject(construction, newArguments, index);
            fixedObject = _configurationObjectsContainer.Add(fixedObject);

            return fixedObject;
        }
    }
}