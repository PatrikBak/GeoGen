using System;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.Constructing.Arguments.Containers
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
            var provider = SimpleMock<IArgumentsListToStringProvider>();
            var obj = new ArgumentsListContainerFactory(provider).CreateContainer();

            Assert.NotNull(obj);
        }
    }
}