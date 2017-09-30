using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output from the analyzer.
    /// </summary>
    public class AnalyzerOutput
    {
        /// <summary>
        /// Gets or set the list of theorems found during the analysis. 
        /// </summary>
        public List<Theorem> Theorems { get; set; }

        /// <summary>
        /// Gets or sets if all added constructed points can be correctly constructed.
        /// If at least one of newly added objects can't be constructed, then 
        /// the configuration is considered as not fully constructible.
        /// </summary>
        public bool CanBeFullyConstructed { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping newly added configuration objects
        /// to their versions that are already present in the configuration. The idea behind
        /// is that two objects that are synthetically distinct could be geometrically the same
        /// (which could potentially be a theorem), but constructing both of them is useful. 
        /// </summary>
        public Dictionary<ConfigurationObject, ConfigurationObject> DuplicateObjects { get; set; }
    }
}