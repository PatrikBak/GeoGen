using GeoGen.Core;
using GeoGen.Utilities;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Theorems"/>.
    /// </summary>
    public class TheoremsRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Take the theorems
            => allTheorems.AllObjects
                // That use the last object of the configuration
                .Where(theorem => theorem.GetInnerConfigurationObjects().Contains(configuration.LastConstructedObject))
                // Except for this one
                .Except(theorem.ToEnumerable())
                // We want their count
                .Count();
    }
}
