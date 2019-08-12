using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an input for the <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremAnalyzerInput
    {
        /// <summary>
        /// Gets or sets the configuration that should be analyzed with its theorems.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that hold true in the configuration.
        /// </summary>
        public List<Theorem> Theorems { get; set; }

        /// <summary>
        /// Gets or sets the manager of pictures where the configuration is drawn.
        /// </summary>
        public Pictures Manager { get; set; }

        /// <summary>
        /// Gets or sets the contextual picture where the configuration is drawn.
        /// </summary>
        public ContextualPicture ContextualPicture { get; set; }

        /// <summary>
        /// Gets or sets the container where the objects of the examined configuration should be contained.
        /// </summary>
        public IContainer<ConfigurationObject> ConfigurationObjectsContainer { get; set; }
    }
}
