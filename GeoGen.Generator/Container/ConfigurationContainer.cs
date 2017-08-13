using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.Constructor;

namespace GeoGen.Generator.Container
{
    internal class ConfigurationContainer : IConfigurationContainer
    {
        private readonly List<Configuration> _configurations = new List<Configuration>();

        public ConfigurationContainer(Configuration initialConfiguration)
        {
            _configurations.Add(initialConfiguration);
        }

        public void AddLayer(List<ConstructorOutput> newLayerConfigurations)
        {
            _configurations.SetItems(newLayerConfigurations.Select(CreateConfiguration));
        }

        public Dictionary<ConfigurationObjectType, List<ConfigurationObject>> GetObjectTypeToObjectsMap(Configuration configuration)
        {
            // TODO: Caching. This gets called too many times for one configuratio.

            var allObjects = configuration.LooseObjects.Cast<ConfigurationObject>().Union(configuration.ConstructedObjects);
            var result = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>();

            foreach (var configurationObject in allObjects)
            {
                var type = configurationObject.ObjectType;

                if (!result.ContainsKey(type))
                    result.Add(type, new List<ConfigurationObject>());

                result[type].Add(configurationObject);
            }

            return result;
        }

        private static Configuration CreateConfiguration(ConstructorOutput constructorOutput)
        {
            var parentConfiguration = constructorOutput.InitialConfiguration;
            var constructedObject = constructorOutput.ConstructedObject;
            var constructedObjects = parentConfiguration.ConstructedObjects.Union(constructedObject.SingleItemAsEnumerable());
            var constructedObjectsSet = new HashSet<ConstructedConfigurationObject>(constructedObjects);

            return new Configuration(parentConfiguration.LooseObjects, constructedObjectsSet);
        }

        #region IEnumerable methods

        public IEnumerator<Configuration> GetEnumerator()
        {
            return _configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}