using System;

namespace GeoGen.Analyzer
{
    internal sealed class AnalyzerException : Exception
    {
        public AnalyzerException(string message)
            : base(message)
        {
        }
    }
}