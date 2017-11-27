using System;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Analyzer.Test.Objects
{
    [TestFixture]
    public class ObjectsContainerTest
    {
        private static ObjectsContainer Container() => new ObjectsContainer();

        [Test]
        public void Test_Add_Analytical_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(null, Object(ConfigurationObjectType.Point, 1)));
        }

        [Test]
        public void Test_Add_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(new Point(1, 1), null));
        }

        [Test]
        public void Test_Add_Types_Of_Object_Must_Correspond()
        {
            var point = Object(ConfigurationObjectType.Point, 1);
            var line = Object(ConfigurationObjectType.Line, 2);
            var circle = Object(ConfigurationObjectType.Circle, 3);

            var analyticalPoint = new Point(1, 5);
            var analyticalLine = new Line(new Point(1, 3), new Point(7, 5));
            var analyticalCircle = new Circle(new Point(1, 2), 3);

            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalPoint, line));
            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalPoint, circle));
            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalLine, point));
            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalLine, circle));
            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalCircle, point));
            Assert.Throws<AnalyzerException>(() => Container().Add(analyticalCircle, line));
        }

        [Test]
        public void Test_Add_Object()
        {
            var container = Container();

            var point1 = Object(ConfigurationObjectType.Point, 1);
            var result1 = container.Add(new Point(1, 3), point1);
            Assert.AreSame(point1, result1);

            var point2 = Object(ConfigurationObjectType.Point, 2);
            var result2 = container.Add(new Point(1, 3), point2);
            Assert.AreSame(point1, result2);

            var line1 = Object(ConfigurationObjectType.Line, 3);
            var result3 = container.Add(new Line(new Point(0, 0), new Point(1, 1)), line1);
            Assert.AreSame(line1, result3);

            var circle1 = Object(ConfigurationObjectType.Circle, 4);
            var result4 = container.Add(new Circle(new Point(0, 0), 5), circle1);
            Assert.AreSame(circle1, result4);
        }

        [Test]
        public void Test_Remove_Configuration_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Remove(null));
        }

        [Test]
        public void Test_Remove_Object_That_Does_Exist()
        {
            var container = Container();

            var point1 = Object(ConfigurationObjectType.Point, 1);
            container.Add(new Point(1, 3), point1);

            Assert.AreEqual(new Point(1, 3), container.Get(point1));
            container.Remove(point1);
            Assert.Throws<AnalyzerException>(() => container.Get(point1));
        }

        [Test]
        public void Test_Remove_Object_That_Doesnt_Exist()
        {
            var container = Container();

            var point1 = Object(ConfigurationObjectType.Point, 1);
            Assert.Throws<AnalyzerException>(() => container.Remove(point1));
        }

        [Test]
        public void Test_Get_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Get<Point>(null));
            Assert.Throws<ArgumentNullException>(() => Container().Get<Line>(null));
            Assert.Throws<ArgumentNullException>(() => Container().Get<Circle>(null));
            Assert.Throws<ArgumentNullException>(() => Container().Get(null));
        }

        [Test]
        public void Test_Get_Object_That_Doesnt_Exist()
        {
            var container = Container();

            var point1 = Object(ConfigurationObjectType.Point, 1);

            Assert.Throws<AnalyzerException>(() => container.Get(point1));
            Assert.Throws<AnalyzerException>(() => container.Get<Point>(point1));
        }

        [Test]
        public void Test_Get_Object_That_Exists()
        {
            var container = Container();

            var point = Object(ConfigurationObjectType.Point, 1);
            var line = Object(ConfigurationObjectType.Line, 2);
            var circle = Object(ConfigurationObjectType.Circle, 3);

            var analyticalPoint = new Point(1, 5);
            var analyticalLine = new Line(new Point(1, 3), new Point(7, 5));
            var analyticalCircle = new Circle(new Point(1, 2), 3);

            container.Add(analyticalCircle, circle);
            container.Add(analyticalLine, line);
            container.Add(analyticalPoint, point);

            var result1 = container.Get<Point>(point);
            var result2 = container.Get<Line>(line);
            var result3 = container.Get<Circle>(circle);

            Assert.AreEqual(result1, analyticalPoint);
            Assert.AreEqual(result2, analyticalLine);
            Assert.AreEqual(result3, analyticalCircle);

            var result4 = container.Get(point);
            var result5 = container.Get(line);
            var result6 = container.Get(circle);

            Assert.AreEqual(result4, analyticalPoint);
            Assert.AreEqual(result5, analyticalLine);
            Assert.AreEqual(result6, analyticalCircle);
        }

        [Test]
        public void Test_Get_Object_Of_Incorrect_Type()
        {
            var container = Container();

            var point = Object(ConfigurationObjectType.Point, 1);
            var line = Object(ConfigurationObjectType.Line, 2);
            var circle = Object(ConfigurationObjectType.Circle, 3);

            var analyticalPoint = new Point(1, 5);
            var analyticalLine = new Line(new Point(1, 3), new Point(7, 5));
            var analyticalCircle = new Circle(new Point(1, 2), 3);

            container.Add(analyticalCircle, circle);
            container.Add(analyticalLine, line);
            container.Add(analyticalPoint, point);

            Assert.Throws<AnalyzerException>(() => container.Get<Line>(point));
            Assert.Throws<AnalyzerException>(() => container.Get<Line>(circle));
            Assert.Throws<AnalyzerException>(() => container.Get<Point>(line));
            Assert.Throws<AnalyzerException>(() => container.Get<Point>(circle));
            Assert.Throws<AnalyzerException>(() => container.Get<Circle>(point));
            Assert.Throws<AnalyzerException>(() => container.Get<Circle>(line));
        }
    }
}