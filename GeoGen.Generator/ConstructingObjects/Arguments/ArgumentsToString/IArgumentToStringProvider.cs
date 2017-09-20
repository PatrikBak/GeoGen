using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString
{
    /// <summary>
    /// Represents a construction argument to string converter. 
    /// </summary>
    internal interface IArgumentToStringProvider
    {
        /// <summary>
        /// Converts a given argument to string.
        /// </summary>
        /// <param name="argument">The construction argument.</param>
        /// <returns>The string represtantation of the argument.</returns>
        string ConvertArgument(ConstructionArgument argument);
    }
}