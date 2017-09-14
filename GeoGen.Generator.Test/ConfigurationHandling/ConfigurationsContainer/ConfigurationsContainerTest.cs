using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using Moq;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Utilities;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationsContainer
{
    [TestFixture]
    public class ConfigurationsContainerTest
    {
        private static ConfigurationContainer Container()
        {
            var provider = new DefaultConfigurationObjectToStringProvider();
            var argumentsProvider = new ArgumentsToStringProvider(provider);
            var argumentsContainerFactory = new ArgumentsContainerFactory(argumentsProvider);
            var configurationToStringProvider = new ConfigurationToStringProvider();
            var defaultResolver = new DefaultObjectIdResolver();
            var resolver = new DefaultComplexConfigurationObjectToStringProvider(argumentsProvider, defaultResolver);
            var configuationObjectContainer = new ConfigurationObjectsContainer(resolver);

            var mock = new Mock<IConfigurationConstructor>();
            mock.Setup(s => s.ConstructWrapper(It.IsAny<ConstructorOutput>()))
                    .Returns<ConstructorOutput>
                    (
                        output =>
                        {
                            var initialConfiguration = output.InitialConfiguration.Configuration;
                            var newObjects = output.ConstructedObjects;
                            
                            var newConfiguration = new Configuration
                            (
                                initialConfiguration.LooseObjects,
                                initialConfiguration.ConstructedObjects.Union(newObjects).ToList()
                            );

                            return new ConfigurationWrapper
                            {
                                Configuration = newConfiguration
                            };
                        }
                    );

            var symetryHandler = mock.Object;

            return new ConfigurationContainer(argumentsContainerFactory, symetryHandler, configurationToStringProvider, configuationObjectContainer);
        }

        [Test]
        public void Test_Argumnts_Container_Factory_Not_Null()
        {
            var handler = SimpleMock<IConfigurationConstructor>();
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
            var handler = SimpleMock<IConfigurationConstructor>();
            var container = SimpleMock<IConfigurationObjectsContainer>();

            Assert.Throws<ArgumentNullException>(() => new ConfigurationContainer(factory, handler, null, container));
        }

        [Test]
        public void Test_Objects_Container_Not_Null()
        {
            var factory = SimpleMock<IArgumentsContainerFactory>();
            var handler = SimpleMock<IConfigurationConstructor>();
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
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2])
                    }
                )
            };

            var obj1 = ConstructedObject(42, 0, args1);

            var args2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(obj1),
                        new ObjectConstructionArgument(looseObjects[0])
                    }
                )
            };

            var obj2 = ConstructedObject(42, 1, args2);

            var configuration = new Configuration
            (
                new HashSet<LooseConfigurationObject>(looseObjects),
                new List<ConstructedConfigurationObject> {obj1, obj2}
            );

            Assert.AreEqual(0, container.CurrentLayer.Count);

            container.Initialize(configuration);

            var currentLayer = container.CurrentLayer;
            Assert.AreEqual(1, currentLayer.Count);

            var currentWrapper = currentLayer[0];
            Assert.AreEqual(configuration, currentWrapper.Configuration);

            var forbidden = currentWrapper.ForbiddenArguments;
            Assert.AreEqual(1, forbidden.Count);
            Assert.AreEqual(2, forbidden[42].Count());

            var objectsMap = currentWrapper.ConfigurationObjectsMap;
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

            var initial = new Configuration
            (
                new HashSet<LooseConfigurationObject>(looseObjects),
                new List<ConstructedConfigurationObject>()
            );
            container.Initialize(initial);

            var wrapper = container.CurrentLayer[0];

            var objects = new List<ConfigurationObject>();
            objects.AddRange(looseObjects);

            var layer = new List<ConstructorOutput>();

            var count = objects.Count;

            for (var i = 0; i < 15; i++)
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
                            new SetConstructionArgument
                            (
                                new HashSet<ConstructionArgument>
                                {
                                    new ObjectConstructionArgument(aObj),
                                    new ObjectConstructionArgument(bObj)
                                }
                            )
                        };

                        var obj = ConstructedObject(42, 0, args);

                        var output = new ConstructorOutput
                        {
                            InitialConfiguration = wrapper,
                            ConstructedObjects = new List<ConstructedConfigurationObject> {obj}
                        };

                        layer.Add(output);
                    }
                }
            }

            Console.WriteLine($"Objects: {layer.Count}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            container.AddLayer(layer);
            stopwatch.Stop();
            Console.WriteLine($"Adding time {stopwatch.ElapsedMilliseconds}");

            Assert.AreEqual(21, container.CurrentLayer.Count);
        }
    }
}