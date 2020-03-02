using GeoGen.Core;
using GeoGen.Utilities;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Repetition"/>.
    /// </summary>
    public class RepetitionRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Take the constructed objects
            => configuration.ConstructedObjects
                // Group them by their construction
                .GroupBy(constructedObject => constructedObject.Construction)
                // For each group calculate the coefficient according to the formula (count-1)^4
                .Select(group => (group.Count() - 1d).Squared().Squared())
                // Sum these values
                .Sum();
    }
}