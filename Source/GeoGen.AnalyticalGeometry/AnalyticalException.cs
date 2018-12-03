using System;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// Represents an exception that is thrown when geometrically illogical things 
    /// are passed as arguments (for example, constructing a circle from three collinear points).
    /// </summary>
    internal class AnalyticalException : Exception
    {
        public AnalyticalException()
        {
        }

        public AnalyticalException(string message)
                : base(message)
        {
        }

        public AnalyticalException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}