using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IFullConfigurationToStringConverter"/> that is meant to be
    /// a converter for <see cref="ConfigurationObject"/>s that have their ids already this.
    /// Then it automatically handles the caching of string versions, unlike 
    /// <see cref="IDefaultFullObjectToStringConverter"/>.
    /// </summary>
    internal interface IAutocacheFullObjectToStringConverter : IFullObjectToStringConverter
    {
    }
}