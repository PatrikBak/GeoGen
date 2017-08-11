using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    internal interface IConfigurationConstructer
    {
        IEnumerable<Configuration> GenerateNewConfigurations(Configuration configuration);
    }
}