using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal class TheoremObjectConstructor : ITheoremObjectConstructor
    {
        public TheoremObject Construct(ConfigurationObjectsMap objects, GeometricalObject geometricalObject)
        {
            if (geometricalObject is PointObject)
                return new TheoremObject(geometricalObject.ConfigurationObject);

            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            var type = line != null
                ? ConfigurationObjectType.Line
                : ConfigurationObjectType.Circle;

            var configurationObject = geometricalObject.ConfigurationObject;

            if (configurationObject != null && objects[type].Contains(configurationObject))
            {
                return new TheoremObject(configurationObject);
            }

            var points = line != null
                ? line.Points
                : circle?.Points ?? throw new AnalyzerException("Unhandled case");

            var involedObjects = points
                    .Select(point => point.ConfigurationObject)
                    .Where(point => objects[ConfigurationObjectType.Point].Contains(point))
                    .ToList();

            var objectType = line != null
                ? TheoremObjectSignature.LineGivenByPoints
                : TheoremObjectSignature.CircleGivenByPoints;

            return new TheoremObject(involedObjects, objectType);
        }
    }
}