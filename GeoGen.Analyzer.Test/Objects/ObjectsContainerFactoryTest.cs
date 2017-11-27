using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Constructing;
using NUnit.Framework;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using Moq;

namespace GeoGen.Analyzer.Test.Objects
{
    [TestFixture]
    public class ObjectsContainerFactoryTest
    {
        private static ObjectsContainersFactory Factory()
        {
            var objects = Objects();

            IAnalyticalObject Construct(LooseConfigurationObject obj)
            {
                var id = obj.Id ?? throw new Exception();

                return objects[id];
            }

            var mock = new Mock<ILooseObjectsConstructor>();
            mock.Setup(s => s.Construct(It.IsAny<IEnumerable<LooseConfigurationObject>>()))
                    .Returns<IEnumerable<LooseConfigurationObject>>(objs => objs.Select(Construct).ToList());

            return new ObjectsContainersFactory(mock.Object);
        }

        private static List<IAnalyticalObject> Objects()
        {
            return new List<IAnalyticalObject>
            {
                new Point(1, 0),
                new Point(1, 4),
                new Line(new Point(1, 2), new Point(4, 5)),
                new Line(new Point(1, 2), new Point(5, 4)),
                new Circle(new Point(1, 3), 5),
                new Circle(new Point(2, 4), 4)
            };
        }

        [Test]
        public void Test_Constructor_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectsContainersFactory(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().CreateContainer(null));
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

            Assert.Throws<ArgumentException>(() => Factory().CreateContainer(objects));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Duplicate_Objects()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 3}
            };

            Assert.Throws<ArgumentException>(() => Factory().CreateContainer(objects));
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

            var container = Factory().CreateContainer(objects);

            Assert.NotNull(container);

            for (var i = 0; i < 6; i++)
            {
                var expected = Objects()[i];
                var actual = container.Get(objects[i]);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}