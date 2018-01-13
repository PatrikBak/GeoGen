using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a <see cref="GeneratorException"/> that is thrown
    /// when the generator could not have been initialized because of the incorrect output.
    /// </summary>
    public class InitializationException : GeneratorException
    {
        public InitializationException()
        {
        }

        public InitializationException(string message)
                : base(message)
        {
        }

        public InitializationException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}