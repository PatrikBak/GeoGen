using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a base type of exception for the system.
    /// </summary>
    public class GeoGenException : Exception
    {
        public GeoGenException()
        {
        }

        public GeoGenException(string message)
            :  base(message)
        {
        }

        public GeoGenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
