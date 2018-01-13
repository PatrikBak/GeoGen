using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace GeoGen.Core.Test.Constructions
{
    [TestFixture]
    public class ComposedConstructionTest
    {
        private static Configuration Configuration()
        {
            var construction = new Mock<Construction>();
            var outputTypes = new List<ConfigurationObjectType> {ConfigurationObjectType.Point, ConfigurationObjectType.Circle};
            construction.Setup(c => c.OutputTypes).Returns(outputTypes);
            var objectsMock = new Mock<ConfigurationObject>();
            var arguments = new List<ConstructionArgument> {new ObjectConstructionArgument(objectsMock.Object)};
            var constructon = construction.Object;
            var constructedObject = new ConstructedConfigurationObject(constructon, arguments, 0);
            var constructedObjects = new List<ConstructedConfigurationObject> {constructedObject, constructedObject};
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var looseObjects = new List<LooseConfigurationObject> {looseObject};

            return new Configuration(looseObjects, constructedObjects);
        }

        [Test]
        public void CTest_Parental_Configuration_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ComposedConstruction(null, new List<int> {0}, new List<ConstructionParameter>())
            );
        }

        [Test]
        public void Test_Output_Object_Indices_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ComposedConstruction(Configuration(), null, new List<ConstructionParameter>())
            );
        }

        [Test]
        public void Test_Output_Object_Indices_Cant_Be_Empty()
        {
            Assert.Throws<ArgumentException>
            (
                () => new ComposedConstruction(Configuration(), new List<int>(), new List<ConstructionParameter>())
            );
        }

        [Test]
        public void Test_Output_Object_Indices_Must_Correspond_To_Existing_Objects()
        {
            Assert.Throws<ArgumentException>
            (
                () => new ComposedConstruction(Configuration(), new List<int> {1, 2}, new List<ConstructionParameter>())
            );
        }

        [Test]
        public void Test_Output_Object_Indices_Are_Correct()
        {
            var parameters = new List<ConstructionParameter> {new ObjectConstructionParameter(ConfigurationObjectType.Point)};
            new ComposedConstruction(Configuration(), new List<int> {0, 1}, parameters);
        }

        [Test]
        public void Test_Construction_Parameters_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new ComposedConstruction(Configuration(), new List<int> {0}, null)
            );
        }

        [Test]
        public void Constructor_Test_Construction_Parameters_Cant_Be_Empty()
        {
            Assert.Throws<ArgumentException>
            (
                () => new ComposedConstruction(Configuration(), new List<int> {0}, new List<ConstructionParameter>())
            );
        }
    }
}