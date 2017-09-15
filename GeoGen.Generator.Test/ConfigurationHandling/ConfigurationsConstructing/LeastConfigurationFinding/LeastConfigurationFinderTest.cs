using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding
{
    [TestFixture]
    public class LeastConfigurationFinderTest
    {
        private LeastConfigurationFinder Finder(IEnumerable<LooseConfigurationObject> objects)
        {
            var provider = new DefaultObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(provider);
            var configurationToString = new ConfigurationToStringProvider();
            var objectToStringFactory = new CustomFullObjectToStringProviderFactory(argsToString);
            var variationsProvider = new VariationsProvider<int>();
            var container = new DictionaryObjectIdResolversContainer(variationsProvider);
            container.Initialize(objects);

            return new LeastConfigurationFinder(configurationToString, objectToStringFactory, container);
        }

        private string AsString(DictionaryObjectIdResolver resolver, IEnumerable<ConfigurationObject> objects)
        {
            return string.Join(" ", objects.Select(obj => resolver.ResolveId(obj).ToString()));
        }

        [Test]
        public void Container_Not_Null()
        {
            var toString = SimpleMock<IConfigurationToStringProvider>();
            var factory = SimpleMock<ICustomFullObjectToStringProviderFactory>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(toString, factory, null));
        }

        [Test]
        public void Configuration_To_String_Provider_Not_Null()
        {
            var container = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var factory = SimpleMock<ICustomFullObjectToStringProviderFactory>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(null, factory, container));
        }

        [Test]
        public void Factory_Not_Null()
        {
            var container = SimpleMock<IDictionaryObjectIdResolversContainer>();
            var toString = SimpleMock<IConfigurationToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(toString, null, container));
        }

        [Test]
        public void Configuration_Not_Null()
        {
            var points = Objects(1, ConfigurationObjectType.Point);

            Assert.Throws<ArgumentNullException>
            (
                () => Finder(points).FindLeastConfiguration(null)
            );
        }

        [Test]
        public void Only_Loose_Points_Test()
        {
            var objects = Objects(3, ConfigurationObjectType.Point, 0);
            var configuration = new Configuration(objects.ToSet(), new List<ConstructedConfigurationObject>());
            var resolver = Finder(objects).FindLeastConfiguration(configuration);

            Assert.AreEqual(0, resolver.ResolveId(objects[0]));
            Assert.AreEqual(1, resolver.ResolveId(objects[1]));
            Assert.AreEqual(2, resolver.ResolveId(objects[2]));
        }

        [Test]
        public void One_Midpoint_Test()
        {
            var objects = Objects(3, ConfigurationObjectType.Point, 0);
            var args = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[1]),
                new ObjectConstructionArgument(objects[2])
            };
            var obj = ConstructedObject(42, 0, args, 5);
            var objAsList = new List<ConstructedConfigurationObject> {obj};

            var configuration = new Configuration(objects.ToSet(), objAsList);
            var resolver = Finder(objects).FindLeastConfiguration(configuration);
            var asString = AsString(resolver, objects);

            var options = new[] {"2 0 1", "2 1 0"};

            Assert.IsTrue(options.Contains(asString));
        }
    }
}