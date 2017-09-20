using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.Test.TestHelpers;
using NUnit.Framework;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectsContainer
{
    [TestFixture]
    public class ConfigurationObjectsContainerTest
    {

        private static ConfigurationObjectsContainer Container()
        {
            var resolver = new DefaultObjectIdResolver();
            var defaultToString = new DefaultObjectToStringProvider(resolver);
            var argsProvider = new ArgumentsListToStringProvider(defaultToString);
            var defaultResolver = new DefaultObjectIdResolver();
            var provider = new DefaultFullObjectToStringProvider(argsProvider, defaultResolver);

            return new ConfigurationObjectsContainer(provider);
        }

        [Test]
        public void Test_Provider_Cant_Be_Null()
        {

            Assert.Throws<ArgumentNullException>(() => new ConfigurationObjectsContainer(null));
        }

    [Test]
        public void Initialization_Loose_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Initialization_Configuration_Cant_Contain_Null_Object()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                null
            };

            var configuration = ConfigurationObjects.AsConfiguration(looseObjects);

            Assert.Throws<ArgumentException>(() => Container().Initialize(configuration));
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
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

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
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));
            looseObjects.RemoveRange(0, 2);
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

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
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

            Assert.Throws<KeyNotFoundException>
            (
                () =>
                {
                    var x = container[4];
                }
            );
        }

        [Test]
        public void Added_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(null));
        }

        [Test]
        public void Added_Object_Cant_Have_Id()
        {
            // TODO: Can have, but will be overriden
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
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject = ConfigurationObjects.ConstructedObject(42, 0, args);

            var result = container.Add(constructedObject);

            Assert.AreEqual(result, constructedObject);
            Assert.AreEqual(4, result.Id ?? throw new Exception());
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
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject1 = ConfigurationObjects.ConstructedObject(42, 1, args);
            var constructedObject2 = ConfigurationObjects.ConstructedObject(42, 1, args);

            container.Add(constructedObject1);
            var result = container.Add(constructedObject2);

            Assert.AreEqual(result, constructedObject1);
            Assert.AreNotEqual(result, constructedObject2);
            Assert.AreEqual(4, result.Id ?? throw new Exception());
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

            var container = Container();
            container.Initialize(ConfigurationObjects.AsConfiguration(looseObjects));

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

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1);
            obj1 = container.Add(obj1);
            Assert.AreEqual(4, obj1.Id ?? throw new Exception());

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

            var obj2 = ConfigurationObjects.ConstructedObject(42, 0, args2);
            obj2 = container.Add(obj2);
            Assert.AreEqual(5, obj2.Id ?? throw new Exception());

            var obj1Copy = ConfigurationObjects.ConstructedObject(42, 0, args1, 4);

            var args3 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(obj1Copy),
                        new ObjectConstructionArgument(looseObjects[0])
                    }
                )
            };

            var obj3 = ConfigurationObjects.ConstructedObject(42, 0, args3);
            var result = container.Add(obj3);
            obj3 = result;
            Assert.AreEqual(obj2, result);

            var args4 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new ObjectConstructionArgument(obj2),
                new ObjectConstructionArgument(obj3)
            };

            var obj4 = ConfigurationObjects.ConstructedObject(42, 1, args4);
            container.Add(obj4);
            Assert.AreEqual(6, obj4.Id ?? throw new Exception());
            Assert.AreEqual(6, container.Count());
        }

        [Test]
        public void Test_Initialization_With_Constructed_Objects()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
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

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1);

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

            var obj2 = ConfigurationObjects.ConstructedObject(42, 0, args2);

            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj2};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            container.Initialize(configuration);
            Assert.AreEqual(5, container.Count());
            Assert.AreEqual(4, obj1.Id ?? throw new Exception());
            Assert.AreEqual(5, obj2.Id ?? throw new Exception());
        }

        [Test]
        public void Test_Incorrect_Initialization_With_Unconstructable_Objects()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
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

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1);

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

            var obj2 = ConfigurationObjects.ConstructedObject(42, 0, args2);

            var constructedObjects = new List<ConstructedConfigurationObject> {obj2, obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

//            Assert.Throws<InitializationException>(() => container.Initialize(configuration));
        }

        [Test]
        public void Test_Incorrect_Initialization_With_Duplicate_Object()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
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

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            // TODO: FIX
            //Assert.Throws<InitializationException>(() => container.Initialize(configuration));
        }

        [Test]
        public void Test_Incorrect_Initialization_With_Not_Added_Loose_Object()
        {
            var container = Container();
            var looseObject = new LooseConfigurationObject(ConfigurationObjectType.Point);

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]),
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObject)
                    }
                )
            };

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            // TODO: FIX
    //        Assert.Throws<InitializationException>(() => container.Initialize(configuration));
        }

        [Test]
        public void Test_Ids_Are_Ignored_During_Inicialization()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]) ,
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2]) 
                    }
                )
            };

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1, 3);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            container.Initialize(configuration);
            Assert.AreEqual(4, obj1.Id);
        }

        [Test]
        public void Test_Not_Initialiazed_Container_Usage()
        {
            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }

        [Test]
        public void Test_Incorrect_Reinitialization()
        {
            var container = Container();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[0]) ,
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[1]),
                        new ObjectConstructionArgument(looseObjects[2])
                    }
                )
            };

            var obj1 = ConfigurationObjects.ConstructedObject(42, 0, args1, 3);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            container.Initialize(configuration);

            try
            {
                container.Initialize(null);
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }
    }
}