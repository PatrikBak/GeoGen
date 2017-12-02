using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Generator.ConfigurationsHandling;

namespace GeoGen.Generator.NInject
{
    /// <summary>
    /// A generator module used for testing constructions process itself without
    /// using the Analyzer module. It inject <see cref="IConfigurationsHandler"/>
    /// to a dummy sealed class that for every generated configuration simply returns
    /// a new instance of empty generator output.
    /// </summary>
    public sealed class ConstructionTestModule : GeneratorModuleBase
    {
        internal sealed class DummyGradualAnalyzer : IGradualAnalyzer
        {
            public GradualAnalyzerOutput Analyze(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects)
            {
                return new GradualAnalyzerOutput
                {
                    Theorems = new List<Theorem> {null},
                    UnambiguouslyConstructible = true
                };
            }
        }

        /// <summary>
        /// Executes all bindings.
        /// </summary>
        public override void Load()
        {
            base.Load();
            Bind<IGradualAnalyzer>().To<DummyGradualAnalyzer>().InSingletonScope();
        }
    }
}