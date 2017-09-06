using System.Collections.Generic;

namespace GeoGen.Generator.Handler
{
    /// <summary>
    /// Represents a handler of configurations provided by configuration constructor. 
    /// It's supposed to pass these configurations to the analyzer and handle it's 
    /// ouput. It's supposed to be a part of a single <see cref="IGeneratorContext"/>.
    /// </summary>
    internal interface IConfigurationsHandler
    {
        IEnumerable<GeneratorOutput> GenerateFinalOutput();
    }
}
