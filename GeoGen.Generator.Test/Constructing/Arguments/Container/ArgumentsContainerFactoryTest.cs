using System;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.Constructing.Arguments.Container
{
    [TestFixture]
    public class ArgumentsContainerFactoryTest
    {
        [Test]
        public void Arguments_To_String_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentsListContainerFactory(null));
        }

        [Test]
        public void Factory_Returns_Someting()
        {
            var provider = SimpleMock<IArgumentsListToStringProvider>();
            var obj = new ArgumentsListContainerFactory(provider).CreateContainer();

            Assert.NotNull(obj);
        }
    }
}