using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;

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
        /// <summary>
        /// A dummy implementation of <see cref="IConfigurationsHandler"/>
        /// that does nothing with the output. This is useful for testing the
        /// generation process performance that doesn't include the analyzer module.
        /// </summary>
        internal sealed class DummyConfigurationsHandler : IConfigurationsHandler
        {
            public IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations)
            {
                return configurations.Select(c => new GeneratorOutput());
            }
        }

        /// <summary>
        /// Executes all bindings.
        /// </summary>
        public override void Load()
        {
            base.Load();
            Bind<IConfigurationsHandler>().To<DummyConfigurationsHandler>().InSingletonScope();
        }
    }
}