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
        public void Test_Construction_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ConstructedConfigurationObject(null, new List<ConstructionArgument>(), 0)
            );
        }

        [Test]
        public void Test_Passsed_Arguments_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    var mock = new Mock<Construction>();
                    var constructon = mock.Object;
                    new ConstructedConfigurationObject(constructon, null, 0);
                }
            );
        }

        [Test]
        public void Test_Passd_Arguments_Cant_Be_Empty()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var mock = new Mock<Construction>();
                    var constructon = mock.Object;
                    new ConstructedConfigurationObject(constructon, new List<ConstructionArgument>(), 0);
                }
            );
        }

        [TestCase(-42)]
        [TestCase(-2)]
        [TestCase(-1)]
        public void Test_Index_Cannot_Be_Less_Than_Zero(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () =>
                {
                    var mock = new Mock<Construction>();
                    var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Circle};
                    mock.Setup(construction => construction.OutputTypes).Returns(outputTypes);
                    var objectsMock = new Mock<ConfigurationObject>();
                    var arguments = new List<ConstructionArgument> {new ObjectConstructionArgument(objectsMock.Object)};
                    var constructon = mock.Object;
                    new ConstructedConfigurationObject(constructon, arguments, index);
                }
            );
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(42)]
        public void Test_Index_Cannot_Be_More_Than_Number_Of_Construction_Outputs(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () =>
                {
                    var mock = new Mock<Construction>();
                    var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Circle};
                    mock.Setup(construction => construction.OutputTypes).Returns(outputTypes);
                    var objectsMock = new Mock<ConfigurationObject>();
                    var arguments = new List<ConstructionArgument> {new ObjectConstructionArgument(objectsMock.Object)};
                    var constructon = mock.Object;
                    new ConstructedConfigurationObject(constructon, arguments, index);
                }
            );
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Test_Index_Is_Correct(int index)
        {
            var mock = new Mock<Construction>();
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Circle};
            mock.Setup(construction => construction.OutputTypes).Returns(outputTypes);
            var objectsMock = new Mock<ConfigurationObject>();
            var arguments = new List<ConstructionArgument> {new ObjectConstructionArgument(objectsMock.Object)};
            var constructon = mock.Object;
            new ConstructedConfigurationObject(constructon, arguments, index);
        }
    }
}