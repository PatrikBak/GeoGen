using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// An exception used for internal generator errors.
    /// </summary>
    public class GeneratorException : Exception
    {
        public GeneratorException()
        {
        }

        public GeneratorException(string message)
            : base(message)
        {
        }

        public GeneratorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}