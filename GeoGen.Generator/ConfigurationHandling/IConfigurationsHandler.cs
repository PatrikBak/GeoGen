using System.Collections.Generic;

namespace GeoGen.Generator.ConfigurationHandling
{
    /// <summary>
    /// Represents a handler of configurations provided by configuration constructor. 
    /// It's supposed to pass these configurations to the analyzer and handle it's 
    /// output. 
    /// </summary>
    internal interface IConfigurationsHandler
    {
        /// <summary>
        /// Handles the current state of a given configuration container.
        /// </summary>
        /// <param name="configurationContainer">The configuration container</param>
        /// <returns></returns>
        IEnumerable<GeneratorOutput> GenerateFinalOutput(IConfigurationContainer configurationContainer);
    }
}
