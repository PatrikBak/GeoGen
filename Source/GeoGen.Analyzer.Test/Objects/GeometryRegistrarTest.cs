//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.AnalyticalGeometry;
//using GeoGen.AnalyticalGeometry.AnalyticalObjects;
//using GeoGen.Analyzer.Constructing;
//using GeoGen.Analyzer.Constructing.PredefinedConstructors;
//using GeoGen.Analyzer.Objects;
//using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
//using GeoGen.Analyzer.Theorems;
//using GeoGen.Core.Configurations;
//using GeoGen.Core.Constructions.Arguments;
//using GeoGen.Core.Constructions.PredefinedConstructions;
//using GeoGen.Core.Theorems;
//using GeoGen.Core.Utilities;
//using GeoGen.Utilities;
//using Moq;
//using NUnit.Framework;
//using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

//namespace GeoGen.Analyzer.Test.Objects
//{
//    [TestFixture]
//    public class GeometryRegistrarTest
//    {
//        private static IObjectsContainersManager _manager;

//        private static IContextualContainer _container;

//        private static GeometryRegistrar Registrar(params List<IAnalyticalObject>[] initialObjects)
//        {
//            var resolver = new ConstructorsResolver
//            (
//                new IPredefinedConstructor[]
//                {
//                    new InteresectionConstructor(),
//                    new MidpointConstructor()
//                }
//            );

//            var theorems = new List<Theorem>();

//            var theoremsContainer = new Mock<ITheoremsContainer>();
//            theoremsContainer.Setup(s => s.Add(It.IsAny<Theorem>())).Callback<Theorem>(theorems.Add);
//            theoremsContainer.Setup(s => s.GetEnumerator()).Returns(() => theorems.GetEnumerator());

//            var constructor = new Mock<ILooseObjectsConstructor>();
//            var currentContainers = 0;

//            constructor.Setup(s => s.Construct(It.IsAny<IEnumerable<LooseConfigurationObject>>()))
//                    .Returns(() => initialObjects[currentContainers++]);

//            //var factory = new ObjectsContainersFactory();
//            _manager = new ObjectsContainersManager(null, constructor.Object, Math.Max(1, initialObjects.Length));
//            var helper = new AnalyticalHelper();
//            _container = new ContextualContainer(_manager, helper);

//            return new GeometryRegistrar(null, resolver, theoremsContainer.Object, _container, _manager);
//        }

//        [Test]
//        public void Test_Resolver_Cant_Be_Null()
//        {
//            var theoremsContainer = SimpleMock<ITheoremsContainer>();
//            var contextualContainer = SimpleMock<IContextualContainer>();
//            var manager = SimpleMock<IObjectsContainersManager>();

//            //Assert.Throws<ArgumentNullException>(() => new GeometryRegistrar(null, theoremsContainer, contextualContainer, manager));
//        }

//        [Test]
//        public void Test_Theorems_Container_Cant_Be_Null()
//        {
//            var resolver = SimpleMock<IConstructorsResolver>();
//            var contextualContainer = SimpleMock<IContextualContainer>();
//            var manager = SimpleMock<IObjectsContainersManager>();

//            //Assert.Throws<ArgumentNullException>(() => new GeometryRegistrar(resolver, null, contextualContainer, manager));
//        }

//        [Test]
//        public void Test_Contextual_Container_Cant_Be_Null()
//        {
//            var resolver = SimpleMock<IConstructorsResolver>();
//            var theoremsContainer = SimpleMock<ITheoremsContainer>();
//            var manager = SimpleMock<IObjectsContainersManager>();

//            //Assert.Throws<ArgumentNullException>(() => new GeometryRegistrar(resolver, theoremsContainer, null, manager));
//        }

//        [Test]
//        public void Test_Object_Containers_Manager_Cant_Be_Null()
//        {
//            var resolver = SimpleMock<IConstructorsResolver>();
//            var theoremsContainer = SimpleMock<ITheoremsContainer>();
//            var contextualContainer = SimpleMock<IContextualContainer>();

//            //Assert.Throws<ArgumentNullException>(() => new GeometryRegistrar(resolver, theoremsContainer, contextualContainer, null));
//        }

//        [Test]
//        public void Test_Initialization_Configuration_Cant_Be_Null()
//        {
//            //Assert.Throws<ArgumentNullException>(() => Registrar().Initialize(null));
//        }

//        [Test]
//        public void Test_Initialization_With_Only_Loose_Objects()
//        {
//            var objects = new[]
//            {
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 5),
//                    new Point(5, 8)
//                },
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 6),
//                    new Point(3, 2)
//                }
//            };

//            var registrar = Registrar(objects);

//            var looseObjects = new List<LooseConfigurationObject>
//            {
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
//            };

//            var configuration = new Configuration(looseObjects.ToSet(), new List<ConstructedConfigurationObject>());

//            //registrar.Initialize(configuration);

//            var containers = _manager.ToList();

//            Assert.AreEqual(new Point(1, 2), containers[0].Get(looseObjects[0]));
//            Assert.AreEqual(new Point(4, 5), containers[0].Get(looseObjects[1]));
//            Assert.AreEqual(new Point(5, 8), containers[0].Get(looseObjects[2]));
//            Assert.AreEqual(new Point(1, 2), containers[1].Get(looseObjects[0]));
//            Assert.AreEqual(new Point(4, 6), containers[1].Get(looseObjects[1]));
//            Assert.AreEqual(new Point(3, 2), containers[1].Get(looseObjects[2]));

