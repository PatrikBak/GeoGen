using System.Linq;
using Moq;
using NUnit.Framework;

namespace GeoGen.AnalyticalGeometry.Test
{
    [TestFixture]
    public class TriangleConstructorTest
    {
        private TriangleConstructor Constructor()
        {
            // Create a randomness provider that always returns a midpoint of any interval
            var randomMock = new Mock<IRandomnessProvider>();
            randomMock.Setup(s => s.NextDouble(It.IsAny<double>(), It.IsAny<double>()))
                    .Returns<double, double>((min, max) => (min + max) / 2);

            // Create the constructor
            return new TriangleConstructor(randomMock.Object);
        }

        [Test]
        public void Test_Scalene_Acute_Angled_Triangle()
        {
            // Difference d is internally set to 7.5
            // Alpha should be in the interval (60-d, 90+d), i.e. it will be the middle 75
            // Beta should be in the interval ((180+d-A)/2, A-d), i.e. it will be the middle 61,875
            // According to GeoGebra, the point C should be around
            var result = new Point(0.333909581513415, 1.246167523342127);

            // Lets find the actual points
            var points = Constructor().NextScaleneAcuteAngedTriangle().Cast<Point>().ToList();

            // Assert
            Assert.AreEqual(new Point(0, 0), points[0]);
            Assert.AreEqual(new Point(1, 0), points[1]);
            Assert.AreEqual(result, points[2]);
        }
    }
}