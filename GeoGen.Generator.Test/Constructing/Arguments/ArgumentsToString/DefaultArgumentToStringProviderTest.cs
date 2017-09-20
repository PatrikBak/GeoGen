using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.Constructing.Arguments.ArgumentsToString
{
    [TestFixture]
    public class DefaultArgumentToStringProviderTest
    {
        private static DefaultArgumentToStringProvider Provider()
        {
            var resolver = new DefaultObjectIdResolver();
            var objectToString = new DefaultObjectToStringProvider(resolver);

            return new DefaultArgumentToStringProvider(objectToString, "; ");
        }

        [Test]
        public void Test_Default_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultArgumentToStringProvider(null));
        }

        [Test]
        public void Test_Argument_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertArgument(null));
        }

        [Test]
        public void Test_Simple_Object_Argument()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var arg = new ObjectConstructionArgument(obj);
            var asString = Provider().ConvertArgument(arg);

            Assert.AreEqual("1", asString);
        }

        [Test]
        public void Test_Constructed_Object_Argument()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var arg = new ObjectConstructionArgument(obj);
            var constructedObj = ConstructedObject(42, 0, new List<ConstructionArgument> {arg}, 1);
            var finalArg = new ObjectConstructionArgument(constructedObj);
            var asString = Provider().ConvertArgument(finalArg);

            Assert.AreEqual("1", asString);
        }

        [Test]
        public void Test_Complex_Set_Argument()
        {
            var objs = Objects(3, ConfigurationObjectType.Point);

            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(objs[0]),
                            new ObjectConstructionArgument(objs[1])
                        }
                    ),
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(objs[0]),
                            new ObjectConstructionArgument(objs[2])
                        }
                    )
                }
            );

            var asString = Provider().ConvertArgument(setArg);

            Assert.AreEqual("{{1; 2}; {1; 3}}", asString);
        }

        [Test]
        public void Test_Simple_Set_Argument()
        {
            var objs = Objects(2, ConfigurationObjectType.Point);
            var arg = new ObjectConstructionArgument(objs[0]);

            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    arg,
                    new ObjectConstructionArgument(objs[1]),
                }
            );

            var asString = Provider().ConvertArgument(setArg);
            Assert.AreEqual("{1; 2}", asString);
        }

        [Test]
        public void Test_Double_Cache_Call()
        {
            var provider = Provider();
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) { Id = 1 };
            var arg = new ObjectConstructionArgument(obj);
            var asString = provider.ConvertArgument(arg);
            provider.CacheArgument(1, asString);

            Assert.Throws<GeneratorException>(() => provider.CacheArgument(1, asString));
        }

        [Test]
        public void Test_Forgotten_Cache_Call()
        {
            var provider = Provider();
            var objs = Objects(2, ConfigurationObjectType.Point);
            var arg = new ObjectConstructionArgument(objs[0]);
            provider.ConvertArgument(arg);
            arg.Id = 1;
            
            var setArg = new SetConstructionArgument
            (
                new HashSet<ConstructionArgument>
                {
                    arg,
                    new ObjectConstructionArgument(objs[1])
                }
            );

            Assert.Throws<GeneratorException>(() => provider.ConvertArgument(setArg));
        }

        [Test]
        public void Test_Caching_Is_Happening()
        {
            var provider = Provider();
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1};
            var arg = new ObjectConstructionArgument(obj) {Id = 1};
            provider.CacheArgument(1, "test");
            var asString = provider.ConvertArgument(arg);

            Assert.AreEqual("test", asString);
        }

        [Test]
        public void Test_Converting_Argument_With_Id_And_Never_Converted()
        {
            var provider = Provider();
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) { Id = 1 };
            var arg = new ObjectConstructionArgument(obj) { Id = 1 };

            Assert.Throws<GeneratorException>(() => provider.ConvertArgument(arg));
        }
    }
}