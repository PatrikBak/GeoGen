using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer
{
    internal interface IContextualContainer : IEnumerable<GeometricalObject>
    {
        /// <summary>
        /// Adds a configuration object to the container. This object must have set its id.
        /// You can't add an object with the same id to the container twice. Also you
        /// can't add an object with a different id that is equal to this object in
        /// the analytical representations of them.
        /// </summary>
        /// <param name="configurationObject">The configuration object.
        /// </param>
        void Add(ConfigurationObject configurationObject);

        IEnumerable<T> GetObjects<T>(ConfigurationObjectsMap objects) where T : GeometricalObject;

        IEnumerable<T> GetNewObjects<T>(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects) where T : GeometricalObject;

        IAnalyticalObject GetAnalyticalObject(GeometricalObject geometricalObject, IObjectsContainer objectsContainer);
    }
}