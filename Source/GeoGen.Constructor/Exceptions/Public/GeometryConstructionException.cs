using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An exception thrown by <see cref="IGeometryConstructor"/> when a construction of geometry objects fails.
    /// </summary>
    public class GeometryConstructionException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructionException"/> class.
        /// </summary>
        public GeometryConstructionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructionException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public GeometryConstructionException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructionException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public GeometryConstructionException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
