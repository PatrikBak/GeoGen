using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing
{
    internal interface IConfigurationConstructor
    {
        ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput);
    }
}
