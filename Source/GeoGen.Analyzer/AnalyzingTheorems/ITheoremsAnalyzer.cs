using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that finds <see cref="Theorem"/>s in a <see cref="Configuration"/>.
    /// </summary>
    public interface ITheoremsAnalyzer
    {
        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        List<Theorem> Analyze(Configuration configuration);
    }
}