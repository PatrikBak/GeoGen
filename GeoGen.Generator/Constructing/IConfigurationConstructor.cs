using System.Collections.Generic;
using GeoGen.Generator.ConfigurationHandling;

namespace GeoGen.Generator.Constructing
{
    /// <summary>
    /// Represents an interface that performs construction operations to obtian new configurations
    /// from a given one. 
    /// </summary>
    internal interface IConfigurationConstructor
    {
        /// <summary>
        /// Performs all possible constructions to a given configution wrapper.
        /// </summary>
        /// <param name="configuration">The given configuration wrapper.</param>
        /// <returns>The enumerable of constructor output.</returns>
        IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(ConfigurationWrapper configuration);
    }
}