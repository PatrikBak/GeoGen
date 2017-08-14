using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Container
{
    internal class ConfigurationContainer : IConfigurationContainer
    {
        private readonly List<ConfigurationWrapper> _configurations = new List<ConfigurationWrapper>();

        public void Initialize(Configuration initialConfiguration)
        {
            _configurations.Clear();
        }

        public void AddLayer(List<ConstructorOutput> newLayerConfigurations)
        {
            _configurations.SetItems(newLayerConfigurations.Select(CreateConfiguration));
        }

        //public Dictionary<ConfigurationObjectType, List<ConfigurationObject>> GetObjectTypeToObjectsMap(Configuration configuration)
        //{
        //    // TODO: Caching. This gets called too many times for one configuratio.

        //    var allObjects = configuration.LooseObjects.Cast<ConfigurationObject>().Union(configuration.ConstructedObjects);
        //    var result = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>();

        //    foreach (var configurationObject in allObjects)
        //    {
        //        var type = configurationObject.ObjectType;

        //        if (!result.ContainsKey(type))
        //            result.Add(type, new List<ConfigurationObject>());

        //        result[type].Add(configurationObject);
        //    }

        //    return result;
        //}

        private static ConfigurationWrapper CreateConfiguration(ConstructorOutput constructorOutput)
        {
            var parentConfiguration = constructorOutput.InitialConfiguration.Configuration;
            var constructedObject = constructorOutput.ConstructedObject;
            var constructedObjects = parentConfiguration.ConstructedObjects.Union(constructedObject.SingleItemAsEnumerable());
            var constructedObjectsSet = new HashSet<ConstructedConfigurationObject>(constructedObjects);
            var configuration = new Configuration(parentConfiguration.LooseObjects, constructedObjectsSet);

            return new ConfigurationWrapper {Configuration = configuration};
        }

        #region IEnumerable methods

        public IEnumerator<ConfigurationWrapper> GetEnumerator()
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