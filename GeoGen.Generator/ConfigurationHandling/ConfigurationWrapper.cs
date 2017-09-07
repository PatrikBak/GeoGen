using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling
{
    internal class ConfigurationWrapper
    {
        public Configuration Configuration { get; set; }

        public IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> ObjectTypeToObjects { get; set; }

        public IReadOnlyDictionary<int, IArgumentsContainer> ConstructionIdToForbiddenArguments { get; set; }
    }
}