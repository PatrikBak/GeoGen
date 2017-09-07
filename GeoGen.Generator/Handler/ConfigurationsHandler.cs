using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConfigurationHandling;
using GeoGen.Generator.Constructing;

namespace GeoGen.Generator.Handler
{
    internal class ConfigurationsHandler : IConfigurationsHandler
    {
        private readonly IConfigurationContainer _configurationContainer;

        public ConfigurationsHandler(IConfigurationContainer container, IConfigurationConstructor constructor)
        {
            _configurationContainer = container;
        }

        public IEnumerable<GeneratorOutput> GenerateFinalOutput()
        {
            return _configurationContainer.Select(c => new GeneratorOutput(c.Configuration));
        }
    }
}