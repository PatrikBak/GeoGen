using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding
{
    [TestFixture]
    public class LeastConfigurationFinderTest
    {
        private LeastConfigurationFinder Finder(IEnumerable<LooseConfigurationObject> objects)
        {
            var resolver = new DefaultObjectIdResolver();
            var defaultToString = new DefaultObjectToStringProvider(resolver);
            var def = new DefaultArgumentToStringProvider(defaultToString);
            var factory = new CustomArgumentToStringProviderFactory();
            var argsProvider = new ArgumentsListToStringProvider(def);
            var objectToStringFactory = new CustomFullObjectToStringProviderFactory(argsProvider, factory);
            var variationsProvider = new VariationsProvider<LooseConfigurationObject>();
            var configurationToString = new ConfigurationToStringProvider();
            var container = new DictionaryObjectIdResolversContainer(variationsProvider);
            container.Initialize(objects.ToList());

            return new LeastConfigurationFinder(configurationToString, objectToStringFactory, container);
        }

        private string AsString(DictionaryObjectIdResolver resolver, IEnumerable<ConfigurationObject> objects)
        {
            return string.Join(" ", objects.Select(obj => resolver.ResolveId(obj).ToString()));
        }

        [Test]
        public void Container_Not_Null()
        {
            var toString = Utilities.SimpleMock<IConfigurationToStringProvider>();
            var factory = Utilities.SimpleMock<ICustomFullObjectToStringProviderFactory>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(toString, factory, null));
        }

        [Test]
        public void Configuration_To_String_Provider_Not_Null()
        {
            var container = Utilities.SimpleMock<IDictionaryObjectIdResolversContainer>();
            var factory = Utilities.SimpleMock<ICustomFullObjectToStringProviderFactory>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(null, factory, container));
        }

        [Test]
        public void Factory_Not_Null()
        {
            var container = Utilities.SimpleMock<IDictionaryObjectIdResolversContainer>();
            var toString = Utilities.SimpleMock<IConfigurationToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new LeastConfigurationFinder(toString, null, container));
        }

        [Test]
        public void Configuration_Not_Null()
        {
            var points = ConfigurationObjects.Objects(1, ConfigurationObjectType.Point);

            Assert.Throws<ArgumentNullException>
            (
                () => Finder(points).FindLeastConfiguration(null)
            );
        }

        [Test]
        public void Only_Loose_Points_Test()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 0);
            var configuration = new Configuration(objects.ToSet(), new List<ConstructedConfigurationObject>());
            var resolver = Finder(objects).FindLeastConfiguration(configuration);

            Assert.AreEqual(0, resolver.ResolveId(objects[0]));
            Assert.AreEqual(1, resolver.ResolveId(objects[1]));
            Assert.AreEqual(2, resolver.ResolveId(objects[2]));
        }

        [Test]
        public void One_Midpoint_Test()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 0);
            var args = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[1]),
                new ObjectConstructionArgument(objects[2])
            };
            ToStringHelper.SetIds(args);
            var obj = ConfigurationObjects.ConstructedObject(42, 0, args, 5);
            var objAsList = new List<ConstructedConfigurationObject> {obj};

            var configuration = new Configuration(objects.ToSet(), objAsList);
            var resolver = Finder(objects).FindLeastConfiguration(configuration);
            var asString = AsString(resolver, objects);

            var options = new[] {"2 0 1", "2 1 0"};

            Assert.IsTrue(options.Contains(asString));
        }
    }
}