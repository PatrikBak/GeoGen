using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Test.TestHelpers;
using GeoGen.Utilities.Variations;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DictionaryObjectIdResolversContainerTest
    {
        private static DictionaryObjectIdResolversContainer Container(IEnumerable<LooseConfigurationObject> objects = null)
        {
            var variationsProvider = new VariationsProvider();

            var container = new Mock<IConfigurationObjectsContainer>();

            var looseObjects = objects ?? Enumerable.Empty<LooseConfigurationObject>();

            container.Setup(s => s.LooseObjects).Returns(looseObjects);

            return new DictionaryObjectIdResolversContainer(container.Object, variationsProvider);
        }

        [Test]
        public void Test_Variations_Provider_Cant_Be_Null()
        {
            var container = new Mock<IConfigurationObjectsContainer>();

            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolversContainer(container.Object, null));
        }

        [Test]
        public void Test_Object_Container_Cant_Be_Null()
        {
            var provider = new Mock<IVariationsProvider>();

            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolversContainer(null, provider.Object));
        }

        [Test]
        public void Test_Resolvers_In_Container_Are_Correct()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 4);
            var container = Container(objects);
            var resolvers = container.ToList();

            // Permutations: [4,5,6], [4,6,5], [5,4,6], [5,6,4], [6,4,5], [6,5,4]

            var first = resolvers[0];
            Assert.AreEqual(1, first.Id);
            Assert.AreEqual(4, first.ResolveId(objects[0]));
            Assert.AreEqual(5, first.ResolveId(objects[1]));
            Assert.AreEqual(6, first.ResolveId(objects[2]));

            var fourth = resolvers[3];
            Assert.AreEqual(4, fourth.Id);
            Assert.AreEqual(5, fourth.ResolveId(objects[0]));
            Assert.AreEqual(6, fourth.ResolveId(objects[1]));
            Assert.AreEqual(4, fourth.ResolveId(objects[2]));
        }

        [Test]
        public void Test_Compose_Method_Dictionaries_Cant_Be_Null()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 4);
            var container = Container(objects);
            var resolvers = container.ToList();

            Assert.Throws<ArgumentNullException>(() => container.Compose(null, resolvers.First()));
            Assert.Throws<ArgumentNullException>(() => container.Compose(resolvers.First(), null));
        }

        [Test]
        public void Test_Compose_Method_With_Correct_Dictionaries()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 4);
            var container = Container(objects);
            var resolvers = container.ToList();

            // Permutations: [4,5,6], [4,6,5], [5,4,6], [5,6,4], [6,5,4], [6,4,5]
            
            var second = resolvers[1];
            var fourth = resolvers[3];

            var composed1 = container.Compose(second, fourth);
            Assert.AreSame(resolvers[2], composed1);

            var composed2 = container.Compose(fourth, second);
            Assert.AreSame(resolvers[4], composed2);
        }
    }
}