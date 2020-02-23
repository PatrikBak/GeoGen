using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.TheoremsPerObject"/>.
    /// </summary>
    public class TheoremsPerObjectRanker : AspectTheoremRankerBase
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
            // Get the number of theorems
            var numberOfTheorems = allTheorems.AllObjects.Count;

            // Get the number of objects
            var numberOfObjects = configuration.AllObjects.Count;

            // Simply apply the formula described in the documentation of RankedAspect.TheoremsPerObject
            // And for the message include the whole formula
            return (1 / (1 + (double)numberOfTheorems / numberOfObjects), $"1 / (1 + theorems [{numberOfTheorems}] / configuration objects [{numberOfObjects}])");
        }
    }
}
