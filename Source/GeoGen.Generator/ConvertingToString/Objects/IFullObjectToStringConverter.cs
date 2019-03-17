using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of a <see cref="ConfigurationObject"/> to a string which converts an object
    /// to a string using only the loose objects it internally consist of. This converter allows to specify 
    /// a custom <see cref="LooseObjectsRemapping"/> to be used during the conversion. If there should be no 
    /// remapping, then the constant value <see cref="LooseObjectsRemapping.NoRemapping"/> should be passed. 
    /// It should fulfill the invariant that for any remapping two objects are equal if and only if their string 
    /// versions (with respect to the current remapping) are equal.
    /// </summary>
    public interface IFullObjectToStringConverter
    {
        /// <summary>
        /// Converts a given configuration object to a string using a given remapping of loose objects during the conversion.
        /// </summary>
        /// <param name="configurationObject">The object to be converted.</param>
        /// <param name="remapping">The remapping of loose objects to be used during the conversion.</param>
        /// <returns>A string representation of the object.</returns>
        string ConvertToString(ConfigurationObject configurationObject, LooseObjectsRemapping remapping);
    }
}