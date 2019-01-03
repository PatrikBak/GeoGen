using GeoGen.Core;
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
        /// Gets or sets the list of theorems. This value may be null if the theorem analysis didn't 
        /// finish successfully.
        /// </summary>
        public List<Theorem> Theorems { get; set; }


        public List<(Theorem, int)> NumberOfTrueContainers { get; set; }
    }
}
