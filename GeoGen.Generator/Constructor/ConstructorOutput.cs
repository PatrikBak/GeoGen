using GeoGen.Core.Configurations;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor
{
    internal class ConstructorOutput
    {
        public ConfigurationWrapper InitialConfiguration { get; }

        public ConstructedConfigurationObject ConstructedObject { get; }

        public ConstructorOutput(ConfigurationWrapper initialConfiguration, ConstructedConfigurationObject constructedObject)
        {
            InitialConfiguration = initialConfiguration;
            ConstructedObject = constructedObject;
        }
    }
}