using System;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IDefaultFullObjectToStringConverter"/>.
    /// This class works as an adapter for <see cref="StringBasedContainer{T}"/>, since
    /// it implements the <see cref="IObjectToStringConverter"/> interface. It's adapting
    /// more general <see cref="IArgumentsToStringProvider"/> interface that requires
    /// <see cref="IObjectToStringConverter"/> in order to convert an arguments to string.
    /// This conversion uses a <see cref="IDefaultObjectIdResolver"/> (hence the name default).
    ///</summary>
    public class DefaultArgumentsToStringConverter : IToStringConverter<Arguments>
    {
        #region Private fields

        /// <summary>
        /// The general arguments to string provider that does the actual conversion.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToString;

        /// <summary>
        /// The default object to string converter that is passed to the general arguments to string provider.
        /// </summary>
        private readonly DefaultObjectToStringConverter _objectToString;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="argumentsToString">The general arguments to string provider.</param>
        /// <param name="objectToString">The default object to string provider.</param>
        public DefaultArgumentsToStringConverter(IArgumentsToStringProvider argumentsToString, DefaultObjectToStringConverter objectToString)
        {
            _argumentsToString = argumentsToString ?? throw new ArgumentNullException(nameof(argumentsToString));
            _objectToString = objectToString ?? throw new ArgumentNullException(nameof(objectToString));
        }

        #endregion

        #region IObjectToString implementation

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The string representation.</returns>
        public string ConvertToString(Arguments item)
        {
            // Call the general converter using a provided object to string converter
            return _argumentsToString.ConvertToString(item, _objectToString);
        }

        #endregion
    }
}