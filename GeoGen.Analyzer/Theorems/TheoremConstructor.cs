using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal class TheoremConstructor : ITheoremConstructor
    {
        public Theorem Construct(List<GeometricalObject> involvedObjects, ConfigurationObjectsMap allObjects, TheoremType type)
        {
            if (involvedObjects == null)
                throw new ArgumentNullException(nameof(involvedObjects));

            if (allObjects == null)
                throw new ArgumentNullException(nameof(allObjects));

            var theoremObjects = involvedObjects
                    .Select(obj => Construct(allObjects, obj))
                    .ToSet();

            return new Theorem(type, theoremObjects);
        }

        private static TheoremObject Construct(ConfigurationObjectsMap objects, GeometricalObject geometricalObject)
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

            var points = line?.Points ?? circle?.Points ?? throw new AnalyzerException("Unhandled case");

            var pointIds = points
                    .Select(p => p.ConfigurationObject.Id ?? throw new AnalyzerException("Id must be set"))
                    .ToSet();

            var involedObjects = objects[ConfigurationObjectType.Point]
                    .Where(point => pointIds.Contains(point.Id ?? throw new AnalyzerException("Id must be set")))
                    .ToList();

            var objectType = line != null
                ? TheoremObjectSignature.LineGivenByPoints
                : TheoremObjectSignature.CircleGivenByPoints;

            return new TheoremObject(involedObjects, objectType);
        }
    }
}