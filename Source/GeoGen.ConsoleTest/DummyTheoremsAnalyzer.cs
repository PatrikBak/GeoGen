using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.ConsoleTest
{
    public class DummyTheoremsAnalyzer : ITheoremsAnalyzer
    {
        TheoremsAnalyzerOutput ITheoremsAnalyzer.Analyze(Configuration configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}