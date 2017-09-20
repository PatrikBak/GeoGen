using System;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConstructingObjects.Arguments.Container
{
    [TestFixture]
    public class ArgumentsListContainerFactoryTest
    {
        [Test]
        public void Test_Arguments_To_String_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsListContainerFactory(null));
        }

        [Test]
        public void Test_Factory_Returns_Distint_Objects()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();
            var factory = new ArgumentsListContainerFactory(provider);

            var obj1 = factory.CreateContainer();
            var obj2 = factory.CreateContainer();

            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.AreNotSame(obj1, obj2);
        }
    }
}