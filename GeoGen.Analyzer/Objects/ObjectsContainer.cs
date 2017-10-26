using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal sealed class ObjectsContainer : IObjectsContainer
    {
        private readonly Dictionary<IAnalyticalObject, ConfigurationObject> _objectsDictionary;

        private readonly Dictionary<int, IAnalyticalObject> _idToObjects;

        public ObjectsContainer()
        {
            _objectsDictionary = new Dictionary<IAnalyticalObject, ConfigurationObject>();
            _idToObjects = new Dictionary<int, IAnalyticalObject>();
        }

        public ConfigurationObject Add(IAnalyticalObject analyticalObject, ConfigurationObject originalObject)
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

        public T Get<T>(ConfigurationObject configurationObject) where T : IAnalyticalObject
        {
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            return (T) _idToObjects[id];
        }

        public IAnalyticalObject Get(ConfigurationObject configurationObject)
        {
            return Get<IAnalyticalObject>(configurationObject);
        }
    }
}