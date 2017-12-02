using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Theorems.TheoremVerifiers;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test.Theorems.TheoremVerifiers
{
    [TestFixture]
    public class ConcurrencyVerifierTest
    {
        private static IObjectsContainersManager _containers;

        private static ConcurrencyVerifier Verifier(params Dictionary<ConfigurationObject, IAnalyticalObject>[] objects)
        {
            var helper = new AnalyticalHelper();
            var provider = new SubsetsProvider();

            var containers = objects
                    .Select
                    (
                        dictionary =>
                        {
                            var result = new ObjectsContainer();

                            foreach (var pair in dictionary)
                            {
                                result.Add(pair.Value, pair.Key);
                            }

                            return result;
                        }
                    )
                    .ToList();

            var manager = new Mock<IObjectsContainersManager>();

            manager.Setup(s => s.GetEnumerator()).Returns(() => containers.GetEnumerator());

            _containers = manager.Object;

            var container = new ContextualContainer(manager.Object, helper);

            if (!objects.Empty())
            {
                foreach (var configurationObject in objects[0].Keys)
                {
                    container.Add(configurationObject);
                }
            }

            return new ConcurrencyVerifier(container, helper, provider);
        }

        [Test]
        public void Test_Theorem_Type_Is_Correct()
        {
            Assert.AreEqual(TheoremType.ConcurrentLines, Verifier().TheoremType);
        }

        [Test]
        public void Test_Contextual_Container_Cant_Be_Null()
        {
            var helper = SimpleMock<IAnalyticalHelper>();
            var provider = SimpleMock<ISubsetsProvider>();

            Assert.Throws<ArgumentNullException>(() => new ConcurrencyVerifier(null, helper, provider));
        }

        [Test]
        public void Test_Analytical_Helper_Cant_Be_Null()
        {
            var container = SimpleMock<IContextualContainer>();
            var provider = SimpleMock<ISubsetsProvider>();

            Assert.Throws<ArgumentNullException>(() => new ConcurrencyVerifier(container, null, provider));
        }

        [Test]
        public void Test_Subsets_Provider_Cant_Be_Null()
        {
            var container = SimpleMock<IContextualContainer>();
            var helper = SimpleMock<IAnalyticalHelper>();

            Assert.Throws<ArgumentNullException>(() => new ConcurrencyVerifier(container, helper, null));
        }

        [Test]
        public void Test_Old_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Verifier().GetOutput(null, new ConfigurationObjectsMap()).ToList());
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Verifier().GetOutput(new ConfigurationObjectsMap(), null).ToList());
        }

        [Test]
        public void Test_Medians_Are_Concurrent()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(1, 4)},
                    {configurationObjects[1], new Point(-1, -1)},
                    {configurationObjects[2], new Point(7, -1)},
                    {configurationObjects[3], new Point(3, -1)},
                    {configurationObjects[4], new Point(4, 1.5)},
                    {configurationObjects[5], new Point(0, 1.5)}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 5)},
                    {configurationObjects[1], new Point(-2, -1)},
                    {configurationObjects[2], new Point(7, -1)},
                    {configurationObjects[3], new Point(2.5, -1)},
                    {configurationObjects[4], new Point(3.5, 2)},
                    {configurationObjects[5], new Point(-1, 2)}
                }
            };

            var verifier = Verifier(dictionaries);

            var oldObjects = new List<ConfigurationObject>
            {
                configurationObjects[0],
                configurationObjects[1],
                configurationObjects[2],
                configurationObjects[3],
                configurationObjects[4]
            };

            var oldMap = new ConfigurationObjectsMap(oldObjects);

            var newObjects = new List<ConfigurationObject>
            {
                configurationObjects[5]
            };

            var newMap = new ConfigurationObjectsMap(newObjects);

            var correctOutputs = verifier.GetOutput(oldMap, newMap)
                    .Where(output => _containers.All(c => output.VerifierFunction(c)))
                    .ToList();

            Assert.AreEqual(8, correctOutputs.Count);
        }

        [Test]
        public void Test_With_Orthocenter()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Line) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Circle) {Id = 6}
            };

            var dictionaries = new[]
            {
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(5, 3)},
                    {configurationObjects[1], new Point(3, -3)},
                    {configurationObjects[2], new Point(8, -3)},
                    {configurationObjects[3], new Line(new Point(7, -1), new Point(3, -3))},
                    {configurationObjects[4], new Line(new Point(3.5, -1.5), new Point(8, -3))},
                    {configurationObjects[5], new Circle(new Point(7, -1), new Point(3.5, -1.5), new Point(5, 3))}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(7, 5)},
                    {configurationObjects[1], new Point(3, -3)},
                    {configurationObjects[2], new Point(11, -3)},
                    {configurationObjects[3], new Line(new Point(9.4, 0.2), new Point(3, -3))},
                    {configurationObjects[4], new Line(new Point(4.6, 0.2), new Point(11, -3))},
                    {configurationObjects[5], new Circle(new Point(7, 5), new Point(9.4, 0.2), new Point(4.6, 0.2))}
                }
            };

            var verifier = Verifier(dictionaries);

            var oldObjects = new List<ConfigurationObject>
            {
                configurationObjects[0],
                configurationObjects[1],
                configurationObjects[2],
                configurationObjects[3],
                configurationObjects[4]
            };

            var oldMap = new ConfigurationObjectsMap(oldObjects);

            var newObjects = new List<ConfigurationObject>
            {
                configurationObjects[5]
            };

            var newMap = new ConfigurationObjectsMap(newObjects);

            var correctOutputs = verifier.GetOutput(oldMap, newMap)
                    .Where(output => _containers.All(c => output.VerifierFunction(c)))
                    .ToList();

            Assert.AreEqual(3, correctOutputs.Count);
        }
    }
}