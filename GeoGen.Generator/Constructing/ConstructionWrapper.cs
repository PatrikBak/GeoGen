using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructing
{
    internal class ConstructionWrapper
    {
        public Core.Constructions.Construction Construction { get; set; }

        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; set; }
    }
}
