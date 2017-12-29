using System;
using GeoGen.Core.Configurations;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Objects.GeometricalObjects
{
    [TestFixture]
    public class CircleObjectTest
    {
        [Test]
        public void Test_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CircleObject(null, 4));
        }

        [Test]
        public void Test_Points_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CircleObject(1, null));
        }

        [Test]
        public void Test_Points_Cant_Contain_Null()
        {
            var point = new PointObject(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}, 1);

            Assert.Throws<ArgumentException>(() => new CircleObject(1, point, null));
        }

        [Test]
        public void Test_Points_Are_Set_Correctly()
        {
            var point = new PointObject(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}, 1);

            var circle = new CircleObject(4, point);

            Assert.NotNull(circle.Points);
            Assert.AreEqual(1, circle.Points.Count);
        }

        [Test]
        public void Test_Equals_And_Hash_Code()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Circle);

            var circle1 = new CircleObject(point, 1);
            var circle2 = new CircleObject(point, 1);
            var circle3 = new CircleObject(point, 2);

            Assert.AreEqual(circle1, circle2);
            Assert.AreEqual(circle2, circle1);

            Assert.AreEqual(circle1.GetHashCode(), circle2.GetHashCode());

            Assert.AreNotEqual(circle1, circle3);
            Assert.AreNotEqual(circle2, circle3);
            Assert.AreNotEqual(circle3, circle1);
            Assert.AreNotEqual(circle3, circle2);
        }
    }
}