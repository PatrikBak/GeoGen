using System;

namespace GeoGen.Analyzer
{
    internal class InconsistentContainersException : AnalyzerException
    {
        public IObjectsContainer InconsistentContainer { get; }

        public InconsistentContainersException(IObjectsContainer inconsistentContainer)
        {
            InconsistentContainer = inconsistentContainer;
        }
    }
}