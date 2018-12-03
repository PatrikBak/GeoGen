﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions.Arguments
{
    [TestFixture]
    public class SetConstructionArgumentTest
    {
        [Test]
        public void Test_Passed_Arguments_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SetConstructionArgument(null));
        }

        [Test]
        public void Test_Passed_Arguments_Size_Cant_Be_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () =>
                {
                    var emptySet = new List<ConstructionArgument>();
                    new SetConstructionArgument(emptySet);
                }
            );
        }

        [Test]
        public void Test_Passed_Arguments_Size_Cant_Be_One()
        {
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () =>
                {
                    var mock = new Mock<ConstructionArgument>();
                    var set = new List<ConstructionArgument> {mock.Object};
                    new SetConstructionArgument(set);
                }
            );
        }
    }
}