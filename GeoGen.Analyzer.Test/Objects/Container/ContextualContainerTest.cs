using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using Moq;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Objects.Container
{
    [TestFixture]
    public class ContextualContainerTest
    {
        private static ContextualContainer Container(Configuration configuration, params List<AnalyticalObject>[] objects)
        {
            var helper = new AnalyticalHelper();

            var containers = objects.Select(analyticalObjects =>
            {
                var result = new ObjectsContainer();

                var allObjects = configuration.ObjectsMap.AllObjects;

                for (var i = 0; i < allObjects.Count; i++)
                {
                    var iCopy = i;
                    result.Add(new[] {allObjects[i]}, c => new List<AnalyticalObject> {analyticalObjects[iCopy]});
                }

                return result;
            }).ToList();

            var manager = new Mock<IObjectsContainersManager>();

            manager.Setup(s => s.GetEnumerator()).Returns(() => containers.GetEnumerator());

            return new ContextualContainer(configuration, manager.Object, helper);
        }

        [Test]
        public void Test_Plain_Triangle()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            var analyticalObjects = new[]
            {
                new List<AnalyticalObject>
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 6)
                },
                new List<AnalyticalObject>
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 5)
                }
            };

            var container = Container(configuration, analyticalObjects);

            var lines = container.GetGeometricalObjects<LineObject>().ToList();
            var circles = container.GetGeometricalObjects<CircleObject>().ToList();
            var points = container.GetGeometricalObjects<PointObject>().ToList();

            Assert.AreEqual(3, lines.Count);
            Assert.AreEqual(1, circles.Count);
            Assert.AreEqual(3, points.Count);
        }

        [Test]
        public void Test_Collinear_Points_And_Circle_Passing_Through_One()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 6}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            var analyticalObjects = new[]
            {
                new List<AnalyticalObject>
                {
                    new Point(0, 0),
                    new Point(1, 1),
                    new Point(2, 2),
                    new Line(new Point(4, 4), new Point(5, 5)),
                    new Point(3, 3),
                    new Circle(new Point(-2, 1), Math.Sqrt(5))
                },
                new List<AnalyticalObject>
                {
                    new Point(0, 1),
                    new Point(0, 2),
                    new Point(0, 3),
                    new Line(new Point(0, 5), new Point(0, 6)),
                    new Point(0, 4),
                    new Circle(new Point(-1, 0), Math.Sqrt(2))
                }
            };

            var container = Container(configuration, analyticalObjects);

            var points = container.GetGeometricalObjects<PointObject>().ToList();

            Assert.AreEqual(4, points.Count);
            Assert.IsTrue(points.All(p => p.Lines.Count == 1));
            Assert.IsTrue(points.Count(p => p.Circles.Count == 0) == 3);
            Assert.IsTrue(points.Count(p => p.Circles.Count == 1) == 1);

            var lines = container.GetGeometricalObjects<LineObject>().ToList();

            Assert.AreEqual(1, lines.Count);
            Assert.IsTrue(lines[0].ConfigurationObject == looseObjects[3]);
            Assert.IsTrue(lines[0].Points.Count == 4);

            var circles = container.GetGeometricalObjects<CircleObject>().ToList();

            Assert.AreEqual(1, circles.Count);
            Assert.IsTrue(circles[0].Points.Count == 1);
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Centroid()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            var analyticalObjects = new[]
            {
                new List<AnalyticalObject>
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 5),
                    new Point(1.5, 3),
                    new Point(1.5, 2.5),
                    new Point(0, 0.5),
                    new Point(1, 2)
                },
                new List<AnalyticalObject>
                {
                    new Point(0, 2),
                    new Point(2, 1),
                    new Point(3, 2),
                    new Point(2.5, 1.5),
                    new Point(1.5, 2),
                    new Point(1, 1.5),
                    new Point(5.0 / 3, 5.0 / 3)
                }
            };

            var container = Container(configuration, analyticalObjects);

            var points = container.GetGeometricalObjects<PointObject>().ToList();

            Assert.AreEqual(7, points.Count);
            Assert.IsTrue(points.Count(p => p.Lines.Count == 3) == 4);
            Assert.IsTrue(points.Count(p => p.Lines.Count == 4) == 3);

            var lines = container.GetGeometricalObjects<LineObject>().ToList();

            Assert.AreEqual(9, lines.Count);
            Assert.IsTrue(lines.All(l => l.ConfigurationObject == null));

            var circles = container.GetGeometricalObjects<CircleObject>().ToList();

            Assert.AreEqual(29, circles.Count);
            Assert.IsTrue(circles.All(c => c.ConfigurationObject == null));
            Assert.IsTrue(circles.All(c => c.Points.Count == 3));
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Feets_Of_Altitudes()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 8},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 9}
            };

            var configuration = new Configuration(looseObjects, new List<ConstructedConfigurationObject>());

            var analyticalObjects = new[]
            {
                new List<AnalyticalObject>
                {
                    new Point(0, 5),
                    new Point(-2, 1),
                    new Point(6, 1),
                    new Point(2, 1),
                    new Point(3, 3),
                    new Point(-1, 3),
                    new Point(0, 1),
                    new Point(6.0 / 13, 61.0 / 13),
                    new Point(-0.4, 4.2)
                },
                new List<AnalyticalObject>
                {
                    new Point(0, 3),
                    new Point(-2, 1),
                    new Point(6, 1),
                    new Point(2, 1),
                    new Point(3, 2),
                    new Point(-1, 2),
                    new Point(0, 1),
                    new Point(-1.2, 3.4),
                    new Point(2, 5)
                }
            };

            var container = Container(configuration, analyticalObjects);

            var circles = container.GetGeometricalObjects<CircleObject>().ToList();

            Assert.IsTrue(circles.Count(c => c.Points.Count == 6) == 1);
            Assert.IsTrue(circles.Count(c => c.Points.Count == 4) == 3);
        }
    }
}