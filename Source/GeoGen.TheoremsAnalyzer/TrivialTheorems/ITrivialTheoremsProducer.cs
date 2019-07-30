using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents a service that can find theorems that be directly derived from construction
    /// of the last object of a configuration.
    /// </summary>
    public interface ITrivialTheoremsProducer
    {
        /// <summary>
        /// Derive trivial theorems from the last object of a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem should hold.</param>
        /// <returns>The trivial theorems.</returns>
        IReadOnlyList<Theorem> DeriveTrivialTheoremsFromLastObject(Configuration configuration);
    }
}
