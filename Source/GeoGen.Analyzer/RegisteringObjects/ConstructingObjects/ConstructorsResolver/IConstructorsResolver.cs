using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that is able to find the right <see cref="IObjectsConstructor"/> for a given <see cref="Construction"/>.
    /// </summary>
    public interface IConstructorsResolver
    {
        /// <summary>
        /// Find the corresponding objects constructor for a given construction.
        /// </summary>
        /// <param name="construction">The construction for which we want a constructor.</param>
        /// <returns>A constructor that performs the given construction.</returns>
        IObjectsConstructor Resolve(Construction construction);
    }
}