using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Theorems
{
    [TestFixture]
    public class TheoremConstructorTest
    {
        private static TheoremConstructor Constructor() => new TheoremConstructor();

        [Test]
        public void Test_Involved_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Constructor().Construct(null, new ConfigurationObjectsMap(), TheoremType.CollinearPoints));
        }

        [Test]
        public void Test_All_Objects_Map_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Constructor().Construct(new List<GeometricalObject>(), null, TheoremType.CollinearPoints));
        }

        [Test]
        public void Test_With_Explicit_Objects()
        {
            var line = new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 1};
            var circle = new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 2};
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};

            var geometricalObjects = new List<GeometricalObject>
            {
                new LineObject(line, 1),
                new CircleObject(circle, 2),
                new PointObject(point, 3)
            };

            var allObjects = new ConfigurationObjectsMap(line.SingleItemAsEnumerable().ConcatItem(circle).ConcatItem(point));

            var result = Constructor().Construct(geometricalObjects, allObjects, TheoremType.ConcurrentObjects);

            Assert.AreEqual(TheoremType.ConcurrentObjects, result.Type);

            var internalObjects = result.InvolvedObjects;

            Assert.AreEqual(3, result.InvolvedObjects.Count);
            Assert.IsTrue(internalObjects.All(o => o.Type == TheoremObjectSignature.SingleObject));
            Assert.IsTrue(internalObjects.All(o => o.InternalObjects.Count == 1));
            Assert.IsTrue(internalObjects.Any(o => o.InternalObjects.Contains(line)));
            Assert.IsTrue(internalObjects.Any(o => o.InternalObjects.Contains(point)));
            Assert.IsTrue(internalObjects.Any(o => o.InternalObjects.Contains(circle)));
        }

        [Test]
        public void Test_With_Implicitly_Defined_Line_And_Circles()
        {
            var point1 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var point2 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2};
            var point3 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};
            var point4 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4};

            var points = new List<PointObject>
            {
                new PointObject(point1, 1),
                new PointObject(point2, 2),
                new PointObject(point3, 3),
                new PointObject(point4, 4)
            };

            var linesAndCircles = new List<GeometricalObject>
            {
                new LineObject(5, points[0], points[1]),
                new LineObject(6, points[0], points[1], points[2]),
                new CircleObject(7, points[1], points[2], points[3])
            };

            var allObjects = new ConfigurationObjectsMap(points.Select(p => p.ConfigurationObject));

            var result = Constructor().Construct(linesAndCircles, allObjects, TheoremType.ConcyclicPoints);

            Assert.AreEqual(TheoremType.ConcyclicPoints, result.Type);

            var internalObjects = result.InvolvedObjects;

            Assert.AreEqual(3, result.InvolvedObjects.Count);
            Assert.IsTrue(internalObjects.Count(o => o.Type == TheoremObjectSignature.CircleGivenByPoints) == 1);
            Assert.IsTrue(internalObjects.Count(o => o.Type == TheoremObjectSignature.LineGivenByPoints) == 2);

            Assert.IsTrue(internalObjects.Count(o => o.InternalObjects.Count == 2) == 1);
            Assert.IsTrue(internalObjects.Count(o => o.InternalObjects.Count == 3) == 2);

            bool ContainsAll(params ConfigurationObject[] objects)
            {
                return internalObjects.Any(o => objects.All(o.InternalObjects.Contains));
            }

            Assert.IsTrue(ContainsAll(point1, point2));
            Assert.IsTrue(ContainsAll(point1, point2, point3));
            Assert.IsTrue(ContainsAll(point2, point3, point4));
        }

        [Test]
        public void Test_With_Complex_Input()
        {
            var point1 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var point2 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2};
            var point3 = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3};
            var line = new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 4};

            var points = new List<PointObject>
            {
                new PointObject(point1, 1),
                new PointObject(point2, 2),
                new PointObject(point3, 3),
            };

            var linesAndCircles = new List<GeometricalObject>
            {
                new LineObject(4, points[0], points[1]),
                new CircleObject(5, points[0], points[1], points[2])
            };

            var geometricalObjects = new List<GeometricalObject>
            {
                points[1],
                linesAndCircles[0],
                linesAndCircles[1]
            };

            var configurationObjects = new List<ConfigurationObject>
            {
                point1,
                point2,
                point3,
                line
            };
            var allObjects = new ConfigurationObjectsMap(configurationObjects);

            var result = Constructor().Construct(geometricalObjects, allObjects, TheoremType.CollinearPoints);

            Assert.AreEqual(TheoremType.CollinearPoints, result.Type);

            Assert.AreEqual(3, result.InvolvedObjects.Count);

            var singleObjects = result
                    .InvolvedObjects
                    .Where(obj => obj.Type == TheoremObjectSignature.SingleObject)
                    .ToList();

            Assert.AreEqual(1, singleObjects.Count);
            Assert.AreEqual(1, singleObjects[0].InternalObjects.Count);
            Assert.AreEqual(singleObjects[0].InternalObjects.First(), point2);

            var circlesByPoints = result
                    .InvolvedObjects
                    .Where(obj => obj.Type == TheoremObjectSignature.CircleGivenByPoints)
                    .ToList();

            Assert.AreEqual(1, circlesByPoints.Count);
            Assert.AreEqual(3, circlesByPoints[0].InternalObjects.Count);
            Assert.IsTrue(circlesByPoints[0].InternalObjects.ToSet().SetEquals(new []{point2, point3, point1}));
            
            var linesByPoints = result
                    .InvolvedObjects
                    .Where(obj => obj.Type == TheoremObjectSignature.LineGivenByPoints)
                    .ToList();

            Assert.AreEqual(1, linesByPoints.Count);
            Assert.AreEqual(2, linesByPoints[0].InternalObjects.Count);
            Assert.IsTrue(linesByPoints[0].InternalObjects.ToSet().SetEquals(new[] { point2, point1 }));
        }
    }
}