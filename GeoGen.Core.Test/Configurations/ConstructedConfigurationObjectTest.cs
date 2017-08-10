using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Configurations
{
    public class ConstructedConfigurationObjectTest
    {
        [Test]
        public void ConstructedConfigurationObject_Construction_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var constructedObject = new ConstructedConfigurationObject(null, new List<ConstructionArgument>());
            });
        }

        [Test]
        public void ConstructedConfigurationObject_Passed_Argument_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var mock = new Mock<Construction>();
                var constructon = mock.Object;
                var constructedObject = new ConstructedConfigurationObject(constructon, null);
            });
        }
    }
}