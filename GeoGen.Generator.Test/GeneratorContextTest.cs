using System;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test
{
    [TestFixture]
    public class GeneratorContextTest
    {
        private static T SimpleMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }

        [Test]
        public void Test_Constructor_Everything_Not_Null()
        {
            new GeneratorContext(
                SimpleMock<IConfigurationContainer>(),
                SimpleMock<IConfigurationsHandler>(),
                SimpleMock<IConfigurationConstructor>());
        }

        [Test]
        public void Test_Constructor_Container_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new GeneratorContext(
                    null,
                    SimpleMock<IConfigurationsHandler>(),
                    SimpleMock<IConfigurationConstructor>());
            });
        }

        [Test]
        public void Test_Constructor_Handler_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new GeneratorContext(
                    SimpleMock<IConfigurationContainer>(),
                    null,
                    SimpleMock<IConfigurationConstructor>());
            });
        }

        [Test]
        public void Test_Constructor_Constructor_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new GeneratorContext(
                    SimpleMock<IConfigurationContainer>(),
                    SimpleMock<IConfigurationsHandler>(),
                    null);
            });
        }
    }
}