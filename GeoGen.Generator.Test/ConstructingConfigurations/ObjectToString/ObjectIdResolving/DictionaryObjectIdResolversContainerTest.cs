using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
using GeoGen.Generator.Test.TestHelpers;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DictionaryObjectIdResolversContainerTest
    {
        private static DictionaryObjectIdResolversContainer Container()
        {
            var variationsProvider = new VariationsProvider();

           // return new DictionaryObjectIdResolversContainer(variationsProvider);
            return null;
        }

        [Test]
        public void Test_Variations_Provider_Cant_Be_Null()
        {
            //Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolversContainer(null));
        }

        [Test]
        public void Test_Cant_Iterate_Before_Initialization()
        {
            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }

        [Test]
        public void Test_Loose_Objects_List_Cant_Be_Null()
        {
            //Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Test_Loose_Object_List_Cant_Be_Empty()
        {
            //Assert.Throws<ArgumentException>(() => Container().Initialize(new List<LooseConfigurationObject>()));
        }

        [Test]
        public void Test_Cant_Iterate_After_Incorrect_Reinitialization()
        {
            var container = Container();
            //container.Initialize(ConfigurationObjects.Objects(2, ConfigurationObjectType.Point));

            try
            {
                //container.Initialize(null);
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }

        [Test]
        public void Test_Objects_Must_Have_Set_Ids()
        {
            var objects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
            };

            //Assert.Throws<GeneratorException>(() => Container().Initialize(objects));
        }

        [Test]
        public void Test_Resolvers_In_Container_Are_Correct()
        {
            var objects = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point, 4);
            var container = Container();
            //container.Initialize(objects);
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
    }
}