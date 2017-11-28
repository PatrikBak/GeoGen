using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test.Objects
{
    [TestFixture]
    public class ObjectsContainersManagerTest
    {
        private static ObjectsContainersManager Manager(int? numberOfContainers = null)
        {
            var factory = new ObjectsContainersFactory();

            var currentContainer = 0;

            var mock = new Mock<ILooseObjectsConstructor>();
            mock.Setup(s => s.Construct(It.IsAny<IEnumerable<LooseConfigurationObject>>()))
                    .Returns(() => Objects()[currentContainer++]);

            return numberOfContainers == null
                ? new ObjectsContainersManager(factory, mock.Object)
                : new ObjectsContainersManager(factory, mock.Object, numberOfContainers.Value);
        }

        private static List<IAnalyticalObject>[] Objects()
        {
            return new[]
            {
                new List<IAnalyticalObject>
                {
                    new Point(1, 0),
                    new Point(1, 4),
                    new Line(new Point(1, 2), new Point(4, 5)),
                    new Line(new Point(1, 2), new Point(5, 4)),
                    new Circle(new Point(1, 3), 5),
                    new Circle(new Point(2, 4), 4)
                },
                new List<IAnalyticalObject>
                {
                    new Point(2, 1),
                    new Point(3, 2),
                    new Line(new Point(2, 2), new Point(4, 5)),
                    new Line(new Point(3, 2), new Point(5, 4)),
                    new Circle(new Point(1, 7), 5),
                    new Circle(new Point(11, 4), 9)
                }
            };
        }

        [Test]
        public void Test_Factory_Cant_Be_Null()
        {
            var constructor = SimpleMock<ILooseObjectsConstructor>();

            Assert.Throws<ArgumentNullException>(() => new ObjectsContainersManager(null, constructor));
        }

        [Test]
        public void Test_Constructor_Cant_Be_Null()
        {
            var factory = SimpleMock<IObjectsContainersFactory>();

            Assert.Throws<ArgumentNullException>(() => new ObjectsContainersManager(factory, null));
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Test_Number_Of_Containers_Is_At_Least_One(int number)
        {
            var factory = SimpleMock<IObjectsContainersFactory>();
            var constructor = SimpleMock<ILooseObjectsConstructor>();

            Assert.Throws<ArgumentOutOfRangeException>(() => new ObjectsContainersManager(factory, constructor, number));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Manager().Initialize(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Null()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 0},
                null,
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 2}
            };

            Assert.Throws<ArgumentException>(() => Manager().Initialize(objects));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Duplicates()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3}
            };

            Assert.Throws<ArgumentException>(() => Manager().Initialize(objects));
        }

        [Test]
        public void Test_Number_Of_Containers_Is_Correct()
        {
            var manager = Manager();

            Assert.AreEqual(ObjectsContainersManager.DefaultNumberOfContainers, manager.Count());

            manager = Manager(4);

            Assert.AreEqual(4, manager.Count());
        }

        [Test]
        public void Test_Loose_Objects_Are_Correct()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 0},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 5}
            };

            var manager = Manager(2);

            manager.Initialize(objects);

            for (var i = 0; i < 6; i++)
            {
                var containerId = 0;

                foreach (var container in manager)
                {
                    var actual = container.Get(objects[i]);

                    Assert.AreEqual(Objects()[containerId++][i], actual);
                }
            }
        }
    }
}