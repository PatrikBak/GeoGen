//using System.Collections.Generic;
//using System.Linq;
//using GeoGen.AnalyticalGeometry;
//using GeoGen.Core;
//using Moq;
//using NUnit.Framework;

//namespace GeoGen.Analyzer.Test.Constructing.ComposedConstructing
//{
//    [TestFixture]
//    public class ComposedConstructorTest
//    {
//        private PredefinedConstruction _midpoint;

//        private PredefinedConstruction _intersection;

//        [SetUp]
//        public void SetUp()
//        {
//            _midpoint = PredefinedConstructionsFactory.Get(PredefinedConstructionType.MidpointFromPoints);
//            _midpoint.Id = 1;

//            _intersection = PredefinedConstructionsFactory.Get(PredefinedConstructionType.IntersectionOfLinesFromPoints);
//            _intersection.Id = 2;
//        }

//        private static ComposedConstructor Constructor(ComposedConstruction construction)
//        {
//            var predefinedConstructors = new IPredefinedConstructor[]
//            {
//                new MidpointFromPointsConstructor(),
//                new CircumcenterFromPointsConstructor(),
//                new IntersectionOfLinesFromPointsConstructor(),
//            };

//            var factory = new Mock<IComposedConstructorFactory>();

//            var containersFactory = new Mock<IObjectsContainerFactory>();
//            containersFactory.Setup(s => s.CreateContainer()).Returns(() => new ObjectsContainer());

//            var resolver = new ConstructorsResolver(predefinedConstructors, factory.Object);

//            factory.Setup(s => s.Create(It.IsAny<ComposedConstruction>()))
//                    .Returns<ComposedConstruction>(c => new ComposedConstructor(c, resolver, containersFactory.Object));

//            return (ComposedConstructor) resolver.Resolve(construction);
//        }

//        private ComposedConstruction MidpointAsComposedConstruction()
//        {
//            var objects = Enumerable.Range(0, 2)
//                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
//                    .ToList();

//            var argument = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(objects[0]),
//                new ObjectConstructionArgument(objects[1])
//            });

//            var argumentsList = new List<ConstructionArgument> {argument};

//            var midpoint = new ConstructedConfigurationObject(_midpoint, argumentsList, 0) {Id = 2};

//            var constructedObjects = new List<ConstructedConfigurationObject> {midpoint};

//            var configuration = new Configuration(objects, constructedObjects);

//            var parameters = new List<ConstructionParameter>
//            {
//                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
//            };

//            return new ComposedConstruction(configuration, new List<int> {0}, parameters) {Id = 3};
//        }

//        private ComposedConstruction CentroidUsingComposedMidpoint()
//        {
//            var objects = Enumerable.Range(0, 3)
//                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
//                    .ToList();

//            var argument1 = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(objects[0]),
//                new ObjectConstructionArgument(objects[1])
//            });
//            var argument2 = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(objects[0]),
//                new ObjectConstructionArgument(objects[2])
//            });

//            var argumentsList1 = new List<ConstructionArgument> {argument1};
//            var argumentsList2 = new List<ConstructionArgument> {argument2};

//            var midpoint1 = new ConstructedConfigurationObject(MidpointAsComposedConstruction(), argumentsList1, 0) {Id = 3};
//            var midpoint2 = new ConstructedConfigurationObject(MidpointAsComposedConstruction(), argumentsList2, 0) {Id = 4};

//            var argument = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new SetConstructionArgument(new List<ConstructionArgument>
//                {
//                    new ObjectConstructionArgument(objects[2]),
//                    new ObjectConstructionArgument(midpoint1)
//                }),
//                new SetConstructionArgument(new List<ConstructionArgument>
//                {
//                    new ObjectConstructionArgument(objects[1]),
//                    new ObjectConstructionArgument(midpoint2)
//                })
//            });

//            var argumentsList = new List<ConstructionArgument> {argument};

