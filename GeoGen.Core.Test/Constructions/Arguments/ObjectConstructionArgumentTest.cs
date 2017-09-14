using System;
using GeoGen.Core.Constructions.Arguments;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions.Arguments
{
    [TestFixture]
    public class ObjectConstructionArgumentTest
    {
        [Test]
        public void ObjectConstruction_Passed_Configuration_Object_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectConstructionArgument(null));
        }
    }
}