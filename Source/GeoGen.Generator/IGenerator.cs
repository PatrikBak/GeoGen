using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that performs the generator algorithm.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Performs the generation algorithm.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <param name="objectFilter">The filter applied to generated constructed objects.</param>
        /// <param name="configurationFilter">The filter applied to generated configuration.</param>
        /// <returns>The generated configurations.</returns>
        IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input, Predicate<ConstructedConfigurationObject> objectFilter, Predicate<GeneratedConfiguration> configurationFilter);
    }
}