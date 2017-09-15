using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Utilities
{
    public class ConfigurationObjectsMap : Dictionary<ConfigurationObjectType, List<ConfigurationObject>>
    {
        private readonly List<ConfigurationObject> _allObjects = new List<ConfigurationObject>();

        public IEnumerable<ConfigurationObject> Objects => _allObjects;

        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> configurationObjects)
        {
            AddAll(configurationObjects);
        }

        public ConfigurationObjectsMap(Configuration configuraton)
        {
            var objects = configuraton.LooseObjects
                    .Cast<ConfigurationObject>()
                    .Union(configuraton.ConstructedObjects);

            AddAll(objects);
        }

        public ConfigurationObjectsMap CloneWithNewObjects(List<ConstructedConfigurationObject> newObjects)
        {
            var result = new ConfigurationObjectsMap(Enumerable.Empty<ConfigurationObject>());

            result.AddAll(_allObjects);
            result.AddAll(newObjects);

            return result;
        }

        private void AddAll(IEnumerable<ConfigurationObject> objects)
        {
            foreach (var configurationObject in objects)
            {
                var type = configurationObject.ObjectType;

                if (!ContainsKey(type))
                    Add(type, new List<ConfigurationObject>());

                this[type].Add(configurationObject);
                _allObjects.Add(configurationObject);
            }
        }
    }
}