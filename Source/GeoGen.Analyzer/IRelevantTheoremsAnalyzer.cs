using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that is able to theorems in an already drawn configuration. This service
    /// should only find those theorems that can't be defined in any sub-configuration of the given one.
    /// </summary>
    public interface IRelevantTheoremsAnalyzer
    {
        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <param name="manager">The manager of all the containers where the theorems should be tested.</param>
        /// <returns>The output of the analyzer holding the theorems.</returns>
        TheoremAnalysisOutput Analyze(Configuration configuration, IObjectsContainersManager manager);
    }
}