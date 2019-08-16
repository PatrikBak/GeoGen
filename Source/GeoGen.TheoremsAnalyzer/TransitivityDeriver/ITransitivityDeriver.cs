using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents a service that is able derive new theorems using the transivity rule,
    /// i.e. if x = y and y = z, then x = z. 
    /// </summary>
    public interface ITransitivityDeriver
    {
        /// <summary>
        /// Derives new theorems using transitivity.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorems hold true.</param>
        /// <param name="trueTheorems">All theorems that are true and contain the last object of the configuration.</param>
        /// <param name="assumedTheorems">The theorems that have already been ruled out as not interesting.</param>
        /// <returns>The enumerable of tuples consisting of two used fact to draw a conclusion and the conclusion itself.</returns>
        IEnumerable<(Theorem fact1, Theorem fact2, Theorem conclusion)> Derive(Configuration configuration, IEnumerable<Theorem> trueTheorems, IEnumerable<Theorem> assumedTheorems);
    }
}
