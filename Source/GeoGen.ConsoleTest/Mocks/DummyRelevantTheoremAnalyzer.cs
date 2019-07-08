using GeoGen.Analyzer;
using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.ConsoleTest
{
    class DummyRelevantTheoremAnalyzer : IRelevantTheoremsAnalyzer
    {
        public TheoremAnalysisOutput Analyze(Configuration configuration, IObjectsContainersManager manager)
        {
            return new TheoremAnalysisOutput
            {
                PotentialFalseNegatives = new List<AnalyzedTheorem> { },
                Theorems = new List<AnalyzedTheorem> { },
                TheoremAnalysisSuccessful = true
            };
        }
    }
}
