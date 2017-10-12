using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class ObjectsContainer : IObjectsContainer
    {
        private readonly Dictionary<AnalyticalObject, ConfigurationObject> _objectsDictionary;

        private readonly Dictionary<int, AnalyticalObject> _idToObjects;

        public ObjectsContainer()
        {
            _objectsDictionary = new Dictionary<AnalyticalObject, ConfigurationObject>();
            _idToObjects = new Dictionary<int, AnalyticalObject>();
        }

        public ConfigurationObject Add(AnalyticalObject analyticalObject, ConfigurationObject originalObject)
        {
            if (_objectsDictionary.ContainsKey(analyticalObject))
                return _objectsDictionary[analyticalObject];

            _objectsDictionary.Add(analyticalObject, originalObject);

            var id = originalObject.Id ?? throw new AnalyzerException("Id must be set");
            _idToObjects.Add(id, analyticalObject);

            return originalObject;
        }

        public void Remove(int id)
        {
            if (!_idToObjects.ContainsKey(id))
                return;

            _objectsDictionary.Remove(_idToObjects[id]);
            _idToObjects.Remove(id);
        }

        public T Get<T>(ConfigurationObject configurationObject) where T : AnalyticalObject
        {
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            return (T) _idToObjects[id];
        }

        public AnalyticalObject Get(ConfigurationObject configurationObject)
        {
            return Get<AnalyticalObject>(configurationObject);
        }
    }
}