using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationsHandling.ObjectsContainer;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.ToStringHelper;
using ConfigurationConstructor = GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.ConfigurationConstructor;

namespace GeoGen.Generator.Test.ConfigurationsHandling.ConfigurationsConstructing
{
    [TestFixture]
    public class ConfigurationConstructorTest
    {
        private static ConfigurationObjectsContainer _container;



        private static ConfigurationObjectsContainer Container(IEnumerable<LooseConfigurationObject> objects)
        {
            var resolver = new DefaultObjectIdResolver();
            var provider = new DefaultObjectToStringProvider(resolver);
            var argsToString = new ArgumentsListToStringProvider(provider);
            var defaultFullObjectToStringProvider = new DefaultFullObjectToStringProvider( argsToString, resolver);
            _container = new ConfigurationObjectsContainer(defaultFullObjectToStringProvider);
            _container.Initialize(AsConfiguration(objects));

            return _container;
        }

        private static ConfigurationConstructor Handler(IEnumerable<LooseConfigurationObject> objects)
        {
            var resolver = new DefaultObjectIdResolver();
            var provider = new DefaultObjectToStringProvider(resolver);
            var argsToString = new ArgumentsListToStringProvider(provider);
            var variations = new VariationsProvider<LooseConfigurationObject>();
            var configurationToString = new ConfigurationToStringProvider();
            var objectToStringFactory = new CustomFullObjectToStringProviderFactory(argsToString);
            var dictionaryObjectIdResolversContainer = new DictionaryObjectIdResolversContainer(variations);
            dictionaryObjectIdResolversContainer.Initialize(objects.ToList());
            var finder = new LeastConfigurationFinder(configurationToString, objectToStringFactory, dictionaryObjectIdResolversContainer);
            var argsContainerFactory = new ArgumentsListContainerFactory(argsToString);
            var fixer = new IdsFixerFactory(_container);

            return new ConfigurationConstructor(finder, fixer, argsContainerFactory);
        }

