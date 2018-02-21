using GeoGen.Core;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an exception that is thrown when something wrong
    /// happens in the generator module. 
    /// </summary>
    public class GeneratorException : GeoGenException
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