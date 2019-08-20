using FluentAssertions;
using GeoGen.Core;
using GeoGen.DependenciesResolver;
using Ninject;
using NUnit.Framework;

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
            var configuration = Configuration.DeriveFromObjects(LooseObjectsLayout.ThreePoints, A, B, C);

            // Prepare the input
            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = new[] { PredefinedConstructions.Midpoint },
                NumberOfIterations = 3
            };

            // Assert count
            Generator.Generate(input, _ => true, _ => true).Should().HaveCount(18);
        }
    }
}
