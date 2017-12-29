using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using Moq;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Theorems.TheoremVerifiers
{
    [TestFixture]
    public class CollinearityVerifierTest
    {
        private static IContextualContainer _container;

        private static CollinearityVerifier Verifier(params Dictionary<ConfigurationObject, IAnalyticalObject>[] objects)
        {
            var helper = new AnalyticalHelper();

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

            var container = new ContextualContainer(manager.Object, helper);

            if (!objects.Empty())
            {
                foreach (var configurationObject in objects[0].Keys)
                {
                    container.Add(configurationObject);
                }
            }

            _container = container;

            return new CollinearityVerifier();
        }

        [Test]
        public void Test_Theorem_Type_Is_Correct()
        {
            Assert.AreEqual(TheoremType.CollinearPoints, Verifier().TheoremType);
        }

        [Test]
        public void Test_Verifier_Input_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Verifier().GetOutput(null).ToList());
        }

        [Test]
        public void Test_Midpoints_And_Centroid()
        {
            var configurationObjects = new List<ConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 4},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 5},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 6},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 7}
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
                    {configurationObjects[5], new Point(0, 1.5)},
                    {configurationObjects[6], new Point(7.0/3, 2.0/3)}
                },
                new Dictionary<ConfigurationObject, IAnalyticalObject>
                {
                    {configurationObjects[0], new Point(0, 5)},
                    {configurationObjects[1], new Point(-2, -1)},
                    {configurationObjects[2], new Point(7, -1)},
                    {configurationObjects[3], new Point(2.5, -1)},
                    {configurationObjects[4], new Point(3.5, 2)},
                    {configurationObjects[5], new Point(-1, 2)},
                    {configurationObjects[6], new Point(5.0/3, 1)}
                }
            };

            var verifier = Verifier(dictionaries);

            var oldObjects = new List<ConfigurationObject>
            {
                configurationObjects[0],
                configurationObjects[1],
                configurationObjects[2],
                configurationObjects[3],
                configurationObjects[4],
                configurationObjects[5]
            };

            var oldMap = new ConfigurationObjectsMap(oldObjects);

            var newObjects = new List<ConfigurationObject>
            {
                configurationObjects[6]
            };

            var newMap = new ConfigurationObjectsMap(newObjects);

            var input = new VerifierInput(_container, oldMap, newMap);

            var output = verifier.GetOutput(input).ToList();

            Assert.AreEqual(3, output.Count);
        }

        [Test]
        public void Test_Euler_Line()
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
                    {configurationObjects[3], new Point(7.0/3, 2.0/3)},
                    {configurationObjects[4], new Point(1, 1.4)},
                    {configurationObjects[5], new Point(3, 0.3)}
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

            var input = new VerifierInput(_container, oldMap, newMap);

            var output = verifier.GetOutput(input).ToList();

            Assert.AreEqual(1, output.Count);
        }
    }
}