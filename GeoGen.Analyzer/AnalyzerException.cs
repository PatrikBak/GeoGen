using System;

namespace GeoGen.Analyzer
{
    public class AnalyzerException : Exception
    {
        public AnalyzerException(string message)
            : base(message)
        {
        }
    }
}