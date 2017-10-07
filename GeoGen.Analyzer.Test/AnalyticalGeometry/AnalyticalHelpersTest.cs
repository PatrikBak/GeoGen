using System.Collections.Generic;
using GeoGen.Analyzer.AnalyticalGeometry;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.AnalyticalGeometry
{
    [TestFixture]
    public class AnalyticalHelpersTest
    {
        [Test]
        public void Test_Are_Points_Equal()
        {
            var points1 = new List<Point>
            {
                new Point(1.5, 1.7),
                new Point(-1.2, 1.8),
                new Point(1.0 / 3, 2.5),
                new Point(1e-45, -1e-43)
            };

            var points2 = new List<Point>
            {
                new Point(3.0 / 2, 1.7),
                new Point(1.2, 1.8),
                new Point(0.333333333, 2.5),
                new Point(1e-46, -1e-43)
            };

            var expected = new List<bool>
            {
                true,
                false,
                false,
                false
            };

            for (var i = 0; i < points1.Count; i++)
            {
                var result = AnalyticalHelpers.ArePointsEqual(points1[i], points2[i]);
                Assert.AreEqual(expected[i], result, $"{i}");
            }
        }

        [Test]
        public void Test_Midpoint()
        {
        }
    }
}