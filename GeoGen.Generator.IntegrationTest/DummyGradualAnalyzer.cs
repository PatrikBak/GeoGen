using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Generator.IntegrationTest
{
    internal class DummyGradualAnalyzer : IGradualAnalyzer
    {
        public List<Theorem> Analyze(IReadOnlyList<ConfigurationObject> oldObjects, IReadOnlyList<ConstructedConfigurationObject> newObjects)
        {
            return new List<Theorem> {null};
        }
    }
}