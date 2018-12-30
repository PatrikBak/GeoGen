using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Utilities.TestHelpers
{
    /// <summary>
    /// Static helpers for dealing with <see cref="ConfigurationObject"/>s. These methods
    /// are intended to be used for debugging purposes and testing.
    /// </summary>
    public static class ConfigurationObjects
    {
        /// <summary>
        /// Performs a given predefined construction on a given input. 
        /// This method does not safety check.
        /// </summary>
        /// <param name="type">The type of the predefined construction to be performed.</param>
        /// <param name="input">The input objects in the flattened order (see <see cref="Arguments.FlattenedList")/></param>
        /// <returns>The constructed object.</returns>
        public static ConstructedConfigurationObject Construct(PredefinedConstructionType type, params ConfigurationObject[] input)
        {
            // Get the construction
            var construction = PredefinedConstructionsFactory.Get(type);

            // Create the matching arguments
            var arguments = construction.Signature.Match(new ConfigurationObjectsMap(input));

            // Create the result
            return new ConstructedConfigurationObject(construction, arguments);
        }
    }
}