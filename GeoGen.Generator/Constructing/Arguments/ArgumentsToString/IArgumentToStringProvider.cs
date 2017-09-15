using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
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

        /// <summary>
        /// Caches a given string representation associated with the argument
        /// with an given id. We call this after converting an argument that
        /// didn't have an id while it was being converted.
        /// </summary>
        /// <param name="argumentId">The argument id.</param>
        /// <param name="stringRepresentation">The string representation.</param>
        void Cache(int argumentId, string stringRepresentation);
    }
}
