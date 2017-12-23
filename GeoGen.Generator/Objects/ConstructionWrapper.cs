using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// An internal wrapper sealed class for a <see cref="Configuration"/>, containing some
    /// additional data regarding the construction.
    /// </summary>
    internal sealed class ConstructionWrapper
    {
        /// <summary>
        /// Gets or sets the construction that is wrapped by this object.
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