using System;
using GeoGen.Core.Constructions.Parameters;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions.Parameters
{
    [TestFixture]
    public class SetConstructionParameterTest
    {
        [Test]
        public void SetConstructionParameter_Construction_Parameter_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var setConstructionParameter = new SetConstructionParameter(null, 42);
            });
        }

        [Test]
        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        public void SetConstructionParameter_Number_Of_Parameters_Cannot_Be_Less_Than_One(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mock = new Mock<ConstructionParameter>();
                var constructionParameter = mock.Object;
                var setConstructionParameter = new SetConstructionParameter(constructionParameter, number);
            });
        }

        [Test]
        [TestCase(42)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void SetConstructionParameter_Number_Of_Parameters_Is_Correct(int number)
        {
            var mock = new Mock<ConstructionParameter>();
            var constructionParameter = mock.Object;
            var setConstructionParameter = new SetConstructionParameter(constructionParameter, number);
        }
    }
}