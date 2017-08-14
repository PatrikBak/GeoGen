using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    internal class ConfigurationsObjectIterator : IConfigurationObjectsIterator
    {
        private IReadOnlyDictionary<ConfigurationObjectType, IEnumerator<ConfigurationObject>> _objectTypeToObjects;

        public void Initialize(IReadOnlyDictionary<ConfigurationObjectType, IEnumerable<ConfigurationObject>> objectTypeToObjects)
        {
            _objectTypeToObjects = objectTypeToObjects.ToDictionary
            (
                keyValue => keyValue.Key,
                keyValue => keyValue.Value.GetEnumerator()
            );
        }

        public ConfigurationObject Next(ConfigurationObjectType type)
        {
            var enumerator = _objectTypeToObjects[type];

            if (!enumerator.MoveNext())
                throw new Exception("Incorrect call");

            return enumerator.Current;
        }
    }
}