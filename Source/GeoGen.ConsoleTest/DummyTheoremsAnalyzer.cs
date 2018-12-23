using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    public class DummyTheoremsAnalyzer : ITheoremsAnalyzer
    {
        public List<Theorem> Analyze(Configuration configuration)
        {
            return new List<Theorem>();
        }
    }
}