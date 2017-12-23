using System;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DefaultObjectIdResolverTest
    {
        private static DefaultObjectIdResolver Resolver()
        {
            return new DefaultObjectIdResolver();
        }

        [Test]
        public void Test_Passed_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Test_Passed_Object_Must_Have_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Passed_Correct_Object_With_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            var id = Resolver().ResolveId(obj);

            Assert.AreEqual(42, id);
        }

        [Test]
        public void Test_Id_Of_Resolver_Is_Set_To_Default()
        {
            var id = Resolver().Id;

            Assert.AreEqual(DefaultObjectIdResolver.DefaultId, id);
        }
    }
}