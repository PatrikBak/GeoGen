using GeoGen.Generator.Constructor;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;

namespace GeoGen.Generator
{
    internal class GeneratorContext : IGeneratorContext
    {
        public IConfigurationContainer ConfigurationContainer { get; }
        public IConfigurationsHandler ConfigurationsHandler { get; }
        public IConfigurationConstructor ConfigurationConstructor { get; }

        public GeneratorContext(IConfigurationContainer configurationContainer, IConfigurationsHandler configurationsHandler,
            IConfigurationConstructor configurationConstructor)
        {
            ConfigurationContainer = configurationContainer;
            ConfigurationsHandler = configurationsHandler;
            ConfigurationConstructor = configurationConstructor;
        }
    }
}