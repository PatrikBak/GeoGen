using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a wrapper of a <see cref="Construction"/>, containing some
    /// additional data regarding the construction.
    /// </summary>
    internal class ConstructionWrapper
    {
        /// <summary>
        /// Gets or sets the construction.
        /// </summary>
        public Construction WrappedConstruction { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping configuration objects types 
        /// to the number of objects of that type that are needed to be 
        /// passed to the construction.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; set; }
    }
}