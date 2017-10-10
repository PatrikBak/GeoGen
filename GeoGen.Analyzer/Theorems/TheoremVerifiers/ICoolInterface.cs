using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Objects;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal interface ICoolInterface
    {
        IEnumerable<T> GetNewObjects<T>(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);

        IEnumerable<T> GetObjects<T>(ConfigurationObjectsMap objects);

        void Add(ConfigurationObject configurationObject);

        AnalyticalObject GetAnalyticalObject(GeometricalObject geometricalObject, IObjectsContainer objectsContainer);

        bool IsContained(AnalyticalObject analyticalObject, ConfigurationObjectsMap objects);

        List<ConfigurationObject> GetContainedObjects(ConfigurationObjectsMap allObjects, GeometricalObject geometricalObject);
    }
}