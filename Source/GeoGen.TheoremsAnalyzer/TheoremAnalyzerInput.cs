using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an input for the <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremAnalyzerInput
    {
        /// <summary>
        /// Gets or sets the contextual picture where the configuration is drawn.
        /// </summary>
        public ContextualPicture ContextualPicture { get; set; }

        /// <summary>
        /// Gets or sets the container where the objects of the examined configuration should be contained.
        /// </summary>
        public IContainer<ConfigurationObject> ConfigurationObjectsContainer { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that hold true in the configuration and 
        /// cannot be stated without the last object of it.
        /// </summary>
        public TheoremsMap NewTheorems { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that hold true in the configuration and don't
        /// use the last object of it.
        /// </summary>
        public TheoremsMap OldTheorems { get; set; }
    }
}
