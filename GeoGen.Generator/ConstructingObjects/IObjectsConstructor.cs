using System.Collections.Generic;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// Represents a service that performs the construction operations to obtian new configurations
    /// from a given one. 
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Performs all possible constructions to a given configution wrapper.
        /// </summary>
        /// <param name="configurationWrapper">The given configuration wrapper.</param>
        /// <returns>The enumerable of constructor output.</returns>
        IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper);
    }
}