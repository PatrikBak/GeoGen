using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an input of the <see cref="ISubtheoremAnalyzer"/>.
    /// </summary>
    public class SubtheoremAnalyzerInput
    {
        /// <summary>
        /// Gets or sets the trivial that is tested whether it implies the examined one.
        /// </summary>
        public Theorem TemplateTheorem { get; set; }

        /// <summary>
        /// Gets or sets the theorem about which we want to found out whether it is a consequence of a simple one.
        /// </summary>
        public Theorem ExaminedTheorem { get; set; }

        /// <summary>
        /// Gets or sets the manager of pictures where the examined configuration is drawn.
        /// </summary>
        public IPicturesManager ExaminedConfigurationManager { get; set; }

        /// <summary>
        /// Gets or sets the contextual picture where the examined configuration is drawn.
        /// </summary>
        public IContextualPicture ExaminedConfigurationContexualPicture { get; set; }

        /// <summary>
        /// Gets or sets the container where the objects of the examined configuration should be contained.
        /// </summary>
        public IContainer<ConfigurationObject> ExaminedConfigurationObjectsContainer { get; set; }
    }
}
