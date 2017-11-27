using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using Moq;

namespace GeoGen.Analyzer.Test.Objects.Container
{
    [TestFixture]
    public class ContextualContainerTest
    {
        private static IObjectsContainersHolder _holder;

        private static ContextualContainer Container(params Dictionary<ConfigurationObject, IAnalyticalObject>[] objects)
        {
            var helper = new AnalyticalHelper();

            var containers = objects
                    .Select
                    (
                        dictionary =>
                        {
                            var result = new ObjectsContainer();

                            foreach (var pair in dictionary)
                            {
                                result.Add(pair.Value, pair.Key);
                            }

                            return result;
                        }
                    )
                    .ToList();

            var holder = new Mock<IObjectsContainersHolder>();

            holder.Setup(s => s.GetEnumerator()).Returns(() => containers.GetEnumerator());

            _holder = holder.Object;

            return new ContextualContainer(_holder, helper);
        }

        [Test]
        public void Test_Containers_Holder_Cant_Be_Null()
        {
            var helper = SimpleMock<IAnalyticalHelper>();

            Assert.Throws<ArgumentNullException>(() => new ContextualContainer(null, helper));
        }

        [Test]
        public void Test_Analytical_Helper_Cant_Be_Null()
        {
            var holder = SimpleMock<IObjectsContainersHolder>();

            Assert.Throws<ArgumentNullException>(() => new ContextualContainer(holder, null));
        }

