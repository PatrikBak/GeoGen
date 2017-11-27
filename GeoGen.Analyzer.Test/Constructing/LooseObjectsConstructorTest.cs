using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.AnalyticalGeometry.RandomObjects;
using GeoGen.Analyzer.Constructing;
using GeoGen.Core.Configurations;
using Moq;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Constructing
{
    [TestFixture]
    public class LooseObjectsConstructorTest
    {
        private static LooseObjectsConstructor Constructor()
        {
            var points = new List<Point>
            {
                new Point(1, 3),
                new Point(1, 1)
            };

            var lines = new List<Line>
            {
                new Line(new Point(1, 3), new Point(2, 4)),
                new Line(new Point(1, 3), new Point(1, 4))
            };

            var circles = new List<Circle>
            {
                new Circle(new Point(1, 4), 5),
                new Circle(new Point(1, 3), 1)
            };

            var currentPoint = 0;
            var currentLine = 0;
            var currentCircle = 0;

            var objectsProvider = new Mock<IRandomObjectsProvider>();

            objectsProvider.Setup(s => s.NextRandomObject<Point>()).Returns(() => points[currentPoint++]);
            objectsProvider.Setup(s => s.NextRandomObject<Line>()).Returns(() => lines[currentLine++]);
            objectsProvider.Setup(s => s.NextRandomObject<Circle>()).Returns(() => circles[currentCircle++]);

            return new LooseObjectsConstructor(objectsProvider.Object);
        }

        [Test]
        public void Test_Random_Objects_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new LooseObjectsConstructor(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Constructor().Construct(null));
        }

        [Test]
        public void Test_Loose_Objects_Cant_Contain_Null()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                null,
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            Assert.Throws<ArgumentException>(() => Constructor().Construct(objects));
        }

        [Test]
        public void Test_Construct_With_Correct_Input()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var randomObjects = Constructor().Construct(objects);

            Assert.AreEqual(randomObjects[0], new Point(1,3));
            Assert.AreEqual(randomObjects[1], new Line(new Point(1,3), new Point(2,4)));
            Assert.AreEqual(randomObjects[2], new Circle(new Point(1,4), 5));
            Assert.AreEqual(randomObjects[3], new Circle(new Point(1,3), 1));
            Assert.AreEqual(randomObjects[4], new Line(new Point(1, 3), new Point(1, 4)));
            Assert.AreEqual(randomObjects[5], new Point(1,1));
        }
    }
}