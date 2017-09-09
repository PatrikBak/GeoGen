using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// Represents a construction argument list to string converter.
    /// </summary>
    public interface IArgumentToStringProvider
    {
        /// <summary>
        /// Converts a given list of construction arguments to string. 
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The string representation of the list.</returns>
        string ConvertToString(IReadOnlyList<ConstructionArgument> arguments);
    }
}
