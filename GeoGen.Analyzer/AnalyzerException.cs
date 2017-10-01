using System;

namespace GeoGen.Analyzer
{
    internal class AnalyzerException : Exception
    {
        public AnalyzerException(string message)
            : base(message)
        {
        }
    }
}