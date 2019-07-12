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

        public IPicturesManager ExaminedConfigurationManager { get; set; }

        public IContextualPicture ExaminedConfigurationContexualPicture { get; set; }

        public IContainer<ConfigurationObject> ExaminedConfigurationObjectsContainer { get; set; }
    }
}
