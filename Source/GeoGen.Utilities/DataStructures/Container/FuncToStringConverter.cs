using System;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/> that wraps a <see cref="Func{T, TResult}"/>, 
    /// where 'TResult' is <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items that are converted.</typeparam>
    public class FuncToStringConverter<T> : IToStringConverter<T>
    {
        #region Private fields

        /// <summary>
        /// The function that performs the actual conversion.
        /// </summary>
        private readonly Func<T, string> _function;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncToStringConverter{T}"/> wrapping a given function.
        /// </summary>
        /// <param name="function">The function that performs the actual conversion.</param>
        public FuncToStringConverter(Func<T, string> function)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }

        #endregion

        #region IToStringConverter implementation

        /// <summary>
        /// Converts a given item to a string.
        /// </summary>
        /// <param name="item">The item to be converted.</param>
        /// <returns>A string representation of the item.</returns>
        public string ConvertToString(T item) => _function(item);

        #endregion
    }
}
