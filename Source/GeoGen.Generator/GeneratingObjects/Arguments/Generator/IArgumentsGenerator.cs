using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represent a generator of all possible mutually distinct construction arguments.
    /// </summary>
    public interface IArgumentsGenerator
    {
        /// <summary>
        /// Generates a container of all possible distinct arguments that can be passed to 
        /// a given construction, using objects from a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The generated arguments.</returns>
        IEnumerable<Arguments> GenerateArguments(Configuration configuration, ConstructionWrapper construction);
    }
}