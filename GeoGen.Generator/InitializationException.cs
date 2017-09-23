using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// An exception used for errors caused by some malformed input
    /// passed to the generator.
    /// </summary>
    public class InitializationException : Exception
    {
        /// <summary>
        /// Constructs a new initialization exception with a given message.
        /// </summary>
        /// <param name="message">The message.</param>
        public InitializationException(string message)
            : base(message)
        {
        }
    }
}