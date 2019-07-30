using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents a service that finds out if theorems are olympiad.
    /// If not, it gives feedback why not.
    /// </summary>
    public interface ITheoremsAnalyzer
    {
        /// <summary>
        /// Performs the analysis for given input.
        /// </summary>
        /// <param name="input">The input for the analyzer.</param>
        /// <returns>
        /// The dictionary mapping non-olympiad theorems to their feedback. 
        /// The ones that are not presents are hopefully olympiad.
        /// </returns>
        Dictionary<Theorem, TheoremFeedback> Analyze(TheoremAnalyzerInput input);
    }
}
