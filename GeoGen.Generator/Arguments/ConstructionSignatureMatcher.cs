using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// A default implementation of the <see cref="IConstructionSignatureMatcher"/> interface.
    /// This sealed class is thread-safe.
    /// </summary>
    internal sealed class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        #region IConstructionSignatureMatcher methods

        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. It must be
        /// possible to perform the construction, otherwise a <see cref="GeneratorException"/> is thrown.
        /// The objects are given in a configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters list.</param>
        /// <param name="map">The configuration objects map.</param>
        /// <returns>The created arguments.</returns>
        public List<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            if (parameters.Empty())
                throw new ArgumentException("Parameters can't be empty");

            // Create a dictionary mapping object types to the current index value of that type
            // in the lists of objects of the type present in the map
            var indices = map.ToDictionary(keyValue => keyValue.Key, keyValue => 0);

            // A local function to pull the next object of a given type from the map
            ConfigurationObject Next(ConfigurationObjectType type)
            {
                try
                {
                    return map[type][indices[type]++];
                }
                catch (Exception)
                {
                    throw new GeneratorException("Cannot do the matching, there are too few objects.");
                }
            }

            // Cast the parameters list to the arguments list using the private and the local function
            return parameters.Select(parameter => CreateArgument(parameter, Next)).ToList();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a new construction argument the matches a given construction parameter,
        /// using a given selector for the next object of a given type.
        /// </summary>
        /// <param name="parameter">The construction parameter.</param>
        /// <param name="nextObjectOfType">The next object of type selector.</param>
        /// <returns></returns>
        private ConstructionArgument CreateArgument
        (
            ConstructionParameter parameter,
            Func<ConfigurationObjectType, ConfigurationObject> nextObjectOfType
        )
        {
            // If the parameter is object construction parameter
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                // Then we simply ask for the next object of the expected type
                var nextObject = nextObjectOfType(objectParameter.ExpectedType);

                // And return the object argument wrapping this object
                return new ObjectConstructionArgument(nextObject);
            }

            // Otherwise we have a set construction parameter
            var setParameter = (SetConstructionParameter) parameter;

            // Create a set of arguments that we're going to create
            var argumentsSet = new HashSet<ConstructionArgument>();

            // For the expected number of items
            for (var i = 0; i < setParameter.NumberOfParameters; i++)
            {
                // Recursively call this function to obtain a new argument
                var newArgument = CreateArgument(setParameter.TypeOfParameters, nextObjectOfType);

                // And update the resulting set
                argumentsSet.Add(newArgument);
            }

            // And finally construct and return the set construction argument
            return new SetConstructionArgument(argumentsSet);
        }

        #endregion
    }
}