//            Assert.AreEqual(7, _container.Count());
//        }

//        [Test]
//        public void Test_Register_Objects_Not_Null()
//        {
//            Assert.Throws<ArgumentNullException>(() => Registrar().Register(null));
//        }

//        [Test]
//        public void Test_Registers_Objects_Are_Not_Empty()
//        {
//            Assert.Throws<ArgumentException>(() => Registrar().Register(new List<ConstructedConfigurationObject>()));
//        }

//        [Test]
//        public void Test_Register_With_Midpoints()
//        {
//            var objects = new[]
//            {
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 5),
//                    new Point(5, 8)
//                },
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 6),
//                    new Point(3, 2)
//                }
//            };

//            var registrar = Registrar(objects);

//            var looseObjects = new List<LooseConfigurationObject>
//            {
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
//            };

//            var configuration = new Configuration(looseObjects.ToSet(), new List<ConstructedConfigurationObject>());

//            //registrar.Initialize(configuration);

//            List<ConstructedConfigurationObject> Midpoint(ConfigurationObject o1, ConfigurationObject o2, int id)
//            {
//                var arguments = new List<ConstructionArgument>
//                {
//                    new SetConstructionArgument
//                    (
//                        new HashSet<ConstructionArgument>
//                        {
//                            new ObjectConstructionArgument(o1),
//                            new ObjectConstructionArgument(o2)
//                        }
//                    )
//                };

//                return new List<ConstructedConfigurationObject>
//                {
//                    new ConstructedConfigurationObject(new Midpoint(), arguments, 0) {Id = id}
//                };
//            }

//            var midpoint1 = Midpoint(looseObjects[0], looseObjects[1], 4);

//            var result1 = registrar.Register(midpoint1);
//            Assert.IsTrue(result1.CanBeConstructed);
//            Assert.IsTrue(result1.GeometricalDuplicates.Empty());
//            Assert.IsTrue(_manager.All(container => container.Get(midpoint1[0]) != null));

//            var result2 = registrar.Register(midpoint1);
//            Assert.AreSame(result1, result2);

//            var midpoint2 = Midpoint(looseObjects[0], looseObjects[1], 5);
//            var result3 = registrar.Register(midpoint2);
//            Assert.AreNotSame(result2, result3);
//            Assert.IsTrue(result3.CanBeConstructed);
//            Assert.AreEqual(1, result3.GeometricalDuplicates.Count);
//            Assert.AreSame(midpoint1[0], result3.GeometricalDuplicates[midpoint2[0]]);
//        }

//        [Test]
//        public void Test_Register_With_Midpoints_And_Intersection()
//        {
//            var objects = new[]
//            {
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 5),
//                    new Point(5, 8)
//                },
//                new List<IAnalyticalObject>
//                {
//                    new Point(1, 2),
//                    new Point(4, 6),
//                    new Point(3, 2)
//                }
//            };

//            var registrar = Registrar(objects);

//            var looseObjects = new List<LooseConfigurationObject>
//            {
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
//                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}
//            };

//            var configuration = new Configuration(looseObjects.ToSet(), new List<ConstructedConfigurationObject>());

//            //registrar.Initialize(configuration);

//            List<ConstructedConfigurationObject> Midpoint(ConfigurationObject o1, ConfigurationObject o2, int id)
//            {
//                var arguments = new List<ConstructionArgument>
//                {
//                    new SetConstructionArgument
//                    (
//                        new HashSet<ConstructionArgument>
//                        {
//                            new ObjectConstructionArgument(o1),
//                            new ObjectConstructionArgument(o2)
//                        }
//                    )
//                };

//                return new List<ConstructedConfigurationObject>
//                {
//                    new ConstructedConfigurationObject(new Midpoint(), arguments, 0) {Id = id}
//                };
//            }

//            var midpoint1 = Midpoint(looseObjects[0], looseObjects[1], 4);
//            var midpoint2 = Midpoint(looseObjects[0], looseObjects[2], 5);

//            registrar.Register(midpoint1);
//            registrar.Register(midpoint2);

//            var intersectionArguments = new List<ConstructionArgument>
//            {
//                new SetConstructionArgument
//                (
//                    new HashSet<ConstructionArgument>
//                    {
//                        new SetConstructionArgument
//                        (
//                            new HashSet<ConstructionArgument>
//                            {
//                                new ObjectConstructionArgument(looseObjects[1]),
//                                new ObjectConstructionArgument(looseObjects[2])
//                            }
//                        ),
//                        new SetConstructionArgument
//                        (
//                            new HashSet<ConstructionArgument>
//                            {
//                                new ObjectConstructionArgument(midpoint1[0]),
//                                new ObjectConstructionArgument(midpoint2[0])
//                            }
//                        )
//                    }
//                )
//            };

//            var intersection = new List<ConstructedConfigurationObject>
//            {
//                new ConstructedConfigurationObject(new Intersection(), intersectionArguments, 0) {Id = 6}
//            };

//            var result = registrar.Register(intersection);

//            Assert.IsFalse(result.CanBeConstructed);
//            Assert.IsTrue(result.GeometricalDuplicates.Empty());
//        }
//    }
//}