        [Test]
        public void Output_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Handler(Objects(1, ConfigurationObjectType.Point)).ConstructWrapper(null)
            );
        }

        [Test]
        public void Triangle_With_One_Midpoint_Symetry()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            _container = Container(looseObjects);
            var handler = Handler(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                var result = new List<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(o1),
                            new ObjectConstructionArgument(o2)
                        }
                    )
                };


                return result;
            }

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var testCases = new List<List<ConstructionArgument>>
                    {
                        Args(looseObjects[0], looseObjects[1]),
                        Args(looseObjects[0], looseObjects[2]),
                        Args(looseObjects[1], looseObjects[2])
                    }
                    .Select
                    (
                        list =>
                        {
                            var obj = ConstructedObject(42, 0, list);
                            obj = _container.Add(obj);
                            var objAsList = new List<ConstructedConfigurationObject> {obj};
                            var objectsMap = new ConfigurationObjectsMap(looseObjects);

                            var wrapper = new ConfigurationWrapper
                            {
                                Configuration = configuration,
                                ForbiddenArguments = new Dictionary<int, IArgumentsListContainer>(),
                                ConfigurationObjectsMap = objectsMap
                            };

                            return new ConstructorOutput
                            {
                                InitialConfiguration = wrapper,
                                ConstructedObjects = objAsList
                            };
                        }
                    );

            foreach (var testCase in testCases)
            {
                var result = handler.ConstructWrapper(testCase);
                var asString = ConfigurationAsString(result.Configuration);

                Assert.AreEqual("42({1;2})", asString);

                var dictionary1 = result.ForbiddenArguments;
                Assert.AreEqual(1, dictionary1.Count);
                Assert.AreEqual(1, dictionary1[42].Count());

                var dictionary2 = result.ConfigurationObjectsMap;
                Assert.AreEqual(1, dictionary2.Count);
                Assert.AreEqual(4, dictionary2[ConfigurationObjectType.Point].Count);
            }
        }

        [Test]
        public void Triangle_With_Two_Midpoints_Symetry()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };
            _container = Container(looseObjects);
            ;
            var handler = Handler(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                var result = new List<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(o1),
                            new ObjectConstructionArgument(o2)
                        }
                    )
                };


                return result;
            }

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var objects = new List<List<ConstructionArgument>>
                    {
                        Args(looseObjects[0], looseObjects[1]),
                        Args(looseObjects[0], looseObjects[2]),
                        Args(looseObjects[1], looseObjects[2])
                    }
                    .Select(arg => _container.Add(ConstructedObject(42, 0, arg)))
                    .ToList();


            var testCases = new List<List<ConstructedConfigurationObject>>
                    {
                        new List<ConstructedConfigurationObject> {objects[0], objects[1]},
                        new List<ConstructedConfigurationObject> {objects[0], objects[2]},
                        new List<ConstructedConfigurationObject> {objects[1], objects[2]}
                    }
                    .Select
                    (
                        list =>
                        {
                            var objectsMap = new ConfigurationObjectsMap(looseObjects);

                            var wrapper = new ConfigurationWrapper
                            {
                                Configuration = configuration,
                                ForbiddenArguments = new Dictionary<int, IArgumentsListContainer>(),
                                ConfigurationObjectsMap = objectsMap
                            };

                            return new ConstructorOutput
                            {
                                ConstructedObjects = list,
                                InitialConfiguration = wrapper
                            };
                        }
                    );

            foreach (var testCase in testCases)
            {
                var result = handler.ConstructWrapper(testCase);
                var asString = ConfigurationAsString(result.Configuration);

                Assert.AreEqual("42({1;2})|42({1;3})", asString);

                var dictionary1 = result.ForbiddenArguments;
                Assert.AreEqual(1, dictionary1.Count);
                Assert.AreEqual(2, dictionary1[42].Count());

                var dictionary2 = result.ConfigurationObjectsMap;
                Assert.AreEqual(1, dictionary2.Count);
                Assert.AreEqual(5, dictionary2[ConfigurationObjectType.Point].Count);
            }
        }

        [Test]
        public void Forbidden_Args_Correction_Test()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
            };
            _container = Container(looseObjects);
            var handler = Handler(looseObjects);

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var list = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[0])
            };

            var obj = ConstructedObject(42, 0, list);
            obj = _container.Add(obj);
            var objAsList = new List<ConstructedConfigurationObject> {obj};
            var objectsMap = new ConfigurationObjectsMap(looseObjects);

            var argsProvider = new ArgumentsListToStringProvider(new DefaultObjectToStringProvider(new DefaultObjectIdResolver()));
            var forbidden = new ArgumentsListContainer(argsProvider);
            var forbiddenArgs1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0])
            };
            var forbiddenArgs2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[1])
            };
            forbidden.AddArguments(forbiddenArgs1);
            forbidden.AddArguments(forbiddenArgs2);

            var forbidenArgs = new Dictionary<int, IArgumentsListContainer> {{42, forbidden}};

            var wrapper = new ConfigurationWrapper
            {
                Configuration = configuration,
                ForbiddenArguments = forbidenArgs,
                ConfigurationObjectsMap = objectsMap
            };

            var output = new ConstructorOutput
            {
                ConstructedObjects = objAsList,
                InitialConfiguration = wrapper
            };

            var result = handler.ConstructWrapper(output);
            var asString = ConfigurationAsString(result.Configuration);

            Assert.AreEqual("42(1,2,3)", asString);

            var dictionary1 = result.ForbiddenArguments;
            Assert.AreEqual(1, dictionary1.Count);

            var argumentsContainer = dictionary1[42];
            Assert.AreEqual(3, argumentsContainer.Count());

            var strings = argumentsContainer.Select(ArgsToString).ToList();

            Assert.IsTrue(strings.Contains("(2,1,3)"));
            Assert.IsTrue(strings.Contains("(1,3,2)"));

            var dictionary2 = result.ConfigurationObjectsMap;
            Assert.AreEqual(3, dictionary2.Count);
            Assert.AreEqual(4, dictionary2[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Circle].Count);

            Assert.IsFalse(dictionary2[ConfigurationObjectType.Point].Contains(obj));
        }

        [Test]
        public void Complex_Triangle_Configuration_Test()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };
            _container = Container(looseObjects);
            var handler = Handler(looseObjects);

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            ConstructedConfigurationObject Obj(List<ConstructionArgument> args)
            {
                return ConstructedObject(42, 0, args);
            }

            var list1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[2])
            };

            var obj1 = Obj(list1);
            obj1 = _container.Add(obj1);

            var list2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0])
            };

            var obj2 = Obj(list2);
            obj2 = _container.Add(obj2);

            var list3 = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[2]),
                        new ObjectConstructionArgument(obj1)
                    }
                )
            };

            var obj3 = Obj(list3);
            obj3 = _container.Add(obj3);

            var list4 = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[0]),
                        new ObjectConstructionArgument(obj2)
                    }
                )
            };

            var obj4 = Obj(list4);
            obj4 = _container.Add(obj4);

            var objAsList = new List<ConstructedConfigurationObject> {obj1, obj2, obj3, obj4};
            var objectsMap = new ConfigurationObjectsMap(looseObjects);

            var wrapper = new ConfigurationWrapper
            {
                Configuration = configuration,
                ForbiddenArguments = new Dictionary<int, IArgumentsListContainer>(),
                ConfigurationObjectsMap = objectsMap
            };

            var output = new ConstructorOutput
            {
                InitialConfiguration = wrapper,
                ConstructedObjects = objAsList
            };

            var result = handler.ConstructWrapper(output);
            var asString1 = ConfigurationAsString(result.Configuration);

            looseObjects[0].Id = 3;
            looseObjects[2].Id = 1;

            var asString2 = ConfigurationAsString(result.Configuration);

            Assert.AreEqual(asString1, asString2);
            Console.WriteLine(asString1);
        }
    }
}