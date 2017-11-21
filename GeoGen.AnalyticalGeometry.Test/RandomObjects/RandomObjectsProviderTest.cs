using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using NUnit.Framework;
using GeoGen.AnalyticalGeometry.RandomObjects;
using Moq;
using static GeoGen.AnalyticalGeometry.RandomObjects.RandomObjectsProvider;

namespace GeoGen.AnalyticalGeometry.Test.RandomObjects
{
    [TestFixture]
    public class RandomObjectsProviderTest
    {
        private static RandomObjectsProvider Provider(IReadOnlyList<int> numbers)
        {
            var mock = new Mock<IRandomnessProvider>();

            var callIndex = 0;

            mock.Setup(provider => provider.NextDouble(It.IsAny<double>()))
                    .Returns(() => numbers[callIndex++ % numbers.Count]);

            return new RandomObjectsProvider(mock.Object);
        }

        [Test]
        public void Test_Randomness_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new RandomObjectsProvider(null));
        }

        [Test]
        public void Test_Generating_Distint_Points()
        {
            var list = new List<int> {1, 1, 1, 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 2, 2};

            var provider = Provider(list);

            Assert.AreEqual(new Point(1, 1), provider.NextRandomObject<Point>());
            Assert.AreEqual(new Point(1, 2), provider.NextRandomObject<Point>());
            Assert.AreEqual(new Point(3, 1), provider.NextRandomObject<Point>());
            Assert.AreEqual(new Point(2, 2), provider.NextRandomObject<Point>());
        }

        [Test]
        public void Test_Generating_Distint_Lines()
        {
            var list = new List<int> {1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 3, 4, 5};

            var provider = Provider(list);

            Assert.AreEqual(new Line(new Point(1, 1), new Point(1, 2)), provider.NextRandomObject<Line>());
            Assert.AreEqual(new Line(new Point(4, 5), new Point(2, 3)), provider.NextRandomObject<Line>());
        }

        [Test]
        public void Test_Generating_Distint_Circles()
        {
            var list = new List<int> {1, 1, 5, 1, 1, 5, 2, 2, 3, 1, 1, 5, 3, 4, 2};

            var provider = Provider(list);

            Assert.AreEqual(new Circle(new Point(1, 1), MaximalRandomValue - 5), provider.NextRandomObject<Circle>());
            Assert.AreEqual(new Circle(new Point(2, 2), MaximalRandomValue - 3), provider.NextRandomObject<Circle>());
            Assert.AreEqual(new Circle(new Point(3, 4), MaximalRandomValue - 2), provider.NextRandomObject<Circle>());
        }

        [Test]
        public void Test_Combined_Generation()
        {
            var list = new List<int> {1, 1, 2, 3, 4, 5, 1, 1, 1, 5, 1, 1, 4, 1, 1, 1, 1, 1, 1, 1};

            var provider = Provider(list);

            Assert.AreEqual(new Point(1, 1), provider.NextRandomObject<Point>());
            Assert.AreEqual(new Line(new Point(2, 3), new Point(4, 5)), provider.NextRandomObject<Line>());
            Assert.AreEqual(new Circle(new Point(1, 1), MaximalRandomValue - 1), provider.NextRandomObject<Circle>());
            Assert.AreEqual(new Line(new Point(5, 1), new Point(1, 4)), provider.NextRandomObject<Line>());
            Assert.AreEqual(new Point(1, 2), provider.NextRandomObject<Point>());
        }
    }
}