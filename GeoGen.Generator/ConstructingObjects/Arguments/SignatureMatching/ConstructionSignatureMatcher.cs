using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// A default implementation of the <see cref="IConstructionSignatureMatcher"/> interface.
    /// </summary>
    internal class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        #region IConstructionSignatureMatcher implementation

        public List<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            if (parameters.Empty())
                throw new ArgumentException("Parameters can't be empty");

            var indices = map.ToDictionary(keyValue => keyValue.Key, keyValue => 0);

            ConfigurationObject Next(ConfigurationObjectType type)
            {
                try
                {
                    return map[type][indices[type]++];
                }
                catch (Exception)
                {
                    throw new GeneratorException("Cannot do the matching, there not too few objects.");
                }
            }

            return parameters.Select(parameter => CreateArgument(parameter, Next)).ToList();
        }

        private static ConstructionArgument CreateArgument
        (
            ConstructionParameter parameter,
            Func<ConfigurationObjectType, ConfigurationObject> nextObjectOfType
        )
        {
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                var nextObject = nextObjectOfType(objectParameter.ExpectedType);

                return new ObjectConstructionArgument(nextObject);
            }

            var setParameter = (SetConstructionParameter) parameter ?? throw new GeneratorException("Unhandled case.");

            var argumentsSet = new HashSet<ConstructionArgument>();

            for (var i = 0; i < setParameter.NumberOfParameters; i++)
            {
                // recursively call this function
                var newArgument = CreateArgument(setParameter.TypeOfParameters, nextObjectOfType);

                // update the resulting set
                argumentsSet.Add(newArgument);
            }

            return new SetConstructionArgument(argumentsSet);
        }

        #endregion
    }
}