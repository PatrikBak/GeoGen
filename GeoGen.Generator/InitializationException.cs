using System;

namespace GeoGen.Generator
{
    public class InitializationException : Exception
    {
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