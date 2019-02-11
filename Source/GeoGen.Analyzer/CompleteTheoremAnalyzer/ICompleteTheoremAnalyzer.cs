using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that finds all theorems that are true in a configuration,
    /// unlike <see cref="IPotentialTheoremsAnalyzer"/>.
    /// </summary>
    public interface ICompleteTheoremAnalyzer
    {
        /// <summary>
        /// Finds all the theorems that are true in a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The analysis output.</returns>
        TheoremAnalysisOutput Analyze(Configuration configuration);
    }
}
