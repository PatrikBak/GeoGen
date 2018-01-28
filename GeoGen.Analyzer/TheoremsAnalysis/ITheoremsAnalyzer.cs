using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that finds theorem in a configuration.
    /// </summary>
    public interface ITheoremsAnalyzer
    {
        /// <summary>
        /// Performs theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        List<Theorem> Analyze(Configuration configuration);
    }
}