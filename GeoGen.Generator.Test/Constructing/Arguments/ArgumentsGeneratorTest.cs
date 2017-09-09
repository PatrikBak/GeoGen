using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments
{
    [TestFixture]
    public class ArgumentsGeneratorTest
    {
        private static int _currentId = 1;

        [SetUp]
        public void ResetId()
        {
            _currentId = 1;
        }

        private static List<LooseConfigurationObject> Objects(int count, ConfigurationObjectType type)
        {
            var result = Enumerable.Range(_currentId, count)
                    .Select(i => new LooseConfigurationObject(type) {Id = i})
                    .ToList();

            _currentId += count;

            return result;
        }

        private static ArgumentsGenerator TestGenerator()
        {
            var combinator = new Combinator<ConfigurationObjectType, List<ConfigurationObject>>();
            var variationsProvider = new VariationsProvider<ConfigurationObject>();
            var signatureMatcher = new ConstructionSignatureMatcher();
            var argumentsContainer = new ArgumentsContainer(new ArgumentsToStringProvider());

            return new ArgumentsGenerator(combinator, signatureMatcher, variationsProvider, argumentsContainer);
        }

        private static string TestString(IReadOnlyList<ConstructionArgument> arg)
        {
            return new ArgumentsToStringProvider(", ").ConvertToString(arg);
        }

        internal static ConstructionWrapper Midpoint()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(1);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> { ConfigurationObjectType.Point });
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 2}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        internal static ConstructionWrapper Intersection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(2);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 4}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        internal static ConstructionWrapper Projection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(3);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> { ConfigurationObjectType.Point });
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 3}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        internal static ConstructionWrapper CircleCenter()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(4);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> { ConfigurationObjectType.Point });
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle),
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Circle, 1}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        internal static ConstructionWrapper CircumCircle()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(5);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> { ConfigurationObjectType.Circle });
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 3}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        internal static ConstructionWrapper CrazyConstruction()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(6);
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> { ConfigurationObjectType.Point });
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Line), 3),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Circle), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 1},
                {ConfigurationObjectType.Line, 3},
                {ConfigurationObjectType.Circle, 3}
            };

            return new ConstructionWrapper
            {
                Construction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        private static ConfigurationWrapper Configuration(int npoints, int nlines, int ncircles)
        {
            var points = Objects(npoints, ConfigurationObjectType.Point);
            var lines = Objects(nlines, ConfigurationObjectType.Line);
            var circles = Objects(ncircles, ConfigurationObjectType.Circle);

            var objects = new HashSet<LooseConfigurationObject>(points.Union(lines).Union(circles));

            var configuration = new Configuration(objects, new List<ConstructedConfigurationObject>());

            var map = new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>();

            if (npoints != 0)
                map.Add(ConfigurationObjectType.Point, points.Cast<ConfigurationObject>().ToList());
            if (nlines != 0)
                map.Add(ConfigurationObjectType.Line, lines.Cast<ConfigurationObject>().ToList());
            if (ncircles != 0)
                map.Add(ConfigurationObjectType.Circle, circles.Cast<ConfigurationObject>().ToList());

            return new ConfigurationWrapper
            {
                Configuration = configuration,
                ObjectTypeToObjects = map
            };
        }

        [TestCase(1, 10, 11, 0)]
        [TestCase(2, 10, 11, 1)]
        [TestCase(7, 0, 1, 21)]
        [TestCase(20, 0, 0, 190)]
        public void Test_Midpoint(int point, int lines, int circles, int expected)
        {
            var construction = Midpoint();
            var configuration = Configuration(point, lines, circles);

            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(1, 10, 11, 0)]
        [TestCase(2, 10, 11, 0)]
        [TestCase(3, 10, 11, 0)]
        [TestCase(4, 10, 11, 3)]
        [TestCase(15, 10, 11, 4095)]
        public void Test_Intersection(int point, int lines, int circles, int expected)
        {
            var construction = Intersection();
            var configuration = Configuration(point, lines, circles);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(1, 10, 11, 0)]
        [TestCase(2, 10, 11, 0)]
        [TestCase(3, 10, 11, 3)]
        [TestCase(10, 10, 11, 360)]
        [TestCase(15, 10, 11, 1365)]
        public void Test_Projection(int point, int lines, int circles, int expected)
        {
            var construction = Projection();
            var configuration = Configuration(point, lines, circles);

            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(17, 10, 11, 11)]
        [TestCase(17, 10, 12, 12)]
        [TestCase(17, 10, 1, 1)]
        [TestCase(17, 10, 14, 14)]
        [TestCase(17, 10, 0, 0)]
        public void Test_CircleCenter(int point, int lines, int circles, int expected)
        {
            var construction = CircleCenter();
            var configuration = Configuration(point, lines, circles);

            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(1, 10, 11, 0)]
        [TestCase(2, 10, 11, 0)]
        [TestCase(3, 10, 11, 1)]
        [TestCase(4, 10, 11, 4)]
        [TestCase(7, 10, 11, 35)]
        [TestCase(11, 10, 11, 165)]
        public void Test_Circum_Circle(int point, int lines, int circles, int expected)
        {
            var construction = CircumCircle();
            var configuration = Configuration(point, lines, circles);

            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase(7, 2, 11, 0)]
        [TestCase(11, 11, 2, 0)]
        [TestCase(0, 9, 11, 0)]
        [TestCase(11, 4, 7, 4620)]
        [TestCase(6, 6, 6, 7200)]
        public void Test_CrazyConstruction(int point, int lines, int circles, int expected)
        {
            var construction = CrazyConstruction();
            var configuration = Configuration(point, lines, circles);

            var result = TestGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase("{4, 9}")]
        [TestCase("{1, 2}")]
        [TestCase("{8, 9}")]
        public void Test_Midpoint_Existence_Of_Arguments(string argument)
        {
            var construction = Midpoint();
            var configuration = Configuration(10, 4, 4);

            var result = TestGenerator().GenerateArguments(configuration, construction).Select(TestString).ToList();
            var contains = result.Any(s => s.Equals(argument));

            Assert.IsTrue(contains);
        }

        [TestCase("13, {7, 8, 9}, 1, {14, 15}")]
        [TestCase("15, {10, 12, 9}, 5, {14, 16}")]
        [TestCase("18, {10, 11, 12}, 6, {16, 17}")]
        public void Test_Crazy_Construction_Existence_Of_Arguments(string argument)
        {
            // Points are [1-6], Lines are [7-12], Circles are [13-18]
            var construction = CrazyConstruction();
            var configuration = Configuration(6, 6, 6);

            var result = TestGenerator().GenerateArguments(configuration, construction).Select(TestString).ToList();
            var contains = result.Any(s => s.Equals(argument));
            Assert.IsTrue(contains);
        }

        [Test]
        [Ignore("This is just a temporary speed test.")]
        public void Test_Intersection_Time_With_25_Points()
        {
            var construction = Intersection();
            var configuration = Configuration(25, 0, 0);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TestGenerator().GenerateArguments(configuration, construction).ToList();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}