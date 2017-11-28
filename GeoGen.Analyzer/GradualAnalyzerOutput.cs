using System.Collections.Generic;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output from the analyzer.
    /// </summary>
    public sealed class GradualAnalyzerOutput
    {
        /// <summary>
        /// Gets or sets if the analyzed configuration is correctly constructible. 
        /// This means that its every object is constructible and that all there
        /// is no other configuration with the same geometrical representation 
        /// (in other words, all its configuration objects have unique geometrical
        /// representations across all generated configuration objects). If this
        /// is false, then this configuration should not be further extended and
        /// its last constructed objects should not be consider any further. This
        /// makes sense because they are either non-constructible (such as 
        /// intersection of parallel lines), or there are duplicate objects so
        /// based on the generator logic there should exist a configuration 
        /// with the same geometrical representation). 
        /// </summary>
        public bool UnambiguouslyConstructible { get; set; }

        /// <summary>
        /// Gets or set the list of theorems found during the analysis. This might
        /// not be empty even if the configuration was rejected to be analyzed further
        /// (this happens in cases when the configuration is not constructible or there
        /// are duplicate objects). In that case, there will be theorems regarding
        /// the sameness of objects, which might be an interesting theorem after all.
        /// </summary>
        public List<Theorem> Theorems { get; set; }
    }
}