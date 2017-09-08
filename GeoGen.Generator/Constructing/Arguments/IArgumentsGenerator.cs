using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling;

namespace GeoGen.Generator.Constructing.Arguments
{
    /// <summary>
    /// Represent a generator of all possible mutally distinct construction arguments, for
    /// given <see cref="ConfigurationWrapper"/> and <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IArgumentsGenerator
    {
        /// <summary>
        /// Generates all possible distinct arguments that can be passed to 
        /// a given construction, using object from a given configuration.
        /// </summary>
        /// <param name="configuration">The wrapper cofiguration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The enumerable of generated construction arguments.</returns>
        IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}