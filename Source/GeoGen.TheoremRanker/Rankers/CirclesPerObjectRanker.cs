using GeoGen.Core;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.CirclesPerObject"/>.
    /// </summary>
    public class CirclesPerObjectRanker : AspectTheoremRankerBase
    {
        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <returns>A number representing the ranking of the theorem together with the explanation of how it was calculated.</returns>
        public override (double ranking, string message) Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // Get the number of concyclic theorems
            var concyclicTheorems = allTheorems.GetObjectsForKeys(TheoremType.ConcyclicPoints).Count();

            // Get the number of objects
            var numberOfObjects = configuration.AllObjects.Count;

            // Now simply apply the formula described in the documentation of RankedAspect.CirclesPerObject
            // And for the message include the whole formula
            return (1 / (1 + (double)concyclicTheorems / numberOfObjects), $"1 / (1 + concyclic theorems [{concyclicTheorems}] / configuration objects [{numberOfObjects}])");
        }
    }
}
