using GeoGen.Core.Configurations;
using GeoGen.Utilities.DataStructures;

namespace GeoGen.Generator
{
    internal interface IObjectToStringConverter : IToStringConverter<ConfigurationObject>
    {
        /// <summary>
        /// Gets the unique id of this converter. 
        /// </summary>
        int Id { get; }
    }
}