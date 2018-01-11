using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a generic arguments list to string converter that uses 
    /// a custom <see cref="IObjectToStringConverter"/> converter. 
    /// </summary>
    internal interface IArgumentsListToStringProvider
    {
        /// <summary>
        /// Converts a given list of construction arguments to string, using
        /// a given configuration object to string provider.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation of the list.</returns>
        string ConvertToString(IReadOnlyList<ConstructionArgument> arguments, IObjectToStringConverter objectToString);
    }
}