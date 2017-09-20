using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator.ConstructingConfigurations
{
    internal interface IConfigurationConstructor
    {
        ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput);
    }
}
