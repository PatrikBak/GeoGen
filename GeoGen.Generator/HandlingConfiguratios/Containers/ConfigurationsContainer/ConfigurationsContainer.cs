using GeoGen.Utilities;

namespace GeoGen.Generator
{
    internal class ConfigurationsContainer : StringBasedContainer<ConfigurationWrapper>, IConfigurationsContainer
    {
        public ConfigurationsContainer(IDefaultConfigurationToStringConverter converter)
                : base(converter)
        {
        }

        public new bool Add(ConfigurationWrapper wrapper)
        {
            return base.Add(wrapper);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}