using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// An exception used for internal generator errors.
    /// </summary>
    public sealed class GeneratorException : Exception
    {
        /// <summary>
        /// Constructs a new generator exception with a given message.
        /// </summary>
        /// <param name="message">The message.</param>
        public GeneratorException(string message)
            : base(message)
        {
        }
    }
}