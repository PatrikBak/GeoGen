using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that is able to find the right <see cref="IObjectConstructor"/> for a given <see cref="Construction"/>.
    /// </summary>
    public interface IConstructorsResolver
    {
        /// <summary>
        /// Find the corresponding object constructor for a given construction.
        /// </summary>
        /// <param name="construction">The construction for which we want a constructor.</param>
        /// <returns>A constructor that performs the given construction.</returns>
        IObjectConstructor Resolve(Construction construction);
    }
}