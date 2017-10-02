using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Theorems
{
    /// <summary>
    /// Represents a service that does actual geometrical theorem finding.
    /// </summary>
    internal interface ITheoremsFinder
    {
        /// <summary>
        /// Finds all theorems that holds in the configuration consisted of
        /// all provided objects, but that have anything to do with some of
        /// the new objects. 
        /// </summary>
        /// <param name="oldObjects">The old objects.</param>
        /// <param name="newObjects">The new objects.</param>
        /// <returns>The found theorems enumerable.</returns>
        IEnumerable<Theorem> Find(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects);
    }
}