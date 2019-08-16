using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="Arguments"/>.
    /// It converts arguments to a string using <see cref="IGeneralArgumentsToStringConverter"/> in a way
    /// that every object is converted to an integer. The main use-case for this is the following:
    /// When we need to find out if some arguments (for example {A;B} and {B;C}, or {A;B} and {B,A}) are equal, 
    /// then we don't need to know how exactly A,B,C are defined -- we just need to know they're distinct. 
    /// </summary>
    public class DefaultArgumentsToStringConverter : IToStringConverter<Arguments>
    {
        #region Dependencies

        /// <summary>
        /// The general arguments to string converter to which the actual conversion is delegated.
        /// </summary>
        private readonly IGeneralArgumentsToStringConverter _argumentsToString;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary that maps configuration objects to integers that are used in the conversion to get shorter strings.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, int> _objectIds = new Dictionary<ConfigurationObject, int>();

        /// <summary>
        /// The single instance of the to string converter that is used to convert the internal object of arguments.
        /// </summary>
        private readonly IToStringConverter<ConfigurationObject> _converter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultArgumentsToStringConverter"/> class.
        /// </summary>
        /// <param name="argumentsToString">The general arguments to string converter to which the actual conversion is delegated.</param>
        public DefaultArgumentsToStringConverter(IGeneralArgumentsToStringConverter argumentsToString)
        {
            _argumentsToString = argumentsToString ?? throw new ArgumentNullException(nameof(argumentsToString));

            // Create the converter that makes sure the object has a unique id in the dictionary and converts it to a string 
            _converter = new FuncToStringConverter<ConfigurationObject>(obj => _objectIds.GetOrAdd(obj, () => _objectIds.Count).ToString());
        }

        #endregion

        #region IToStringConverter implementation

        /// <summary>
        /// Converts given arguments to a string.
        /// </summary>
        /// <param name="arguments">The arguments to be converted.</param>
        /// <returns>A string representation of the arguments.</returns>
        public string ConvertToString(Arguments arguments) => _argumentsToString.ConvertToString(arguments, _converter);

        #endregion
    }
}