//            var centroid = new ConstructedConfigurationObject(_intersection, argumentsList, 0) {Id = 5};

//            var constructedObjects = new List<ConstructedConfigurationObject> {midpoint1, midpoint2, centroid};

//            var configuration = new Configuration(objects, constructedObjects);

//            var parameters = new List<ConstructionParameter>
//            {
//                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
//            };

//            return new ComposedConstruction(configuration, new List<int> {2}, parameters) {Id = 4};
//        }

//        [Test]
//        public void Test_Of_Midpoint_With_Correct_Input()
//        {
//            var midpoint = MidpointAsComposedConstruction();

//            var constructor = Constructor(midpoint);

//            var looseObjects = Enumerable.Range(0, 2)
//                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
//                    .ToList();

//            var container = new ObjectsContainer();
//            Add(container, new Point(1, 2), looseObjects[0]);
//            Add(container, new Point(2, 3), looseObjects[1]);

//            var argument = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(looseObjects[0]),
//                new ObjectConstructionArgument(looseObjects[1])
//            });

//            var midpointObject = new ConstructedConfigurationObject(midpoint, new ConstructionArgument[] {argument}, 0);

//            var result = constructor.Construct(new List<ConstructedConfigurationObject> {midpointObject});

//            var output = result.ConstructorFunction(container);

//            Assert.AreEqual(new Point(1.5, 2.5), output[0]);
//        }

//        private void Add(IObjectsContainer container, AnalyticalObject analyticalObject, ConfigurationObject configurationObject)
//        {
//            List<AnalyticalObject> Function(IObjectsContainer c) => new List<AnalyticalObject> {analyticalObject};

//            container.Add(new[] {configurationObject}, Function);
//        }

//        [Test]
//        public void Test_Of_Centroid_With_Constructible_Result()
//        {
//            var centroid = CentroidUsingComposedMidpoint();

//            var constructor = Constructor(centroid);

//            var looseObjects = Enumerable.Range(0, 3)
//                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
//                    .ToList();

//            var container = new ObjectsContainer();
//            Add(container, new Point(1, 2), looseObjects[0]);
//            Add(container, new Point(2, 3), looseObjects[1]);
//            Add(container, new Point(7, 9), looseObjects[2]);

//            var argument = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(looseObjects[0]),
//                new ObjectConstructionArgument(looseObjects[1]),
//                new ObjectConstructionArgument(looseObjects[2])
//            });

//            var centroidObject = new ConstructedConfigurationObject(centroid, new ConstructionArgument[] {argument}, 0);

//            var result = constructor.Construct(new List<ConstructedConfigurationObject> {centroidObject});

//            var output = result.ConstructorFunction(container);

//            Assert.AreEqual(new Point(10.0 / 3, 14.0 / 3), output[0]);
//        }

//        [Test]
//        public void Test_Of_Centroid_With_Not_Constructible_Result()
//        {
//            var centroid = CentroidUsingComposedMidpoint();

//            var constructor = Constructor(centroid);

//            var looseObjects = Enumerable.Range(0, 3)
//                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
//                    .ToList();

//            var container = new ObjectsContainer();
//            Add(container, new Point(1, 2), looseObjects[0]);
//            Add(container, new Point(2, 3), looseObjects[1]);
//            Add(container, new Point(7, 8), looseObjects[2]);

//            var argument = new SetConstructionArgument(new List<ConstructionArgument>
//            {
//                new ObjectConstructionArgument(looseObjects[0]),
//                new ObjectConstructionArgument(looseObjects[1]),
//                new ObjectConstructionArgument(looseObjects[2])
//            });

//            var centroidObject = new ConstructedConfigurationObject(centroid, new ConstructionArgument[] {argument}, 0);

//            var result = constructor.Construct(new List<ConstructedConfigurationObject> {centroidObject});

//            var output = result.ConstructorFunction(container);

//            Assert.IsNull(output);
//        }
//    }
//}