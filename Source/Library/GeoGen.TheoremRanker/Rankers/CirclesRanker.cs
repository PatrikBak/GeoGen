using GeoGen.Core;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Circles"/>.
    /// </summary>
    public class CirclesRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Simply return the number of theorems of type ConcyclicPoints
            => allTheorems.GetObjectsForKeys(TheoremType.ConcyclicPoints).Count();
    }
}