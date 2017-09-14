using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Constructing.Arguments.Container;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.ToStringHelper;
using ConfigurationConstructor = GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.ConfigurationConstructor;

namespace GeoGen.Generator.Test.ConfigurationHandling.ConfigurationsConstructing
{
    [TestFixture]
    public class ConfigurationConstructorTest
    {
        private static ConfigurationObjectsContainer _container;

        private static ConfigurationConstructor Handler()
        {
            var provider = new DefaultConfigurationObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(provider);
            var variations = new VariationsProvider<int>();
            var configurationToString = new ConfigurationToStringProvider();
            var objectToStringFactory = new ConfigurationObjectToStringProviderFactory(argsToString);
            var finder = new LeastConfigurationFinder(variations, configurationToString, objectToStringFactory);
            var factory = new ArgumentsContainerFactory(argsToString);
            var resolver = new DefaultObjectIdResolver();
            var complexObjToString = new DefaultComplexConfigurationObjectToStringProvider(argsToString, resolver);
            var container = new ConfigurationObjectsContainer(complexObjToString);
            var fixer = new IdsFixer(container);

            _container = container;

            return new ConfigurationConstructor(finder, fixer, factory);
        }

        [Test]
        public void Output_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Handler().ConstructWrapper(null));
        }

        [Test]
        public void Triangle_With_One_Midpoint_Symetry()
        {
            var handler = Handler();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            _container.Initialize(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                return new List<ConstructionArgument>
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
                                ForbiddenArguments = new Dictionary<int, IArgumentsContainer>(),
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

                Assert.AreEqual("1|2|3|42({1;2})[0]", asString);

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
            var handler = Handler();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            _container.Initialize(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                return new List<ConstructionArgument>
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
                                ForbiddenArguments = new Dictionary<int, IArgumentsContainer>(),
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

                Assert.AreEqual("1|2|3|42({1;2})[0]|42({1;3})[0]", asString);

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
            var handler = Handler();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Line)
            };

            _container.Initialize(looseObjects);

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

            var objectProvider = new DefaultConfigurationObjectToStringProvider();
            var argsProvider = new ArgumentsToStringProvider(objectProvider);
            var forbidden = new ArgumentsContainer(argsProvider);
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

            var forbidenArgs = new Dictionary<int, IArgumentsContainer> {{42, forbidden}};

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

            Assert.AreEqual("1|2|3|4|42(1,2,3)[0]|5|6|7", asString);

            var dictionary1 = result.ForbiddenArguments;
            Assert.AreEqual(1, dictionary1.Count);

            var container = dictionary1[42];
            Assert.AreEqual(3, container.Count());

            var strings = container.Select(ArgsToString).ToList();

            Assert.IsTrue(strings.Contains("(2, 1, 3)"));
            Assert.IsTrue(strings.Contains("(1, 3, 2)"));

            var dictionary2 = result.ConfigurationObjectsMap;
            Assert.AreEqual(3, dictionary2.Count);
            Assert.AreEqual(4, dictionary2[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Circle].Count);
        }

        [Test]
        public void Complex_Triangle_Configuration_Test()
        {
            var handler = Handler();

            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            _container.Initialize(looseObjects);

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
                ForbiddenArguments = new Dictionary<int, IArgumentsContainer>(),
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