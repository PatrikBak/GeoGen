using System;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.Container
{
    [TestFixture]
    public class ArgumentsContainerFactoryTest
    {
        [Test]
        public void Arguments_To_String_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsContainerFactory(null));
        }

        [Test]
        public void Factory_Returns_Someting()
        {
            var provider = Utilities.SimpleMock<IArgumentsToStringProvider>();
            var obj = new ArgumentsContainerFactory(provider).CreateContainer();

            Assert.NotNull(obj);
        }
    }
}