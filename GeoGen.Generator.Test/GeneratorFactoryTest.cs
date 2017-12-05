using System;
using System.Linq;
using GeoGen.Analyzer;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Configurations;
using static GeoGen.Generator.Test.TestHelpers.Constructions;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    public class GeneratorFactoryTest
    {
        [Test]
        public void Test_Constructions_Container_Cant_Be_Null()
        {
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var initialiazer = SimpleMock<IAnalyzerInitializer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(null, constructor, handler, configurations, resolvers, initialiazer)
            );
        }

        [Test]
        public void Test_Objects_Constructor_Cant_Be_Null()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var initialiazer = SimpleMock<IAnalyzerInitializer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(constructions, null, handler, configurations, resolvers, initialiazer)
            );
        }

        [Test]
        public void Test_Configurations_Handler_Cant_Be_Null()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var constructor = SimpleMock<IObjectsConstructor>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var initialiazer = SimpleMock<IAnalyzerInitializer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(constructions, constructor, null, configurations, resolvers, initialiazer)
            );
        }

        [Test]
        public void Test_Configurations_Container_Cant_Be_Null()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var initialiazer = SimpleMock<IAnalyzerInitializer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(constructions, constructor, handler, null, resolvers, initialiazer)
            );
        }

        [Test]
        public void Test_Dictionary_Object_Id_Resolvers_Container_Cant_Be_Null()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var initialiazer = SimpleMock<IAnalyzerInitializer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(constructions, constructor, handler, configurations, null, initialiazer)
            );
        }

        [Test]
        public void Test_Analyzer_Initialiazer_Cant_Be_Null()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();

            Assert.Throws<ArgumentNullException>
            (
                () => new GeneratorFactory(constructions, constructor, handler, configurations, resolvers, null)
            );
        }

        [Test]
        public void Test_Generator_Is_Returned()
        {
            var constructions = SimpleMock<IConstructionsContainer>();
            var constructor = SimpleMock<IObjectsConstructor>();
            var handler = SimpleMock<IConfigurationsHandler>();
            var configurations = SimpleMock<IConfigurationsContainer>();
            var resolvers = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var initializer = SimpleMock<IAnalyzerInitializer>();

            var generator = new GeneratorFactory(constructions, constructor, handler, configurations, resolvers, initializer);

            var input = new GeneratorInput
            {
                Constructions = ConstructionWithId(42).SingleItemAsEnumerable().ToList(),
                InitialConfiguration = Configuration(1, 1, 1).Configuration,
                MaximalNumberOfIterations = 5
            };

            Assert.NotNull(generator.CreateGenerator(input));
        }
    }
}