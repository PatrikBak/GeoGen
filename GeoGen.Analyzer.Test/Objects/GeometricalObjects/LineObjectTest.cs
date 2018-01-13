using System;
using GeoGen.Core;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Objects.GeometricalObjects
{
    [TestFixture]
    public class LineObjectTest
    {
        [Test]
        public void Test_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new LineObject(null, 4));
        }

        [Test]
        public void Test_Points_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new LineObject(1, null));
        }

        [Test]
        public void Test_Points_Cant_Contain_Null()
        {
            var point = new PointObject(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}, 1);

            Assert.Throws<ArgumentException>(() => new LineObject(1, point, null));
        }

        [Test]
        public void Test_Points_Are_Set_Correctly()
        {
            var point = new PointObject(new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}, 1);

            var line = new LineObject(4, point);

            Assert.NotNull(line.Points);
            Assert.AreEqual(1, line.Points.Count);
        }

        [Test]
        public void Test_Equals_And_Hash_Code()
        {
            var point = new LooseConfigurationObject(ConfigurationObjectType.Line);

            var line1 = new LineObject(point, 1);
            var line2 = new LineObject(point, 1);
            var line3 = new LineObject(point, 2);

            Assert.AreEqual(line1, line2);
            Assert.AreEqual(line2, line1);

            Assert.AreEqual(line1.GetHashCode(), line2.GetHashCode());

            Assert.AreNotEqual(line1, line3);
            Assert.AreNotEqual(line2, line3);
            Assert.AreNotEqual(line3, line1);
            Assert.AreNotEqual(line3, line2);
        }
    }
}