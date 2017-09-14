using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;

namespace GeoGen.Generator.ConfigurationHandling
{
    class ConfigurationsHandler : IConfigurationsHandler
    {
        public IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations)
        {
            return configurations.Select(configurationWrapper => new GeneratorOutput {GeneratedConfiguration = configurationWrapper.Configuration});
        }
    }
}