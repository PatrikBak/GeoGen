using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents an exception that is thrown when geometrically illogical things 
    /// are passed as arguments (for example, constructing a circle from three collinear points).
    /// </summary>
    public class AnalyticException : Exception
    {
        public AnalyticException()
        {
        }

        public AnalyticException(string message)
                : base(message)
        {
        }

        public AnalyticException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}