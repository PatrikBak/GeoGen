using System;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions.Parameters
{
    [TestFixture]
    public class SetConstructionParameterTest
    {
        [Test]
        public void Test_Construction_Parameter_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SetConstructionParameter(null, 42));
        }

        [TestCase(-42)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        public void Test_Number_Of_Parameters_Cant_Be_Less_Than_One(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () =>
                {
                    var mock = new Mock<ConstructionParameter>();
                    var constructionParameter = mock.Object;
                    new SetConstructionParameter(constructionParameter, number);
                }
            );
        }

        [TestCase(42)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Test_Number_Of_Parameters_Is_Correct(int number)
        {
            var mock = new Mock<ConstructionParameter>();
            var constructionParameter = mock.Object;
            new SetConstructionParameter(constructionParameter, number);
        }
    }
}