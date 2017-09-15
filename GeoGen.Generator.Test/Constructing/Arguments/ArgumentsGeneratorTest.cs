using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Configurations;
using static GeoGen.Generator.Test.TestHelpers.Constructions;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.Constructing.Arguments
{
    [TestFixture]
    public class ArgumentsGeneratorTest
    {
        private static ArgumentsGenerator ArgumentsGenerator()
        {
            var combinator = new Combinator<ConfigurationObjectType, List<ConfigurationObject>>();
            var variationsProvider = new VariationsProvider<ConfigurationObject>();
            var signatureMatcher = new ConstructionSignatureMatcherFactory();
            var objectToStringProvider = new DefaultObjectToStringProvider();
            var argumentsProvider = new ArgumentsToStringProvider(objectToStringProvider);
            var argumentsContainerFactory = new ArgumentsContainerFactory(argumentsProvider);

            return new ArgumentsGenerator(combinator, signatureMatcher, variationsProvider, argumentsContainerFactory);
        }

        [Test]
        public void Constructor_Test_Combinator_Not_Null()
        {
            var variationsProvider = SimpleMock<IVariationsProvider<ConfigurationObject>>();
            var matcherFactory = SimpleMock<IConstructionSignatureMatcherFactory>();
            var argumentsFactory = SimpleMock<IArgumentsContainerFactory>();

            Assert.Throws<ArgumentNullException>
            (
                () => new ArgumentsGenerator(null, matcherFactory, variationsProvider, argumentsFactory)
            );
        }

        [Test]
        public void Constructor_Test_Variations_Provider_Not_Null()
        {
            var combinator = SimpleMock<ICombinator<ConfigurationObjectType, List<ConfigurationObject>>>();
            var matcherFactory = SimpleMock<IConstructionSignatureMatcherFactory>();
            var argumentsFactory = SimpleMock<IArgumentsContainerFactory>();

            Assert.Throws<ArgumentNullException>
            (
                () => new ArgumentsGenerator(combinator, matcherFactory, null, argumentsFactory)
            );
        }

        [Test]
        public void Constructor_Test_Signature_Matcher_Factory_Not_Null()
        {
            var combinator = SimpleMock<ICombinator<ConfigurationObjectType, List<ConfigurationObject>>>();
            var variationsProvider = SimpleMock<IVariationsProvider<ConfigurationObject>>();
            var argumentsFactory = SimpleMock<IArgumentsContainerFactory>();

            Assert.Throws<ArgumentNullException>
            (
                () => new ArgumentsGenerator(combinator, null, variationsProvider, argumentsFactory)
            );
        }

        [Test]
        public void Test_Arguments_Container_Factory_Not_Null()
        {
            var combinator = SimpleMock<ICombinator<ConfigurationObjectType, List<ConfigurationObject>>>();
            var variationsProvider = SimpleMock<IVariationsProvider<ConfigurationObject>>();
            var matcherFactory = SimpleMock<IConstructionSignatureMatcherFactory>();

            Assert.Throws<ArgumentNullException>
            (
                () => new ArgumentsGenerator(combinator, matcherFactory, variationsProvider, null)
            );
        }

        [Test]
        public void Test_Construction_Wrapper_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => ArgumentsGenerator().GenerateArguments(Configuration(1, 1, 1), null)
            );
        }

        [Test]
        public void Test_Configuration_Wrapper_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => ArgumentsGenerator().GenerateArguments(null, CrazyConstruction())
            );
        }

        [TestCase(1, 10, 11, 0)]
        [TestCase(2, 10, 11, 1)]
        [TestCase(7, 0, 1, 21)]
        [TestCase(20, 0, 0, 190)]
        public void Test_Midpoint(int point, int lines, int circles, int expected)
        {
            var construction = Midpoint();
            var configuration = Configuration(point, lines, circles);

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
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

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
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

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
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

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
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

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
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

            var result = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
            Assert.AreEqual(expected, result.Count);
        }

        [TestCase("({4 9})")]
        [TestCase("({1 2})")]
        [TestCase("({8 9})")]
        public void Test_Midpoint_Existence_Of_Arguments(string argument)
        {
            var construction = Midpoint();
            var configuration = Configuration(10, 4, 4);

            var result = ArgumentsGenerator()
                .GenerateArguments(configuration, construction)
                .Select(ToStringHelper.ArgsToString)
                .ToList();

            var contains = result.Any(s => s.Equals(argument));

            Assert.IsTrue(contains);
        }

        [TestCase("(13, {7 8 9}, 1, {14 15})")]
        [TestCase("(15, {10 12 9}, 5, {14 16})")]
        [TestCase("(18, {10 11 12}, 6, {16 17})")]
        public void Test_Crazy_Construction_Existence_Of_Arguments(string argument)
        {
            // Points are [1-6], Lines are [7-12], Circles are [13-18]
            var construction = CrazyConstruction();
            var configuration = Configuration(6, 6, 6);

            var result = ArgumentsGenerator()
                    .GenerateArguments(configuration, construction)
                    .Select(ToStringHelper.ArgsToString)
                    .ToList();

            var contains = result.Any(s => s.Equals(argument));
            Assert.IsTrue(contains);
        }

        [Test]
        [Ignore("This is just a temporary speed test.")]
        public void Test_Intersection_Time_With_25_Points()
        {
            long totalTime = 0;
            const int count = 20;

            for (var i = 0; i < count; i++)
            {
                var construction = Intersection();
                var configuration = Configuration(25, 0, 0);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var unused = ArgumentsGenerator().GenerateArguments(configuration, construction).ToList();
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }

            Console.WriteLine((double) totalTime / count);
        }
    }
}