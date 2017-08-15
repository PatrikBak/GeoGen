using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    internal class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        private IReadOnlyDictionary<ConfigurationObjectType, IEnumerator<ConfigurationObject>> _objectTypeToObjects;

        public void Initialize(IReadOnlyDictionary<ConfigurationObjectType, IEnumerator<ConfigurationObject>> objectTypeToObjects)
        {
            _objectTypeToObjects = objectTypeToObjects;
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
            var enumerator = _objectTypeToObjects[type];

            if (!enumerator.MoveNext())
                throw new Exception("Incorrect call");

            return enumerator.Current;
        }
    }
}