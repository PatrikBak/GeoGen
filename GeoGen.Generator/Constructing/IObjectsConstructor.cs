using System.Collections.Generic;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;

namespace GeoGen.Generator.Constructing
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
        IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(ConfigurationWrapper configurationWrapper);
    }
}