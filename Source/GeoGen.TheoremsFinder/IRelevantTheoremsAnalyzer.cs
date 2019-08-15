using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a service that is able to find theorems in an already drawn configuration. This service
    /// should only find those theorems that can't be defined in any sub-configuration of the given one.
    /// </summary>
    public interface IRelevantTheoremsAnalyzerx
    {
        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <param name="manager">The manager of all the pictures where the theorems should be tested.</param>
        /// <param name="contextualPicture">The contextual picture where the configuration is drawn.</param>
        /// <returns>The list of theorems that are true in the configuration.</returns>
        List<Theorem> Analyze(Configuration configuration, Pictures manager, ContextualPicture contextualPicture);
    }
}