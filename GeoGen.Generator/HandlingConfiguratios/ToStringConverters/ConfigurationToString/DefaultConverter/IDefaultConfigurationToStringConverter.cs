using GeoGen.Utilities;

namespace GeoGen.Generator
{
    internal interface IDefaultConfigurationToStringConverter : IToStringConverter<ConfigurationWrapper>
    {
        IObjectIdResolver Resolver { get; }
    }
}
