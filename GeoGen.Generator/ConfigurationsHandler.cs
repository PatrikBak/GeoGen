using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConstructingConfigurations;

namespace GeoGen.Generator
{
    internal class ConfigurationsHandler : IConfigurationsHandler
    {
        public IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations)
        {
            return configurations.Select(configurationWrapper => new GeneratorOutput {GeneratedConfiguration = configurationWrapper.Configuration});
        }
    }
}