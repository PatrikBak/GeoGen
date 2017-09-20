using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Configuration"/>, containing some
    /// additional information regarding the construction. During the generation 
    /// process, these instances are preferred.
    /// </summary>
    internal class ConstructionWrapper
    {
        /// <summary>
        /// Gets or sets the construction that this class wrapps.
        /// </summary>
        public Construction Construction { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping configuration objects types 
        /// to the number of objects of that type that are needed to be 
        /// passed to the construction.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; set; }
    }
}
