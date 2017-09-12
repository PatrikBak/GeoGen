using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.ConfigurationHandling.SymetricConfigurationsHandler;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationsContainer
{
    [TestFixture]
    public class ConfigurationsContainerTest
    {
        private static T SimpleMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }

        private static ConfigurationContainer Container()
        {
            var argumentsProvider = new ArgumentsToStringProvider();
            var argumentsContainerFactory = new ArgumentsContainerFactory(argumentsProvider);
            var configurationToStringProvider = new ConfigurationToStringProvider();
            var resolver = new DefaultComplexConfigurationObjectToStringProvider(argumentsProvider);
            var configuationObjectContainer = new ConfigurationObjectsContainer(resolver);

            var mock = new Mock<ISymetricConfigurationsHandler>();
            mock.Setup(
                        s => s.CreateSymetryClassRepresentant(
                            It.IsAny<ConfigurationWrapper>(), It.IsAny<List<ConstructedConfigurationObject>>()))
                    .Returns<ConfigurationWrapper, List<ConstructedConfigurationObject>>(
                        (w, c) =>
                        {
                            var newConfiguration = new Configuration(
                                w.Configuration.LooseObjects,
                                w.Configuration.ConstructedObjects.Union(c).ToList());

                            return new ConfigurationWrapper
                            {
                                Configuration = newConfiguration,
                            };
                        });

            var symetryHandler = mock.Object;

            return new ConfigurationContainer(argumentsContainerFactory, symetryHandler, configurationToStringProvider, configuationObjectContainer);
        }

        private static Construction Construction()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.Id).Returns(42);
            mock.Setup(s => s.OutputTypes).Returns(
                new List<ConfigurationObjectType>
                {
                    ConfigurationObjectType.Point,
                    ConfigurationObjectType.Point
                });

            return mock.Object;
        }

        [Test]
        public void Test_Argumnts_Container_Factory_Not_Null()
        {
            var handler = SimpleMock<ISymetricConfigurationsHandler>();
            var provider = SimpleMock<IConfigurationToStringProvider>();
            var container = SimpleMock<IConfigurationObjectsContainer>();

            Assert.Throws<ArgumentNullException>(() => new ConfigurationContainer(null, handler, provider, container));
        }

        [Test]
        public void Test_Symetric_Configuration_Handler_Not_Null()
        {
            var factory = SimpleMock<IArgumentsContainerFactory>();
            var provider = SimpleMock<IConfigurationToStringProvider>();
            var container = SimpleMock<IConfigurationObjectsContainer>();

            Assert.Throws<ArgumentNullException>(() => new ConfigurationContainer(factory, null, provider, container));
        }

        [Test]
        public void Test_Configuration_To_String_Provider_Not_Null()
        {
            var factory = SimpleMock<IArgumentsContainerFactory>();
            var handler = SimpleMock<ISymetricConfigurationsHandler>();
            var container = SimpleMock<IConfigurationObjectsContainer>();

            Assert.Throws<ArgumentNullException>(() => new ConfigurationContainer(factory, handler, null, container));
        }

        [Test]
        public void Test_Objects_Container_Not_Null()
        {
            var factory = SimpleMock<IArgumentsContainerFactory>();
            var handler = SimpleMock<ISymetricConfigurationsHandler>();
            var provider = SimpleMock<IConfigurationToStringProvider>();

            Assert.Throws<ArgumentNullException>(() => new ConfigurationContainer(factory, handler, provider, null));
        }

        [Test]
        public void Test_Initial_Configuration_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Test_Initialization_Is_Succesful()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

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

            var obj1 = new ConstructedConfigurationObject(Construction(), args1, 0);

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

            var obj2 = new ConstructedConfigurationObject(Construction(), args2, 1);

            var configuration = new Configuration(
                new HashSet<LooseConfigurationObject>(looseObjects),
                new List<ConstructedConfigurationObject>
                {
                    obj1,
                    obj2
                });

            Assert.AreEqual(0, container.CurrentLayer.Count);

            container.Initialize(configuration);

            var currentLayer = container.CurrentLayer;
            Assert.AreEqual(1, currentLayer.Count);

            var currentWrapper = currentLayer[0];
            Assert.AreEqual(configuration, currentWrapper.Configuration);

            var forbidden = currentWrapper.ConstructionIdToForbiddenArguments;
            Assert.AreEqual(1, forbidden.Count);
            Assert.AreEqual(2, forbidden[42].Count());

            var objectsMap = currentWrapper.ObjectTypeToObjects;
            Assert.AreEqual(3, objectsMap.Count);
            Assert.AreEqual(3, objectsMap[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(1, objectsMap[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(1, objectsMap[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Test_New_Layer_Is_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().AddLayer(null));
        }

        [Test]
        public void Test_Adding_Many_Configurations_More_Than_Once()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var initial = new Configuration(new HashSet<LooseConfigurationObject>(looseObjects), new List<ConstructedConfigurationObject>());
            container.Initialize(initial);

            var wrapper = container.CurrentLayer[0];

            var objects = new List<ConfigurationObject>();
            objects.AddRange(looseObjects);

            var layer = new List<ConstructorOutput>();

            var count = objects.Count;

            for (var i = 0; i < 150; i++)
            {
                for (var a = 0; a < count; a++)
                {
                    for (var b = 0; b < count; b++)
                    {
                        var aObj = objects[a];
                        var bObj = objects[b];

                        if (aObj == bObj)
                            continue;

                        var args = new List<ConstructionArgument>
                        {
                            new SetConstructionArgument(
                                new HashSet<ConstructionArgument>
                                {
                                    new ObjectConstructionArgument(aObj),
                                    new ObjectConstructionArgument(bObj)
                                })
                        };

                        var obj = new ConstructedConfigurationObject(Construction(), args, 0);

                        var output = new ConstructorOutput
                        {
                            InitialConfiguration = wrapper,
                            ConstructedObjects = new List<ConstructedConfigurationObject> {obj}
                        };

                        layer.Add(output);
                    }
                }
            }

            container.AddLayer(layer);

            Assert.AreEqual(21, container.CurrentLayer.Count);
        }
    }
}