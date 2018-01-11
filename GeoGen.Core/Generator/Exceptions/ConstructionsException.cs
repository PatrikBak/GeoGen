using System;

namespace GeoGen.Core.Generator
{
    public class ConstructionException : GeneratorException
    {
        public ConstructionException()
        {
        }

        public ConstructionException(string message)
                : base(message)
        {
        }

        public ConstructionException(string message, Exception innerException)
                : base(message, innerException)
        {
        }

        public static ConstructionException ConstructionMostNotBeNull()
        {
            return new ConstructionException("Construction can't be null.");
        }

        public static ConstructionException ConstructionIdNotSet()
        {
            return new ConstructionException("Construction id must be set.");
        }

        public static ConstructionException ConstructionsMustHaveUniqueId()
        {
            return new ConstructionException("Constructions must have mutually distinct ids.");
        }

        public static ConstructionException ConstructionsMustNotBeNull()
        {
            return new ConstructionException("Constructions must not be null.");
        }
    }
}