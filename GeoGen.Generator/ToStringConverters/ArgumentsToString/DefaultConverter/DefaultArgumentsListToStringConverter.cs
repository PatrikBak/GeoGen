using System;
using System.Collections.Generic;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IDefaultFullObjectToStringConverter"/>.
    /// This class works as an adapter for <see cref="StringBasedContainer{T}"/>, since
    /// it implements the <see cref="IObjectToStringConverter"/> interface. It's adapting
    /// more general <see cref="IArgumentsListToStringProvider"/> interface that requires
    /// <see cref="IObjectToStringConverter"/> in order to convert an arguments list to string.
    /// This conversion uses a <see cref="IDefaultObjectIdResolver"/> (hence the name default).
    ///</summary>
    internal class DefaultArgumentsListToStringConverter : IDefaultArgumentsListToStringConverter
    {
        #region Private fields

        /// <summary>
        /// The general arguments list to string provider that does the actual conversion.
        /// </summary>
        private readonly IArgumentsListToStringProvider _argumentsToString;

        /// <summary>
        /// The default object to string converter that is passed to the arguments list to string provider.
        /// </summary>
        private readonly IDefaultObjectToStringConverter _objectToString;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="argumentsToString">The general arguments list to string provider.</param>
        /// <param name="objectToString">The default object to string provider.</param>
        public DefaultArgumentsListToStringConverter(IArgumentsListToStringProvider argumentsToString, IDefaultObjectToStringConverter objectToString)
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
        public string ConvertToString(IReadOnlyList<ConstructionArgument> item)
        {
            return _argumentsToString.ConvertToString(item, _objectToString);
        }

        #endregion
    }
}