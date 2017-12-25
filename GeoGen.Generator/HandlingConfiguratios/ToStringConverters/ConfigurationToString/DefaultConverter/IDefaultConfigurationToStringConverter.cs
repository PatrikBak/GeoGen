using GeoGen.Utilities.DataStructures;

namespace GeoGen.Generator
{
    internal interface IDefaultConfigurationToStringConverter : IToStringConverter<ConfigurationWrapper>
    {
        IObjectIdResolver Resolver { get; }
    }
}
