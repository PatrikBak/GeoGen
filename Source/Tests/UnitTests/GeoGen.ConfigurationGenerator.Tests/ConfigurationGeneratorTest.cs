using FluentAssertions;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.ConfigurationGenerator.Tests
{
    /// <summary>
    /// The test class for <see cref="ConfigurationGenerator.ConfigurationGenerator"/>
    /// </summary>
    [TestFixture]
    public class ConfigurationGeneratorTest
    {
        #region GetGenerator method 

        /// <summary>
        /// Gets an instance of the generator.
        /// </summary>
        /// <param name="filterType">The type of configuration filter to be used</param>
        private static IConfigurationGenerator GetGenerator(ConfigurationFilterType filterType) => NinjectUtilities.CreateKernel().AddConfigurationGenerator(new GenerationSettings(filterType)).Get<IConfigurationGenerator>();

        #endregion

        [TestCase(1, 1)]
        [TestCase(2, 4)]
        [TestCase(3, 18)]
        public void Test_Triangle_And_Midpoint_No_Initial_Objects(int iterations, int expectedCount)
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C);

            // Prepare the input with just the midpoint construction
            var input = new ConfigurationGeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalNumbersOfObjectsToAdd: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, int.MaxValue }
                },
                configurationFilter: _ => true
            );

            // Assert counts (can be verified by hand)
            GetGenerator(ConfigurationFilterType.Fast).Generate(input).ToArray().Length.Should().Be(expectedCount);
            GetGenerator(ConfigurationFilterType.MemoryEfficient).Generate(input).ToArray().Length.Should().Be(expectedCount);
        }

        [TestCase(1, 3)]
        public void Test_Triangle_And_Midpoint_With_Initial_Objects(int iterations, int expectedCount)
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var P = new ConstructedConfigurationObject(Midpoint, A, B);
            var Q = new ConstructedConfigurationObject(Midpoint, B, C);
            var R = new ConstructedConfigurationObject(Midpoint, C, A);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C, P, Q, R);

            // Prepare the input with just the midpoint construction
            var input = new ConfigurationGeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalNumbersOfObjectsToAdd: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, int.MaxValue }
                },
                configurationFilter: _ => true
            );

            // Assert counts (can be verified by hand)
            GetGenerator(ConfigurationFilterType.Fast).Generate(input).ToArray().Length.Should().Be(expectedCount);
            GetGenerator(ConfigurationFilterType.MemoryEfficient).Generate(input).ToArray().Length.Should().Be(expectedCount);
        }

        [TestCase(1, 1)]
        [TestCase(2, 4)]
        [TestCase(3, 18)]
        [TestCase(4, 18)]
        [TestCase(5, 18)]
        public void Test_Triangle_And_Midpoint_With_Limiting_Number_Of_Objects(int iterations, int expectedCount)
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C);

            // Prepare the input with just the midpoint construction
            var input = new ConfigurationGeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalNumbersOfObjectsToAdd: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, 3 }
                },
                configurationFilter: _ => true
            );

            // Assert counts (can be verified by hand)
            GetGenerator(ConfigurationFilterType.Fast).Generate(input).ToArray().Length.Should().Be(expectedCount);
            GetGenerator(ConfigurationFilterType.MemoryEfficient).Generate(input).ToArray().Length.Should().Be(expectedCount);
        }

        [Test]
        public void Test_Triangle_And_Various_Constructions_With_Limiting_Number_Of_Objects()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, A, B, C);

            // Prepare the input with just the midpoint construction
            var input = new ConfigurationGeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint, LineFromPoints, Circumcircle }.ToReadOnlyHashSet(),
                numberOfIterations: 42,
                maximalNumbersOfObjectsToAdd: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, 1 },
                    { Line, 1 },
                    { Circle, 0 },
                },
                configurationFilter: _ => true
            );

            // Assert counts (can be verified by hand)
            GetGenerator(ConfigurationFilterType.Fast).Generate(input).ToArray().Length.Should().Be(6);
            GetGenerator(ConfigurationFilterType.MemoryEfficient).Generate(input).ToArray().Length.Should().Be(6);
        }
    }
}