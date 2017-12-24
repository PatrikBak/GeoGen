using System;
using System.Collections.Generic;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConstructingConfigurations.IdsFixing
{
    [TestFixture]
    public class IdsFixerFactoryTest
    {
        private static IdsFixerFactory Factory()
        {
            var container = SimpleMock<IConfigurationObjectsContainer>();

            return new IdsFixerFactory(container);
        }

        [Test]
        public void Test_Objects_Container_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new IdsFixerFactory(null));
        }

        [Test]
        public void Test_Dictionary_Object_Id_Resolver_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Factory().CreateFixer(null));
        }

        [Test]
        public void Test_Caching_Is_Happening()
        {
            var factory = Factory();

            var dictionary1 = new DictionaryObjectIdResolver(1, new Dictionary<int, int>());
            var dictionary2 = new DictionaryObjectIdResolver(2, new Dictionary<int, int>());

            var fixer1 = factory.CreateFixer(dictionary1);
            var fixer2 = factory.CreateFixer(dictionary2);

            var fixer3 = factory.CreateFixer(dictionary1);
            var fixer4 = factory.CreateFixer(dictionary2);

            Assert.NotNull(fixer1);
            Assert.NotNull(fixer2);
            Assert.AreSame(fixer1, fixer3);
            Assert.AreSame(fixer2, fixer4);
            Assert.AreNotSame(fixer1, fixer2);
        }
    }
}