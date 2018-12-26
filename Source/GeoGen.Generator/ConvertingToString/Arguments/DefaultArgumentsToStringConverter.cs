using GeoGen.Core;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="Arguments"/>.
    /// It converts arguments to a string using <see cref="IGeneralArgumentsToStringConverter"/> in a way
    /// that every object is converted to its id. The main use-case for this is the following:
    /// When we need to find out if some arguments (for example {1;2} and {2;3}) are equal, 
    /// and we're sure the objects they're composed of have distinct ids, then we just need to convert
    /// each object into the string representing its ids.
    /// </summary>
    public class DefaultArgumentsToStringConverter : IToStringConverter<Arguments>
    {
        #region Dependencies

        /// <summary>
        /// The general arguments to string provider to which the actual conversion is delegated.
        /// </summary>
        private readonly IGeneralArgumentsToStringConverter _argumentsToString;

        #endregion

        #region Private fields

        /// <summary>
        /// The single instance of the to string converter that is used to convert the internal object of arguments, each to its id.
        /// </summary>
        private readonly IToStringConverter<ConfigurationObject> _converter = new FuncToStringConverter<ConfigurationObject>(obj => obj.Id.ToString());

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultArgumentsToStringConverter"/> class.
        /// </summary>
        /// <param name="argumentsToString">The general arguments to string provider to which the actual conversion is delegated.</param>
        public DefaultArgumentsToStringConverter(IGeneralArgumentsToStringConverter argumentsToString)
        {
            _argumentsToString = argumentsToString ?? throw new ArgumentNullException(nameof(argumentsToString));
        }

        #endregion

        #region IToStringConverter implementation

        /// <summary>
        /// Converts given arguments to a string.
        /// </summary>
        /// <param name="arguments">The arguments to be converted.</param>
        /// <returns>A string representation of the arguments.</returns>
        public string ConvertToString(Arguments arguments)
        {
            // Call the general converter using the object to string converter that converts an object to its id
            return _argumentsToString.ConvertToString(arguments, _converter);
        }

        #endregion
    }
}