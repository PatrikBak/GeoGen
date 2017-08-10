using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions.Arguments
{
    [TestFixture]
    public class SetConstructionArgumentTest
    {
        [Test]
        public void SetConstructionArgument_Passed_Arguments_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var setConstructionArgument = new SetConstructionArgument(null);
            });
        }

        [Test]
        public void SetConstructionArgument_Passed_Arguments_Size_Cannot_Be_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var emptySet = new HashSet<ConstructionArgument>();
                var objectConstructionArgument = new SetConstructionArgument(emptySet);
            });
        }

        [Test]
        public void SetConstructionArgument_Passed_Arguments_Size_Cannot_Be_One()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var mock = new Mock<ConstructionArgument>();
                var set = new HashSet<ConstructionArgument> {mock.Object};
                var objectConstructionArgument = new SetConstructionArgument(set);
            });
        }
    }
}