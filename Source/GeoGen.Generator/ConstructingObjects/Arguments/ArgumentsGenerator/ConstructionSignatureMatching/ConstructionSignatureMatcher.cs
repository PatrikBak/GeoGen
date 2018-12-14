using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConstructionSignatureMatcher"/>.
    /// </summary>
    internal class ConstructionSignatureMatcher : IConstructionSignatureMatcher
    {
        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. 
        /// The objects that are actually passed to as the arguments are given in a 
        /// configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters list.</param>
        /// <param name="map">The configuration objects map.</param>
        /// <returns>The created arguments.</returns>
        public Arguments Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map)
        {
            // Create a dictionary mapping object types to the current index of the object of that type
            // that will be passed to the next arguments. Initially, these indices are 0.
            var indices = map.ToDictionary(keyValue => keyValue.Key, keyValue => 0);

            // A local function to pull the next object of a given type from the map
            ConfigurationObject Next(ConfigurationObjectType type) => map[type][indices[type]++];

            // Cast each parameter to the argument using the private function and enumerate them to a list
            var argumentsList = parameters.Select(parameter => CreateArgument(parameter, Next)).ToList();

            // Construct the output
            return new Arguments(argumentsList);
        }

        /// <summary>
        /// Creates a new construction argument that matches a given construction parameter.
        /// </summary>
        /// <param name="parameter">The construction parameter.</param>
        /// <param name="nextObjectOfType">The selector for the next object of a given type.</param>
        /// <returns>The construction argument.</returns>
        private ConstructionArgument CreateArgument(ConstructionParameter parameter, Func<ConfigurationObjectType, ConfigurationObject> nextObjectOfType)
        {
            // If the parameter is an object construction parameter...
            if (parameter is ObjectConstructionParameter objectParameter)
            {
                // Then we simply ask for the next object of the expected type
                var nextObject = nextObjectOfType(objectParameter.ExpectedType);

                // And return the object argument wrapping this object
                return new ObjectConstructionArgument(nextObject);
            }

            // Otherwise we have a set construction parameter
            var setParameter = (SetConstructionParameter) parameter;

            // Create arguments list that we're going to fill
            var arguments = new List<ConstructionArgument>();

            // For the expected number of times...
            GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () =>
            {
                // Recursively call this function to obtain a new argument
                var newArgument = CreateArgument(setParameter.TypeOfParameters, nextObjectOfType);

                // And update the current list
                arguments.Add(newArgument);
            });

            // Finally construct and return the set construction argument
            return new SetConstructionArgument(arguments);
        }
    }
}