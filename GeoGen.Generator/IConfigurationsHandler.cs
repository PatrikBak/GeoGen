using System.Collections.Generic;
using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    internal interface IConfigurationsHandler
    {
        IEnumerable<GeneratorOutput> GenerateFinalOutput();
    }
}
