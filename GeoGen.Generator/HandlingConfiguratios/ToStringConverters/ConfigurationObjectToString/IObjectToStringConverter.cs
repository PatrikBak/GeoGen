using GeoGen.Core.Configurations;
using GeoGen.Utilities.DataStructures;

namespace GeoGen.Generator
{
    internal interface IObjectToStringConverter : IToStringConverter<ConfigurationObject>
    {
        /// <summary>
        /// Gets the object id resolver associated with this convert
        /// </summary>
        IObjectIdResolver Resolver { get; }
    }
}