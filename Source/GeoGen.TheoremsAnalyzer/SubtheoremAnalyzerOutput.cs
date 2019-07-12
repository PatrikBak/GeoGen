using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an output of the <see cref="ISubtheoremAnalyzer"/>.
    /// </summary>
    public class SubtheoremAnalyzerOutput
    {
        /// <summary>
        /// Gets or sets if the theorem is sub-theorem of a provided theorem.
        /// </summary>
        public bool IsSubtheorem { get; set; }

        /// <summary>
        /// Gets or sets the list of all objects equalities that were needed to detect the sub-theorem.
        /// </summary>
        public List<(ConfigurationObject originalObject, ConfigurationObject equalObject)> UsedEqualities { get; set; }

        /// <summary>
        /// Gets or sets the list of all used theorems that were needed to detect the sub-theorem.
        /// These theorems are related to the initial layout of objects. For example: 
        /// We might have a layout of concyclic quadruples of points. If we successfully
        /// match some, then the fact those points are concyclic might be an interesting theorem,
        /// so we remember it. 
        /// </summary>
        public List<Theorem> UsedFacts { get; set; }
    }
}