        [Test]
        public void Test_Add_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(null));
        }

        [Test]
        public void Test_Add_Configuration_Object_Must_Have_Id()
        {
            Assert.Throws<AnalyzerException>(() => Container().Add(new LooseConfigurationObject(ConfigurationObjectType.Point)));
        }

        [Test]
        public void Test_Add_Collinear_Points_And_Circle_Passing_Through_One()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 6}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)},
                    {configurationObjects[1], new Point(1, 1)},
                    {configurationObjects[2], new Point(2, 2)},
                    {configurationObjects[3], new Line(new Point(4, 4), new Point(5, 5))},
                    {configurationObjects[4], new Point(3, 3)},
                    {configurationObjects[5], new Circle(new Point(-2, 1), Math.Sqrt(5))}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 1)},
                    {configurationObjects[1], new Point(0, 2)},
                    {configurationObjects[2], new Point(0, 3)},
                    {configurationObjects[3], new Line(new Point(0, 5), new Point(0, 6))},
                    {configurationObjects[4], new Point(0, 4)},
                    {configurationObjects[5], new Circle(new Point(-1, 0), Math.Sqrt(2))}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var objects = container.ToList();

            Assert.AreEqual(6, objects.Count);

            var pointObjects = objects.Where(p => p is PointObject).Cast<PointObject>().ToList();

            Assert.AreEqual(4, pointObjects.Count);
            Assert.IsTrue(pointObjects.All(p => p.Lines.Count == 1));
            Assert.IsTrue(pointObjects.Count(p => p.Circles.Count == 0) == 3);
            Assert.IsTrue(pointObjects.Count(p => p.Circles.Count == 1) == 1);

            var lines = objects.Where(o => o is LineObject).Cast<LineObject>().ToList();

            Assert.AreEqual(1, lines.Count);
            Assert.IsTrue(lines[0].ConfigurationObject == configurationObjects[3]);
            Assert.IsTrue(lines[0].Points.Count == 4);

            var circles = objects.Where(o => o is CircleObject).Cast<CircleObject>().ToList();

            Assert.AreEqual(1, circles.Count);
            Assert.IsTrue(circles[0].Points.Count == 1);
        }

        [Test]
        public void Test_In_Triangle_With_Midpoints_And_Centroid()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)},
                    {configurationObjects[1], new Point(0, 1)},
                    {configurationObjects[2], new Point(3, 5)},
                    {configurationObjects[3], new Point(1.5, 3)},
                    {configurationObjects[4], new Point(1.5, 2.5)},
                    {configurationObjects[5], new Point(0, 0.5)},
                    {configurationObjects[6], new Point(1, 2)}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 2)},
                    {configurationObjects[1], new Point(2, 1)},
                    {configurationObjects[2], new Point(3, 2)},
                    {configurationObjects[3], new Point(2.5, 1.5)},
                    {configurationObjects[4], new Point(1.5, 2)},
                    {configurationObjects[5], new Point(1, 1.5)},
                    {configurationObjects[6], new Point(5.0 / 3, 5.0 / 3)}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var objects = container.ToList();

            var pointObjects = objects.Where(p => p is PointObject).Cast<PointObject>().ToList();

            Assert.AreEqual(7, pointObjects.Count);
            Assert.IsTrue(pointObjects.Count(p => p.Lines.Count == 3) == 4);
            Assert.IsTrue(pointObjects.Count(p => p.Lines.Count == 4) == 3);

            var lines = objects.Where(o => o is LineObject).Cast<LineObject>().ToList();

            Assert.AreEqual(9, lines.Count);
            Assert.IsTrue(lines.All(l => l.ConfigurationObject == null));

            var circles = objects.Where(o => o is CircleObject).Cast<CircleObject>().ToList();

            Assert.AreEqual(29, circles.Count);
            Assert.IsTrue(circles.All(c => c.ConfigurationObject == null));
            Assert.IsTrue(circles.All(c => c.Points.Count == 3));
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Feets_Of_Altitudes()
        {
            var configurationObjects = new List<ConfigurationObject>
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

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 5)},
                    {configurationObjects[1], new Point(-2, 1)},
                    {configurationObjects[2], new Point(6, 1)},
                    {configurationObjects[3], new Point(2, 1)},
                    {configurationObjects[4], new Point(3, 3)},
                    {configurationObjects[5], new Point(-1, 3)},
                    {configurationObjects[6], new Point(0, 1)},
                    {configurationObjects[7], new Point(6.0 / 13, 61.0 / 13)},
                    {configurationObjects[8], new Point(-0.4, 4.2)}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 3)},
                    {configurationObjects[1], new Point(-2, 1)},
                    {configurationObjects[2], new Point(6, 1)},
                    {configurationObjects[3], new Point(2, 1)},
                    {configurationObjects[4], new Point(3, 2)},
                    {configurationObjects[5], new Point(-1, 2)},
                    {configurationObjects[6], new Point(0, 1)},
                    {configurationObjects[7], new Point(-1.2, 3.4)},
                    {configurationObjects[8], new Point(2, 5)}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var objects = container.ToList();

            var circles = objects.Where(o => o is CircleObject).Cast<CircleObject>().ToList();

            Assert.IsTrue(circles.Count(c => c.Points.Count == 6) == 1);
            Assert.IsTrue(circles.Count(c => c.Points.Count == 4) == 3);
        }

        [Test]
        public void Test_Get_Analytical_Object_Container_Is_Not_Null()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);
            container.Add(configurationObjects[0]);

            var geometricalObject = container.First();

            Assert.Throws<ArgumentNullException>(() => container.GetAnalyticalObject(geometricalObject, null));
        }

        [Test]
        public void Test_Get_Analytical_Object_Container_Must_Exist()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            container.Add(configurationObjects[0]);

            var geometricalObject = container.First();

            var mock = SimpleMock<IObjectsContainer>();

            Assert.Throws<ArgumentException>(() => container.GetAnalyticalObject(geometricalObject, mock));
        }

        [Test]
        public void Test_Get_Analytical_Object_Geometrical_Object_Is_Not_Null()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            Assert.Throws<ArgumentNullException>(() => container.GetAnalyticalObject(null, _holder.First()));
        }

        [Test]
        public void Test_Get_Analytical_Object_Geometrical_Object_Must_Be_Present()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            var geometricalObject = new PointObject(configurationObjects[0], 100);

            Assert.Throws<ArgumentException>(() => container.GetAnalyticalObject(geometricalObject, _holder.First()));
        }

        [Test]
        public void Test_Get_Analytical_Object_With_Correct_Input()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            container.Add(configurationObjects[0]);

            var geometricalObject = container.First();

            var analyticalObject = container.GetAnalyticalObject(geometricalObject, _holder.First());

            Assert.AreEqual(new Point(0, 0), analyticalObject);
        }

        [Test]
        public void Test_Get_Objects_Map_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().GetObjects<LineObject>(null).ToList());
            Assert.Throws<ArgumentNullException>(() => Container().GetObjects<PointObject>(null).ToList());
            Assert.Throws<ArgumentNullException>(() => Container().GetObjects<CircleObject>(null).ToList());
        }

        [Test]
        public void Test_Get_Objects_Simple_Test_With_Triangle()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)},
                    {configurationObjects[1], new Point(0, 1)},
                    {configurationObjects[2], new Point(3, 5)}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var objectsMap = new ConfigurationObjectsMap(configurationObjects);

            var lines = container.GetObjects<LineObject>(objectsMap).ToList();
            var circles = container.GetObjects<CircleObject>(objectsMap).ToList();
            var points = container.GetObjects<PointObject>(objectsMap).ToList();

            Assert.AreEqual(3, lines.Count);
            Assert.AreEqual(1, circles.Count);
            Assert.AreEqual(3, points.Count);
        }

        [Test]
        public void Test_Get_Objects_Complex_Test()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 8},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 9},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 10},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 11},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 12},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 13}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 5)},
                    {configurationObjects[1], new Point(-2, 1)},
                    {configurationObjects[2], new Point(6, 1)},
                    {configurationObjects[3], new Point(2, 1)},
                    {configurationObjects[4], new Point(3, 3)},
                    {configurationObjects[5], new Point(-1, 3)},
                    {configurationObjects[6], new Point(0, 1)},
                    {configurationObjects[7], new Point(6.0 / 13, 61.0 / 13)},
                    {configurationObjects[8], new Point(-0.4, 4.2)},
                    {configurationObjects[9], new Line(new Point(0, 5), new Point(0, 1))},
                    {configurationObjects[10], new Line(new Point(3, 3), new Point(-1, 3))},
                    {configurationObjects[11], new Circle(new Point(2, 1), new Point(-1, 3), new Point(3, 3))},
                    {configurationObjects[12], new Circle(new Point(1, 1), 2)}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var involvedObjects = new List<ConfigurationObject>
            {
                configurationObjects[1],
                configurationObjects[3],
                configurationObjects[4],
                configurationObjects[5],
                configurationObjects[6],
                configurationObjects[9],
                configurationObjects[10],
                configurationObjects[11],
                configurationObjects[12]
            };

            var objectsMap = new ConfigurationObjectsMap(involvedObjects);

            var lines = container.GetObjects<LineObject>(objectsMap).ToList();
            var circles = container.GetObjects<CircleObject>(objectsMap).ToList();
            var points = container.GetObjects<PointObject>(objectsMap).ToList();

            Assert.AreEqual(9, lines.Count);
            Assert.AreEqual(7, circles.Count);
            Assert.AreEqual(5, points.Count);
        }

        [Test]
        public void Test_Get_New_Objects_New_Objects_Cant_Be_Null()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            var oldObjects = new ConfigurationObjectsMap(configurationObjects);

            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<PointObject>(oldObjects, null).ToList());
            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<LineObject>(oldObjects, null).ToList());
            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<CircleObject>(oldObjects, null).ToList());
        }

        [Test]
        public void Test_Get_New_Objects_Old_Objects_Cant_Be_Null()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)}
                }
            };

            var container = Container(dictionaries);

            var newObjects = new ConfigurationObjectsMap(configurationObjects);

            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<PointObject>(null, newObjects).ToList());
            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<LineObject>(null, newObjects).ToList());
            Assert.Throws<ArgumentNullException>(() => container.GetNewObjects<CircleObject>(null, newObjects).ToList());
        }

        [Test]
        public void Test_Get_New_Objects_With_Triangle_And_Midpoints()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 0)},
                    {configurationObjects[1], new Point(0, 1)},
                    {configurationObjects[2], new Point(3, 5)},
                    {configurationObjects[3], new Point(0, 0.5)},
                    {configurationObjects[4], new Point(1.5, 2.5)},
                    {configurationObjects[5], new Point(1.5, 3)}
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var oldObjects = new List<ConfigurationObject>
            {
                configurationObjects[0],
                configurationObjects[1],
                configurationObjects[2],
                configurationObjects[3],
                configurationObjects[4]
            };

            var oldMap = new ConfigurationObjectsMap(oldObjects);

            var newObjects = new List<ConfigurationObject>
            {
                configurationObjects[5]
            };

            var newMap = new ConfigurationObjectsMap(newObjects);

            var points = container.GetNewObjects<PointObject>(oldMap, newMap).ToList();
            var lines = container.GetNewObjects<LineObject>(oldMap, newMap).ToList();
            var circles = container.GetNewObjects<CircleObject>(oldMap, newMap).ToList();

            Assert.AreEqual(1, points.Count);
            Assert.AreEqual(points[0].ConfigurationObject, newObjects[0]);

            Assert.AreEqual(3, lines.Count);
            Assert.IsTrue(lines.All(l => l.Points.Any(p => p.ConfigurationObject == newObjects[0])));

            Assert.AreEqual(9, circles.Count);
            Assert.IsTrue(circles.All(c => c.Points.Any(p => p.ConfigurationObject == newObjects[0])));
        }

        [Test]
        public void Test_Get_New_Objects_Complex_Test()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 8},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 9},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 10},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 11},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 12},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 13}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 5)}, // A
                    {configurationObjects[1], new Point(-2, 1)}, // B
                    {configurationObjects[2], new Point(6, 1)}, // C
                    {configurationObjects[3], new Point(-1, 3)}, // D
                    {configurationObjects[4], new Point(3, 3)}, // E 
                    {configurationObjects[5], new Point(2, 1)}, // F
                    {configurationObjects[6], new Point(0, 1)}, // G
                    {configurationObjects[7], new Point(6.0 / 13, 61.0 / 13)}, // H
                    {configurationObjects[8], new Point(-0.4, 4.2)}, // I
                    {configurationObjects[9], new Circle(new Point(-1, 3), new Point(3, 3), new Point(-0.4, 4.2))}, // DEI
                    {configurationObjects[10], new Circle(new Point(-2, 1), new Point(3, 3), new Point(-0.4, 4.2))}, // BEI
                    {configurationObjects[11], new Line(new Point(-1, 3), new Point(0, 1))}, // DG
                    {configurationObjects[12], new Line(new Point(-2, 1), new Point(6, 1))} // BC
                }
            };

            var container = Container(dictionaries);

            foreach (var configurationObject in configurationObjects)
            {
                container.Add(configurationObject);
            }

            var oldObjects = new ConfigurationObjectsMap
            (
                new List<ConfigurationObject>
                {
                    configurationObjects[0], // A
                    configurationObjects[1], // B
                    configurationObjects[2], // C
                    configurationObjects[6], // G
                    configurationObjects[5], // F
                    configurationObjects[8], // I
                    configurationObjects[11] // DG
                }
            );

            var newObjects = new ConfigurationObjectsMap
            (
                new List<ConfigurationObject>
                {
                    configurationObjects[3], // D
                    configurationObjects[4], // E
                    configurationObjects[12], // BC
                    configurationObjects[9], // DEI
                    configurationObjects[10] // BEI
                }
            );

            var points = container.GetNewObjects<PointObject>(oldObjects, newObjects).ToList();
            var lines = container.GetNewObjects<LineObject>(oldObjects, newObjects).ToList();
            var circles = container.GetNewObjects<CircleObject>(oldObjects, newObjects).ToList();

            Assert.AreEqual(2, points.Count);
            Assert.AreEqual(7, lines.Count);
            Assert.AreEqual(23, circles.Count);
        }
    }
}