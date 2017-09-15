using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing
{
    internal interface IConfigurationConstructor
    {
        ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput);
    }
}
