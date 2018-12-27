using GeoGen.Core;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="ConfigurationObject"/>.
    /// It converts objects to a string using <see cref="IFullObjectToStringConverter"/> in a way that no 
    /// id is being remapped (it passes the value <see cref="LooseObjectIdsRemapping.NoRemapping"/> to it).
    /// The use-case for this is to recognize equal configuration objects.
    /// </summary>
    public class DefaultFullObjectToStringConverter : IToStringConverter<ConfigurationObject>
    {
        #region Dependencies

        /// <summary>
        /// The general full object to string converter to which the actual conversion is delegated.
        /// </summary>
        private readonly IFullObjectToStringConverter _objectToString;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFullObjectToStringConverter"/> class.
        /// </summary>
        /// <param name="objectToString">The general full object to string converter to which the actual conversion is delegated.</param>
        public DefaultFullObjectToStringConverter(IFullObjectToStringConverter objectToString)
        {
            _objectToString = objectToString ?? throw new ArgumentNullException(nameof(objectToString));
        }

        #endregion

        #region IToStringConverter implementation

        /// <summary>
        /// Converts a given configuration object to a string.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be converted.</param>
        /// <returns>A string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            // Call the general full converter with no loose objects ids remapping
            return _objectToString.ConvertToString(configurationObject, LooseObjectIdsRemapping.NoRemapping);
        }

        #endregion
    }
}
