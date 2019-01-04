using NUnit.Framework;
using System.Collections.Generic;

namespace GeoGen.AnalyticGeometry.Tests
{
    [TestFixture]
    public class AnalyticalHelperTest
    {
        [Test]
        public void Test_Lies_On_With_Line()
        {
            var line = new Line(new Point(1, 1), new Point(4, 5));

            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(1, 1)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(7, 9)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(4, 5)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(-2, -3)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(line, new Point(2.5, 3)));

            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(7, 2)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(7, 3)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(-5, 3)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(line, new Point(-7, -7)));
        }

        [Test]
        public void Test_Lies_On_With_Circle()
        {
            var circle = new Circle(new Point(1, 1), 2);

            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-1, 1)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-0.2, 2.6)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(-0.6, 2.2)));
            Assert.IsTrue(AnalyticHelpers.LiesOn(circle, new Point(3, 1)));

            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(1, 1)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(7, -5)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(5, -1)));
            Assert.IsFalse(AnalyticHelpers.LiesOn(circle, new Point(3, -4)));
        }

        [Test]
        public void Test_Intersect_Two_Lines()
        {
            var objects = new List<IAnalyticObject>
            {
                new Line(new Point(1, 5), new Point(1, 3)),
                new Line(new Point(1, 5), new Point(2, 4))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Count);
            Assert.IsTrue(points.Contains(new Point(1, 5)));
        }

        [Test]
        public void Test_Intersect_Two_Circles()
        {
            var objects = new List<IAnalyticObject>
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Circle(new Point(0, 0), new Point(1, 1), new Point(8, 7))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(2, points.Count);
            Assert.IsTrue(points.Contains(new Point(0, 0)));
            Assert.IsTrue(points.Contains(new Point(1, 1)));
        }

        [Test]
        public void Test_Intersect_Line_And_Circle()
        {
            var objects = new List<IAnalyticObject>
            {
                new Circle(new Point(0, 0), new Point(1, 1), new Point(7, 8)),
                new Line(new Point(0, 0), new Point(1, 1))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(2, points.Count);
            Assert.IsTrue(points.Contains(new Point(0, 0)));
            Assert.IsTrue(points.Contains(new Point(1, 1)));
        }

        [Test]
        public void Test_Intersect_Complex_Test_Medians()
        {
            var a = new Point(1, 5);
            var b = new Point(7, 4);
            var c = new Point(1, 77);

            var objects = new List<IAnalyticObject>
            {
                new Line(a, b.Midpoint(c)),
                new Line(b, c.Midpoint(a)),
                new Line(c, a.Midpoint(b))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Count);
            Assert.IsTrue(points.Contains((a + b + c) / 3));
        }

        [Test]
        public void Test_Intersect_Complex_Test_Feet_Of_Altitude()
        {
            var a = new Point(1, 5);
            var b = new Point(7, 4);
            var c = new Point(1, 77);

            var objects = new List<IAnalyticObject>
            {
                new Line(b, c),
                new Circle(a.Midpoint(b), a.DistanceTo(b) / 2),
                new Circle(a.Midpoint(c), a.DistanceTo(c) / 2),
                new Line(a, a.Project(new Line(b, c)))
            };

            var points = AnalyticHelpers.Intersect(objects);

            Assert.AreEqual(1, points.Count);
            Assert.IsTrue(points.Contains(a.Project(new Line(b, c))));
        }
    }
}