using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Generator.IntegrationTest
{
    internal class DummyGradualAnalyzer : IGradualAnalyzer
    {
        public GradualAnalyzerOutput Analyze(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects)
        {
            return new GradualAnalyzerOutput
            {
                    Theorems = new List<Theorem> { null },
                    UnambiguouslyConstructible = true
            };
        }
    }
}