using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString
{
    [TestFixture]
    public class ConfigurationObjectToStringProviderTest
    {
        private static IArgumentsToStringProvider ArgumentsProvider()
        {
            var mock = new Mock<IArgumentsToStringProvider>();

            mock.Setup(s => s.ConvertToString(It.IsAny<IReadOnlyList<ConstructionArgument>>()))
                    .Returns("args");

            return mock.Object;
        }

        private static ConfigurationObjectToStringProvider Provider()
        {
            return new ConfigurationObjectToStringProvider(ArgumentsProvider());
        }

        [Test]
        public void Arguments_Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Provider().ConvertToString(null));
        }

        [Test]
        public void Loose_Objects_To_String_Test()
        {
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};
            
            Assert.AreEqual("42", Provider().ConvertToString(looseObject));
        }

        public void Contructed_Object_To_String_Test()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(7);

            var args = Enumerable.Range(0, 4)
                    .Select(i => new ObjectConstructionArgument(new LooseConfigurationObject(ConfigurationObjectType.Point)))
                    .ToList();

            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1);

            Assert.AreEqual("7-args-1", Provider().ConvertToString(constructedObject));
        }
    }
}