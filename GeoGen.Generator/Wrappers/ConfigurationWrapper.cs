using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Wrappers
{
    internal class ConfigurationWrapper
    {
        public Configuration Configuration { get; set; }

        public IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> ObjectTypeToObjects { get; set; }
    }
}