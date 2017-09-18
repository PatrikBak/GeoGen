using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// Represents a construction arguments list to string converter.
    /// </summary>
    internal interface IArgumentsListToStringProvider
    {
        /// <summary>
        /// Converts a given list of construction arguments to string,
        /// using a default configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        string ConvertToString(IReadOnlyList<ConstructionArgument> arguments);

        /// <summary>
        /// Converts a given list of construction arguments to string, 
        /// using  a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the list.</returns>
        string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringProvider objectToString);
    }
}