using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that applies all constructions to a given
    /// configuration in all possible ways to create new configuration objects.
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Performs all possible constructions to a given configuration.
        /// </summary>
        /// <param name="configurationWrapper">The configuration wrapper.</param>
        /// <returns>The enumerable of constructor outputs.</returns>
        IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper);
    }
}