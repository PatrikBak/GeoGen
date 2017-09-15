using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    internal interface IArgumentsToStringProviderFactory
    {
        IArgumentsToStringProvider CreateProvider(DefaultObjectToStringProvider provider);
    }
}