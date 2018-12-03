//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.Core.Configurations;
//using GeoGen.Core.Constructions.Arguments;
//using GeoGen.Core.Utilities;
//using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
//using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
//using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
//using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
//using GeoGen.Generator.Test.TestHelpers;
//using GeoGen.Utilities;
//using NUnit.Framework;
//using static GeoGen.Generator.Test.TestHelpers.Utilities;

//namespace GeoGen.Generator.Test.ConstructingConfigurations.LeastConfigurationFinding
//{
//    [TestFixture]
//    public class LeastConfigurationFinderTest
//    {
//        private static LeastConfigurationFinder Finder(IEnumerable<LooseConfigurationObject> objects)
//        {
//            var resolver = new DefaultObjectIdResolver();
//            var defaultToString = new DefaultObjectToStringProvider(resolver);
//            var argsListToString = new ArgumentsListToStringProvider(defaultToString);
//            var fullObjectToStringFactory = new CustomFullObjectToStringProviderFactory(argsListToString);
//            var variationsProvider = new VariationsProvider();
//            var configurationToString = new ConfigurationToStringProvider();
//            var dicionaryResolversContainer = new DictionaryObjectIdResolversContainer(null, variationsProvider);

//            return new LeastConfigurationFinder(configurationToString, fullObjectToStringFactory, dicionaryResolversContainer);
//        }

//        private static string AsString(DictionaryObjectIdResolver resolver, IEnumerable<ConfigurationObject> objects)
//        {
//            return string.Join(" ", objects.Select(obj => resolver.ResolveId(obj).ToString()));
//        }

//        [Test]
//        public void Test_Container_Cant_Be_Null()
//        {
//            var provider = SimpleMock<IConfigurationToStringProvider>();
//            var factory = SimpleMock<ICustomFullObjectToStringProviderFactory>();

//            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(provider, factory, null));
//        }

//        [Test]
//        public void Test_Configuration_To_String_Provider_Cant_Be_Null()
//        {
//            var container = SimpleMock<IDictionaryObjectIdResolversContainer>();
//            var factory = SimpleMock<ICustomFullObjectToStringProviderFactory>();

//            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(null, factory, container));
//        }

//        [Test]
//        public void Test_Factory_Cant_Be_Null()
//        {
//            var container = SimpleMock<IDictionaryObjectIdResolversContainer>();
//            var provider = SimpleMock<IConfigurationToStringProvider>();

//            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(provider, null, container));
//        }

//        [Test]
//        public void Test_Passed_Configuration_Cant_Be_Null()
//        {
//            var points = ConfigurationObjects.Objects(1, ConfigurationObjectType.Point);

//            Assert.Throws<ArgumentNullException>(() => Finder(points).FindLeastConfiguration(null));
//        }

//        [Test]
//        public void Test_Configuation_With_Only_Loose_Points()
//        {
//            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 0);
//            var configuration = new Configuration(objects.ToSet(), new List<ConstructedConfigurationObject>());
//            var resolver = Finder(objects).FindLeastConfiguration(configuration);

//            Assert.AreEqual(0, resolver.ResolveId(objects[0]));
//            Assert.AreEqual(1, resolver.ResolveId(objects[1]));
//            Assert.AreEqual(2, resolver.ResolveId(objects[2]));
//        }

//        [Test]
//        public void Test_One_Midpoint_Configuration()
//        {
//            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 0);
//            var args = new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(objects[1]),
//                new ObjectConstructionArgument(objects[2])
//            };
//            var obj = ConfigurationObjects.ConstructedObject(42, 0, args, 5);
//            var objAsList = new List<ConstructedConfigurationObject> {obj};

//            var configuration = new Configuration(objects.ToSet(), objAsList);
//            var resolver = Finder(objects).FindLeastConfiguration(configuration);
//            var asString = AsString(resolver, objects);

//            var options = new[] {"2 0 1", "2 1 0"};

//            Assert.IsTrue(options.Contains(asString));
//        }
//    }
//}