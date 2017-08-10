using System;
using GeoGen.Core.Constructions;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions
{
    [TestFixture]
    public class ComposedConstructionTest
    {
        [Test]
        public void ComposedConstructon_Configuration_Object_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ComposedConstruction(null);
            });
        }
    }
}