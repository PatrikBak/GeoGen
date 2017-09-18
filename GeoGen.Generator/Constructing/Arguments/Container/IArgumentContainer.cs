using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// Represents a container for <see cref="ConstructionArgument"/> objects.
    /// </summary>
    internal interface IArgumentContainer
    {
        /// <summary>
        /// Adds a given argument to a container. The argument must
        /// not have set the id, it's going to be set in the container.
        /// If an equal version of the object is present in the container, 
        /// it will return instance of this internal object. Otherwise
        /// it will return this object with set id.
        /// </summary>
        /// <param name="argument">The construction argument.</param>
        /// <returns>The identified version of the construction argument.</returns>
        ConstructionArgument AddArgument(ConstructionArgument argument);
    }
}