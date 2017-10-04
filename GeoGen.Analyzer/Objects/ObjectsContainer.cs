using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class ObjectsContainer : IObjectsContainer
    {
        private readonly Dictionary<GeometricalObject, GeometricalObject> _objectsDictionary;

        private readonly Dictionary<int, GeometricalObject> _idToObjects;

        public ObjectsContainer()
        {
            _objectsDictionary = new Dictionary<GeometricalObject, GeometricalObject>();
            _idToObjects = new Dictionary<int, GeometricalObject>();
        }

        public GeometricalObject Add(GeometricalObject geometricalObject)
        {
            if (_objectsDictionary.ContainsKey(geometricalObject))
                return _objectsDictionary[geometricalObject];

            _objectsDictionary.Add(geometricalObject, geometricalObject);

            var id = geometricalObject.ConfigurationObject.Id ?? throw new AnalyzerException("Id must be set");
            _idToObjects.Add(id, geometricalObject);

            return geometricalObject;
        }

        public void Remove(int id)
        {
            if (!_idToObjects.ContainsKey(id))
                return;

            _objectsDictionary.Remove(_idToObjects[id]);
            _idToObjects.Remove(id);
        }

        public T Get<T>(ConfigurationObject obj1) where T : GeometricalObject
        {
            var id = obj1.Id ?? throw new AnalyzerException("Id must be set.");

            return (T) _idToObjects[id];
        }
    }
}