using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    internal interface IConfigurationObjectsIterator
    {
        ConfigurationObject Next(ConfigurationObjectType type);

        void Initialize(IReadOnlyDictionary<ConfigurationObjectType, IEnumerable<ConfigurationObject>> objectTypeToObjects);
    }
}
