using System;

namespace GeoGen.Core.Generator
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

        public static GeneratorException ObjectIdNotSet()
        {
            return new GeneratorException("Configuration objects id must be set.");
        }

        public static GeneratorException ConfigurationIdNotSet()
        {
            return new GeneratorException("Configuration id is not set.");
        }
    }
}