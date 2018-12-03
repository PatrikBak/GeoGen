using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of <see cref="ConfigurationWrapper"/>s to string that uses
    /// the full string conversion. This type of conversion makes sure that
    /// formally equal configurations will get have same string representation.
    /// </summary>
    internal interface IFullConfigurationToStringConverter : IToStringConverter<ConfigurationWrapper>
    {
    }
}