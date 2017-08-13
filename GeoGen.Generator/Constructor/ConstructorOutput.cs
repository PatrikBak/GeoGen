using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor
{
    internal class ConstructorOutput
    {
        public Configuration InitialConfiguration { get; }

        public ConstructedConfigurationObject ConstructedObject { get; }

        public ConstructorOutput(Configuration initialConfiguration, ConstructedConfigurationObject constructedObject)
        {
            InitialConfiguration = initialConfiguration;
            ConstructedObject = constructedObject;
        }
    }
}