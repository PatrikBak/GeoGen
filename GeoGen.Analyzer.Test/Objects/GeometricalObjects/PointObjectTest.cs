using System;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Objects.GeometricalObjects
{
    [TestFixture]
    public class PointObjectTest
    {
        [Test]
        public void Test_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new PointObject(null, 4));
        }

        [Test]
        public void Test_Type_Cant_Be_Null()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};

            var pointObject = new PointObject(point, 4);

            Assert.Throws<ArgumentNullException>(() => pointObject.ObjectsThatContainThisPoint(null));
        }

        [Test]
        public void Test_When_Type_Is_Incorrect()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};

            var pointObject = new PointObject(point, 4);

            Assert.Throws<AnalyzerException>(() => pointObject.ObjectsThatContainThisPoint(typeof(Circle)));
        }

        [Test]
        public void Test_Returning_Lines()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};

            var pointObject = new PointObject(point, 4);

            var lines = pointObject.ObjectsThatContainThisPoint(typeof(LineObject));

            Assert.NotNull(lines);
            Assert.AreSame(lines, pointObject.Lines);
        }

        [Test]
        public void Test_Returning_Circles()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};

            var pointObject = new PointObject(point, 4);

            var circles = pointObject.ObjectsThatContainThisPoint(typeof(CircleObject));

            Assert.NotNull(circles);
            Assert.AreSame(circles, pointObject.Circles);
        }

        [Test]
        public void Test_Equals_And_HashCode()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Point);

            var p1 = new PointObject(point, 1);
            var p2 = new PointObject(point, 1);
            var p3 = new PointObject(point, 2);

            Assert.AreEqual(p1, p2);
            Assert.AreEqual(p2, p1);

            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());

            Assert.AreNotEqual(p1, p3);
            Assert.AreNotEqual(p2, p3);
            Assert.AreNotEqual(p3, p1);
            Assert.AreNotEqual(p3, p2);
        }
    }
}