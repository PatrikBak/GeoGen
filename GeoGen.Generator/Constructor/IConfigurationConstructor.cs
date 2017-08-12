using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor
{
    /// <summary>
    /// Represents an interface that performs construction operations to obtian new configurations
    /// from a given one. It's supposed to be a part of a single <see cref="IGeneratorContext"/>.
    /// </summary>
    internal interface IConfigurationConstructor
    {
        /// <summary>
        /// Constructs new configurations from a given one by performing the constructions.
        /// </summary>
        /// <param name="configuration">The given configuration to be extended.</param>
        /// <returns></returns>
        IEnumerable<Configuration> GenerateNewConfigurations(Configuration configuration);
    }
}