using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a result returned by a <see cref="IGeometryRegistrar"/>.
    /// </summary>s
    public class RegistrationResult
    {
        /// <summary>
        /// Gets of sets the list of duplicate object, where each duplicate is represented as a tuple
        /// containing the new object and the older object. The new objects should be a subset of 
        /// objects gotten in a single construction.
        /// </summary>
        public IReadOnlyList<(ConfigurationObject newerObject, ConfigurationObject olderObject)> Duplicates { get; set; }

        /// <summary>
        /// Gets or sets the list of unconstructible configuration objects. This objects should be the result
        /// of a single construction.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> UnconstructibleObjects { get; set; }
    }
}
