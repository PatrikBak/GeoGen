using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer.Constructing
{
    /// <summary>
    /// Represents a service that is able to find the right
    /// <see cref="IObjectsConstructor"/> for a given construction.
    /// </summary>
    internal interface IConstructorsResolver
    {
        /// <summary>
        /// Find the corresponding <see cref="IObjectsConstructor"/> for 
        /// a given <see cref="Construction"/>.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The constructor.</returns>
        IObjectsConstructor Resolve(Construction construction);
    }
}