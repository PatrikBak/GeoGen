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
        /// Gets or sets the list of theorems that hold true in the configuration.
        /// </summary>
        public TheoremsMap AllTheorems { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that hold true in the configuration and 
        /// cannot be stated without the last object of it. These theorems will be 
        /// analyzed whether they are interesting or not.
        /// </summary>
        public TheoremsMap NewTheorems { get; set; }
    }
}
