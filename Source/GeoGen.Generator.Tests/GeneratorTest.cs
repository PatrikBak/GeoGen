using FluentAssertions;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.Generator.Tests
{
    /// <summary>
    /// The test class for <see cref="Generator.Generator"/>
    /// </summary>
    [TestFixture]
    public class GeneratorTest
    {
        #region GetGenerator method 

        /// <summary>
        /// Gets an instance of the generator.
        /// </summary>
        /// <param name="filterType">The type of configuration filter to be used</param>
        private static IGenerator GetGenerator(ConfigurationFilterType filterType) => IoC.CreateKernel().AddGenerator(filterType).Get<IGenerator>();

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
            var input = new GeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalObjectCounts: new Dictionary<ConfigurationObjectType, int>
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
            var input = new GeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalObjectCounts: new Dictionary<ConfigurationObjectType, int>
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
            var input = new GeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: iterations,
                maximalObjectCounts: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, 6 }
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
            var input = new GeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { Midpoint, LineFromPoints, Circumcircle }.ToReadOnlyHashSet(),
                numberOfIterations: 42,
                maximalObjectCounts: new Dictionary<ConfigurationObjectType, int>
                {
                    { Point, 4 },
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