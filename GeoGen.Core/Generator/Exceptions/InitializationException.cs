using System;

namespace GeoGen.Core.Generator
{
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

        public static InitializationException NotConstructibleConfiguration()
        {
            return new InitializationException("Configuration is not constructible");
        }

        public static InitializationException DuplicateObjects()
        {
            return new InitializationException("Duplicate objects in the configuration.");
        }

        public static InitializationException InitialConfigurationCantBeNull()
        {
            return new InitializationException("Initial configuration can't be null");
        }

        public static InitializationException NullObjectPresent()
        {
            throw new InitializationException("Null object present.");
        }
    }
}