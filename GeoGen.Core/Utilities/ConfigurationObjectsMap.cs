using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Utilities
{
    public class ConfigurationObjectsMap : Dictionary<ConfigurationObjectType, List<ConfigurationObject>>
    {
        public ConfigurationObjectsMap()
        {
        }

        public ConfigurationObjectsMap(IDictionary<ConfigurationObjectType, List<ConfigurationObject>> objects)
            : base(objects)
        {
        }

        public ConfigurationObjectsMap(IEnumerable<ConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            AddAll(objects);
        }

        public ConfigurationObjectsMap(Configuration configuraton)
        {
            if (configuraton == null)
                throw new ArgumentNullException(nameof(configuraton));

            var objects = configuraton.LooseObjects
                    .Cast<ConfigurationObject>()
                    .Union(configuraton.ConstructedObjects);

            AddAll(objects);
        }

        public void AddAll(IEnumerable<ConfigurationObject> objects,
                           Func<ConfigurationObject, ConfigurationObject> selector = null)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            foreach (var configurationObject in objects)
            {
                var newObject = selector == null ? configurationObject : selector(configurationObject);
                var type = newObject?.ObjectType ?? throw new ArgumentException("Null object");

                List<ConfigurationObject> list;

                if (!ContainsKey(type))
                {
                    list = new List<ConfigurationObject>();
                    Add(type, list);
                }

                list = this[type];
                list.Add(newObject);
            }
        }

        public void AddAll(ConfigurationObjectsMap map, Func<ConfigurationObject, ConfigurationObject> selector = null)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            AddAll(map.SelectMany(o => o.Value), selector);
        }
    }
}