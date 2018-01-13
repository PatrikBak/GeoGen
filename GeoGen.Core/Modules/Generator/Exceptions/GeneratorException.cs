using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an exception that is thrown when something wrong
    /// happens in the generator module. 
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