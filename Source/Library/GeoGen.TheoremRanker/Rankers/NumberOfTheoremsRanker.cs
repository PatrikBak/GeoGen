using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.NumberOfTheorems"/>.
    /// </summary>
    public class NumberOfTheoremsRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Simply return the number of theorems, excluding the one we cannot prove
            => allTheorems.AllObjects.Count - 1;
    }
}
