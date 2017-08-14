using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.Wrappers
{
    internal class ConstructionWrapper
    {
        public Construction Construction { get; set; }

        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; set; }
    }
}
