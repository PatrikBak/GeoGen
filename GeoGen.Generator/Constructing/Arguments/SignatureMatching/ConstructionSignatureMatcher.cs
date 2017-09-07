using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructing.Arguments.SignatureMatching
{
    internal class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        private IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> _objectTypeToObjects;

        private Dictionary<ConfigurationObjectType, int> _currentIndices;

        public void Initialize(IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> objectTypeToObjects)
        {
            _objectTypeToObjects = objectTypeToObjects;
            _currentIndices = objectTypeToObjects.ToDictionary(keyValue => keyValue.Key, keyValue => 0);
        }

        public IReadOnlyList<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters)
        {
            return parameters.Select(CreateArgument).ToList();
        }

        private ConstructionArgument CreateArgument(ConstructionParameter parameter)
        {
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                var nextObject = Next(objectParameter.ExpectedType);

                return new ObjectConstructionArgument(nextObject);
            }

            var setParameter = parameter as SetConstructionParameter ?? throw new NullReferenceException();

            var argumentsSet = new HashSet<ConstructionArgument>();

            for (var i = 0; i < setParameter.NumberOfParameters; i++)
            {
                // recursively call this function
                var newArgument = CreateArgument(setParameter.TypeOfParameters);

                // update the resulting set
                argumentsSet.Add(newArgument);
            }

            return new SetConstructionArgument(argumentsSet);
        }

        private ConfigurationObject Next(ConfigurationObjectType type)
        {
            return _objectTypeToObjects[type][_currentIndices[type]++];
        }
    }
}