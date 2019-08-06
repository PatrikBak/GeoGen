using GeoGen.Core;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents a feedback theorem is derived using the transitivity
    /// property of some theorems, such as <see cref="TheoremType.EqualAngles"/>.
    /// </summary>
    public class TransitivityFeedback : TheoremFeedback
    {
        /// <summary>
        /// The first fact used in the conclusion.
        /// </summary>
        public Theorem Fact1 { get; set; }

        /// <summary>
        /// The second fact used in the conclusion.
        /// </summary>
        public Theorem Fact2 { get; set; }
    }
}
