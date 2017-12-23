using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Configurations;

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

            return null;
            //return new ConfigurationObjectsContainer(provider);
        }

        [Test]
        public void Test_Provider_Cant_Be_Null()
        {
            //Assert.Throws<ArgumentNullException>(() => new ConfigurationObjectsContainer(null));
        }

        [Test]
        public void Test_Passed_Configuration_Cant_Be_Null()
        {
            //Assert.Throws<ArgumentNullException>(() => Container().Initialize(null));
        }

        [Test]
        public void Test_Passed_Configuration_Cant_Contain_Null_Object()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                null
            };
            var configuration = AsConfiguration(looseObjects);

            //Assert.Throws<ArgumentException>(() => Container().Initialize(configuration));
        }

        [Test]
        public void Test_Initialization_Is_Successful()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            //container.Initialize(AsConfiguration(looseObjects));

            Assert.AreEqual(3, container.Count());
            Assert.AreEqual(ConfigurationObjectType.Line, container[2].ObjectType);
        }

        [Test]
        public void Test_Reinitialization_Is_Successful()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            //container.Initialize(AsConfiguration(looseObjects));
            looseObjects.RemoveRange(0, 2);
            //container.Initialize(AsConfiguration(looseObjects));

            Assert.AreEqual(1, container.Count());
            Assert.AreEqual(ConfigurationObjectType.Circle, container[1].ObjectType);
        }

        [Test]
        public void Test_Indexer_Throws_Exception_For_Not_Existing_Objects()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
           // container.Initialize(AsConfiguration(looseObjects));

            Assert.Throws<KeyNotFoundException>(
                () =>
                {
                    var unused = container[4];
                });
        }

        [Test]
        public void Test_Id_Of_Configuration_Objects_Objects_Will_Be_Ignored()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };

            var container = Container();
            var args1 = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var obj1 = ConstructedObject(42, 0, args1, 7);

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
            var obj2 = ConstructedObject(42, 0, args2, 7);

            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj2};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);
            //container.Initialize(configuration);

            Assert.AreEqual(5, container.Count());
            Assert.AreEqual(1, looseObjects[0].Id);
            Assert.AreEqual(2, looseObjects[1].Id);
            Assert.AreEqual(3, looseObjects[2].Id);
            Assert.AreEqual(4, obj1.Id);
            Assert.AreEqual(5, obj2.Id);
        }

        [Test]
        public void Test_Added_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Container().Add(null));
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
            //container.Initialize(AsConfiguration(looseObjects));

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject = ConstructedObject(42, 0, args);

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
            //container.Initialize(AsConfiguration(looseObjects));

            var args = new List<ConstructionArgument> {new ObjectConstructionArgument(looseObjects[0])};
            var constructedObject1 = ConstructedObject(42, 1, args);
            var constructedObject2 = ConstructedObject(42, 1, args);

            container.Add(constructedObject1);
            var result = container.Add(constructedObject2);

            Assert.AreEqual(result, constructedObject1);
            Assert.AreNotEqual(result, constructedObject2);
            Assert.AreEqual(4, result.Id ?? throw new Exception());
            Assert.AreEqual(result, container[4]);
        }

        [Test]
        public void Test_Adding_Complex_Objects()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var container = Container();
            //container.Initialize(AsConfiguration(looseObjects));

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
            var obj2 = ConstructedObject(42, 0, args2);
            obj2 = container.Add(obj2);
            Assert.AreEqual(5, obj2.Id ?? throw new Exception());

            var obj1Copy = ConstructedObject(42, 0, args1, 4);

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
            var obj3 = ConstructedObject(42, 0, args3);
            var result = container.Add(obj3);
            obj3 = result;
            Assert.AreEqual(obj2, result);

            var args4 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(obj1),
                new ObjectConstructionArgument(obj2),
                new ObjectConstructionArgument(obj3)
            };
            var obj4 = ConstructedObject(42, 1, args4);
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

            var obj2 = ConstructedObject(42, 0, args2);

            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj2};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            //container.Initialize(configuration);
            Assert.AreEqual(5, container.Count());
            Assert.AreEqual(4, obj1.Id);
            Assert.AreEqual(5, obj2.Id);
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

            var obj2 = ConstructedObject(42, 0, args2);

            var constructedObjects = new List<ConstructedConfigurationObject> {obj2, obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            //Assert.Throws<InitializationException>(() => container.Initialize(configuration));
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

            var obj1 = ConstructedObject(42, 0, args1);
            var obj1Copy = ConstructedObject(42, 0, args1);

            var constructedObjects1 = new List<ConstructedConfigurationObject> {obj1, obj1};
            var constructedObjects2 = new List<ConstructedConfigurationObject> {obj1, obj1Copy};

            var configuration1 = new Configuration(looseObjects.ToSet(), constructedObjects1);
            var configuration2 = new Configuration(looseObjects.ToSet(), constructedObjects2);

            //Assert.Throws<InitializationException>(() => container.Initialize(configuration1));
            //Assert.Throws<InitializationException>(() => container.Initialize(configuration2));
        }

        [Test]
        public void Test_Incorrect_Initialization_With_New_Loose_Objects()
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

            var obj1 = ConstructedObject(42, 0, args1);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1, obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            //Assert.Throws<InitializationException>(() => container.Initialize(configuration));
        }

        [Test]
        public void Test_Not_Initialiazed_Container_Usage()
        {
            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }

        [Test]
        public void Test_Incorrectly_Reinitialized_Container_Usage()
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

            var obj1 = ConstructedObject(42, 0, args1, 3);
            var constructedObjects = new List<ConstructedConfigurationObject> {obj1};
            var configuration = new Configuration(looseObjects.ToSet(), constructedObjects);

            //container.Initialize(configuration);

            try
            {
                //container.Initialize(null);
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.Throws<GeneratorException>(() => Container().FirstOrDefault());
        }
    }
}