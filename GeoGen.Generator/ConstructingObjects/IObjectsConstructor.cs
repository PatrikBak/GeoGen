using System.Collections.Generic;
using GeoGen.Generator.ConstructingConfigurations;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// Represents a service that performs the construction operations 
    /// to obtain new configurations from a given one. 
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Performs all possible constructions to a given configuration wrapper.
        /// </summary>
        /// <param name="configurationWrapper">The configuration wrapper.</param>
        /// <returns>The enumerable of constructor outputs.</returns>
        IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper);
    }
}