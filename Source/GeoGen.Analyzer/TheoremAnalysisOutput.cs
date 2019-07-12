using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output of a <see cref="IRelevantTheoremsAnalyzer"/> service containing found theorems.
    /// </summary>
    public class TheoremAnalysisOutput
    {
        /// <summary>
        /// Gets or sets the list of found theorems.
        /// </summary>
        public List<AnalyzedTheorem> Theorems { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that were true in some pictures, but after
        /// the second test they weren't evaluated as correct ones.
        /// </summary>
        public List<AnalyzedTheorem> PotentialFalseNegatives { get; set; }
    }
}
