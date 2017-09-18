using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;

namespace GeoGen.Generator.ConfigurationsHandling
{
    internal class ConfigurationsHandler : IConfigurationsHandler
    {
        public IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations)
        {
            return configurations.Select(configurationWrapper => new GeneratorOutput {GeneratedConfiguration = configurationWrapper.Configuration});
        }
    }
}