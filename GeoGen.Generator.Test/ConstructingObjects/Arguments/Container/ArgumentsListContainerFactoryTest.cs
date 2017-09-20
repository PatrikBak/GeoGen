using System;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingObjects.Arguments.Container
{
    [TestFixture]
    public class ArgumentsListContainerFactoryTest
    {
        [Test]
        public void Arguments_To_String_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsListContainerFactory(null));
        }

        [Test]
        public void Factory_Returns_Someting()
        {
            var provider = Utilities.SimpleMock<IArgumentsListToStringProvider>();
            var obj = new ArgumentsListContainerFactory(provider).CreateContainer();

            Assert.NotNull(obj);
        }
    }
}