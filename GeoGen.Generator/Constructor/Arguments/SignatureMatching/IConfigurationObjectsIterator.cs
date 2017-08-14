using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    interface IConfigurationObjectsIterator
    {
        ConfigurationObject Next(ConfigurationObjectType type);
    }
}
