using GeoGen.Analyzer.Objects;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Objects
{
    [TestFixture]
    public class ObjectsContainersFactoryTest
    {
        [Test]
        public void Test_Factory_Creates_An_Objects()
        {
            Assert.NotNull(new ObjectsContainersFactory().CreateContainer());
        }
    }
}