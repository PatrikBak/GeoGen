using GeoGen.Core;

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
        /// <returns>The output of the analyzer holding the theorems.</returns>
        TheoremsAnalyzerOutput Analyze(Configuration configuration);
    }
}