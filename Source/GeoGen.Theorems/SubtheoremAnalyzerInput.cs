using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.Theorems
{
    /// <summary>
    /// Represents an input of the <see cref="ISubtheoremAnalyzer"/>.
    /// </summary>
    public class SubtheoremAnalyzerInput
    {
        public Theorem TemplateTheorem { get; set; }

        public Theorem ExaminedTheorem { get; set; }

        public IObjectsContainersManager ExaminedConfigurationManager { get; set; }

        public IContextualContainer ExaminedConfigurationContexualContainer { get; set; }

        public IContainer<ConfigurationObject> ExaminedConfigurationObjectsContainer { get; set; }
    }
}
