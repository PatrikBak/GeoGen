using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the drawer module.
    /// </summary>
    public class ConstructionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawerException"/> class.
        /// </summary>
        public ConstructionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawerException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public ConstructionException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawerException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public ConstructionException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
