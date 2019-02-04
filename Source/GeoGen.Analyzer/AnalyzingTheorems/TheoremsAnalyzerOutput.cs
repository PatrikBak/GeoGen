using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output of a <see cref="ITheoremsAnalyzer"/> service containing found theorems.
    /// </summary>
    public class TheoremsAnalyzerOutput
    {
        /// <summary>
        /// Gets or sets whether the theorem analysis successfully finished. 
        /// </summary>
        public bool TheoremAnalysisSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the list of found theorems.
        /// </summary>
        public List<AnalyzedTheorem> Theorems { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that were true in some containers, but after
        /// the second test they weren't evaluated as correct ones.
        /// </summary>
        public List<AnalyzedTheorem> PotentialFalseNegatives { get; set; }

        /// <summary>
        /// The manager of all the containers holding analytic representations of the objects.
        /// </summary>
        public IObjectsContainersManager Manager { get; set; }
    }
}
