using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ObjectsContainer
{
    [TestFixture]
    public class ConfigurationObjectsContainerTest
    {
        private static ConfigurationObjectsContainer Container()
        {
            return new ConfigurationObjectsContainer(Provider());
        }

        private static DefaultComplexConfigurationObjectToStringProvider Provider()
        {
            return new DefaultComplexConfigurationObjectToStringProvider(ArgumentsProvider());
        }

        private static IArgumentsToStringProvider ArgumentsProvider()
        {
            return new ArgumentsToStringProvider();
        }

        [Test]
        public void Provider_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ConfigurationObjectsContainer(null));
        }

        [Test]
        public void Initialization_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Initialization_Loose_Objects_Cant_Contain_Null()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                null
            };

            Assert.Throws<ArgumentException>(() => Container().Initialize(looseObjects));
        }

        [Test]
        public void Initialization_Works()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);

            Assert.AreEqual(3, container.Count());
            Assert.AreEqual(ConfigurationObjectType.Line, container[2].ObjectType);
        }

        [Test]
        public void Reinitialization_Works()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);
            looseObjects.RemoveRange(0, 2);
            container.Initialize(looseObjects);

            Assert.AreEqual(1, container.Count());
            Assert.AreEqual(ConfigurationObjectType.Circle, container[1].ObjectType);
        }

        [Test]
        public void Indexer_Throws_Exception_For_Not_Existing_Objects()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);

            Assert.Throws<KeyNotFoundException>(
                () =>
                {
                    var unused = container[4];
                });
        }

        [Test]
        public void Added_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(null));
        }

        [Test]
        public void Added_Object_Cant_Have_Id()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1) {Id = 4};

            Assert.Throws<GeneratorException>(() => container.Add(constructedObject));
        }

        [Test]
        public void Test_Adding_Not_Equal_Object()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject = new ConstructedConfigurationObject(mock.Object, args, 1);

            var result = container.Add(constructedObject);

            Assert.AreEqual(result, constructedObject);
            Assert.AreEqual(4, result.Id.Value);
            Assert.AreEqual(constructedObject, container[4]);
        }

        [Test]
        public void Test_Added_Equal_Object()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            container.Initialize(looseObjects);

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject1 = new ConstructedConfigurationObject(mock.Object, args, 1);
            var constructedObject2 = new ConstructedConfigurationObject(mock.Object, args, 1);

            container.Add(constructedObject1);
            var result = container.Add(constructedObject2);

            Assert.AreEqual(result, constructedObject1);
            Assert.AreNotEqual(result, constructedObject2);
            Assert.AreEqual(4, result.Id.Value);
            Assert.AreEqual(result, container[4]);
        }

        [Test]
        public void Complex_Objects_Test()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            var container = Container();
            container.Initialize(looseObjects);

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]),
                new SetConstructionArgument(
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2])
                    })
            };

            var obj1 = new ConstructedConfigurationObject(mock.Object, args1, 0);
            obj1 = container.Add(obj1);
            Assert.AreEqual(4, obj1.Id.Value);

            var args2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new SetConstructionArgument(
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(obj1),
                        new ObjectConstructionArgument(looseObjects[0])
                    })
            };

            var obj2 = new ConstructedConfigurationObject(mock.Object, args2, 0);
            obj2 = container.Add(obj2);
            Assert.AreEqual(5, obj2.Id.Value);

            var obj1Copy = new ConstructedConfigurationObject(mock.Object, args1, 0) { Id = 4 };

            var args3 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new SetConstructionArgument(
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(obj1Copy),
                        new ObjectConstructionArgument(looseObjects[0])
                    })
            };

            var obj3 = new ConstructedConfigurationObject(mock.Object, args3, 0);
            var result = container.Add(obj3);
            obj3 = result;
            Assert.AreEqual(obj2, result);

            var args4 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new ObjectConstructionArgument(obj2),
                new ObjectConstructionArgument(obj3)
            };

            var obj4 = new ConstructedConfigurationObject(mock.Object, args4, 1);
            container.Add(obj4);
            Assert.AreEqual(6, obj4.Id.Value);
            Assert.AreEqual(6, container.Count());
        }
    }
}
