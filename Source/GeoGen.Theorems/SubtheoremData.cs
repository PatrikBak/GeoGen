using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Theorems
{
    /// <summary>
    /// Represents an output of the <see cref="ISubtheoremAnalyzer"/>.
    /// </summary>
    public class SubtheoremData
    {
        /// <summary>
        /// Gets or sets if the theorem was successfully analyzed. 
        /// </summary>
        public bool SuccessfullyAnalyzed { get; set; }

        /// <summary>
        /// Gets or sets if the theorem is sub-theorem of a provided theorem.
        /// </summary>
        public bool IsSubtheorem { get; set; }

        /// <summary>
        /// Gets or sets the list of all objects equalities that were needed to detect the sub-theorem.
        /// </summary>
        public List<(ConfigurationObject originalObject, ConfigurationObject equalObject)> UsedEqualities { get; set; }

        /// <summary>
        /// Gets or sets the list of objects to which the original theorem configuration was mapped to detect the sub-theorem.
        /// </summary>
        public List<ConfigurationObject> MappedObjects { get; set; }
    }
}