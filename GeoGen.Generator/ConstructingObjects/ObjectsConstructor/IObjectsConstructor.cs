using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that generates all possible <see cref="ConstructorOutput"/>s
    /// for a given configuration.
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Performs all possible constructions on a given configuration.
        /// </summary>
        /// <param name="configurationWrapper">The configuration wrapper.</param>
        /// <returns>The enumerable of constructor outputs.</returns>
        IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper);
    }
}