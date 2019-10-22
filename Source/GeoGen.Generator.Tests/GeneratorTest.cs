using FluentAssertions;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Utilities;
using Ninject;
using NUnit.Framework;
using System.Collections.Generic;

namespace GeoGen.Generator.Tests
{
    /// <summary>
    /// The test class for <see cref="Generator.Generator"/>
    /// </summary>
    [TestFixture]
    public class GeneratorTest
    {
        #region Generator instance

        /// <summary>
        /// Gets the instance of the generator
        /// </summary>
        private static IGenerator Generator => IoC.CreateKernel().AddGenerator().Get<IGenerator>();

        #endregion

        [Test]
        public void Test_Triangle_And_Midpoint()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(LooseObjectsLayout.Triangle, A, B, C);

            // Prepare the input with just the midpoint construction
            var input = new GeneratorInput
            (
                initialConfiguration: configuration,
                constructions: new HashSet<Construction> { PredefinedConstructions.Midpoint }.ToReadOnlyHashSet(),
                numberOfIterations: 3,
                objectFilter: _ => true,
                configurationFilter: _ => true
            );

            // Assert count (can be verified by hand)
            Generator.Generate(input).Should().HaveCount(18);
        }
    }
}
