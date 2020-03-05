using System;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the sorter module.
    /// </summary>
    public class TheoremSorterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorterException"/> class.
        /// </summary>
        public TheoremSorterException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorterException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public TheoremSorterException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorterException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public TheoremSorterException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
