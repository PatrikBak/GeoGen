using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Level"/>.
    /// </summary>
    public class LevelRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // Pull the number of constructed objects for comfort
            var n = configuration.ConstructedObjects.Count;

            // If there is at most 1 constructed object, let the level be 1
            // This should practically not happen, because we are supposed to be ranking
            // actual interesting problems...
            if (n <= 1)
                return 1;

            // Otherwise calculate the levels of objects
            var levels = configuration.CalculateObjectLevels();

            // And apply the formula from the documentations, i.e. 1 - 6[(l1^2+...+ln^2) - n] / [n(n-1)(2n+5)]
            return 1 - 6d * (levels.Values.Select(level => level * level).Sum() - n) / (n * (n - 1) * (2 * n + 5));
        }
    }
}