using System;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an exception that is thrown when something wrong
    /// happens in the analyzer module. 
    /// </summary>
    public class AnalyzerException : Exception
    {
        public AnalyzerException()
        {
        }

        public AnalyzerException(string message)
                : base(message)
        {
        }

        public AnalyzerException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}