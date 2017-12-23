using System;

namespace GeoGen.Core.Generator
{
    /// <summary>
    /// An exception used for internal generator errors.
    /// </summary>
    internal sealed class GeneratorException : Exception
    {
        /// <summary>
        /// Constructs a new generator exception with a given message.
        /// </summary>
        /// <param name="message">The message.</param>
        public GeneratorException(string message)
            : base(message)
        {
        }

        public static GeneratorException ConstructionIdMustBeSet()
        {
            return new GeneratorException("Constructions id must be set.");
        }

        public static GeneratorException ObjectsIdNotSet()
        {
            return new GeneratorException("Configuration objects id must be set.");
        }

        public static GeneratorException ConstructionMostNotBeNull()
        {
            return new GeneratorException("Construction can't be null");
        }

        public static GeneratorException ConstructionsMustHaveUniqueId()
        {
            throw new NotImplementedException();
        }
    }
}