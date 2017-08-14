using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        public IReadOnlyList<ConstructionArgument> Match(IConfigurationObjectsIterator iterator, IReadOnlyList<ConstructionParameter> parameters)
        {
            return parameters.Select(parameter => CreateArgument(parameter, iterator)).ToList();
        }

        private static ConstructionArgument CreateArgument(ConstructionParameter parameter, IConfigurationObjectsIterator iterator)
        {
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                var nextObject = iterator.Next(objectParameter.ExpectedType);

                return new ObjectConstructionArgument(nextObject);
            }

            var setParameter = parameter as SetConstructionParameter ?? throw new NullReferenceException();

            var argumentsSet = new HashSet<ConstructionArgument>();

            for (var i = 0; i < setParameter.NumberOfParameters; i++)
            {
                // recursively call this function
                var newArgument = CreateArgument(setParameter.TypeOfParameters, iterator);

                // update the resulting set
                argumentsSet.Add(newArgument);
            }

            return new SetConstructionArgument(argumentsSet);
        }
    }
}