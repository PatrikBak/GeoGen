using System.Collections.Generic;
using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a handler of configurations provided by configuration constructor. 
    /// It's supposed to pass these configurations to the analyzer and handle it's 
    /// output. 
    /// </summary>
    internal interface IConfigurationsHandler
    {
        /// <summary>
        /// Handles generated configuration. It is supposed to call the analyzer
        /// service and handle it's output.
        /// </summary>
        /// <param name="configurations">The configurations enumerable.</param>
        /// <returns>The final generator output enumerable.</returns>
        IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations);
    }
}