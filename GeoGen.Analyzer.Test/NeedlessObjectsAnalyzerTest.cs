using System;
using System.Collections.Generic;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test
{
    [TestFixture]
    public class NeedlessObjectsAnalyzerTest
    {
        private NeedlessObjectsAnalyzer Analyzer() => new NeedlessObjectsAnalyzer(new Combinator(), new SubsetsProvider());

        private Construction Midpoint() => PredefinedConstructionsFactory.Get(PredefinedConstructionType.MidpointFromPoints);

        private Construction Intersection() => PredefinedConstructionsFactory.Get(PredefinedConstructionType.IntersectionOfLinesFromPoints);

        [Test]
        public void Test_With_Only_Loose_Objects()
        {
            // Configuration
            var configuration = new Configuration(new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2},
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3},
            }, new List<ConstructedConfigurationObject>());

            // Helper function to construct a point
            PointObject Point(int looseObjectIndex, int id) => new PointObject(id, configuration.LooseObjects[looseObjectIndex]);

            // Test cases (first value is input, second is expected result)
            var testCases = new List<Tuple<List<GeometricalObject>, bool>>
            {
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    Point(0, 1),
                    Point(1, 2),
                }, true),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    Point(0, 1),
                    Point(1, 2),
                    Point(2, 3),
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new LineObject(0, Point(0, 2), Point(1, 3)),
                    new LineObject(-1, Point(0, 2), Point(1, 3)),
                    new LineObject(-2, Point(1, 4), Point(2, 5))
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new LineObject(0, Point(0, 2), Point(1, 3)),
                    new LineObject(-1, Point(0, 2), Point(1, 3)),
                }, true),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new CircleObject(-1, Point(0, 1), Point(1, 2), Point(2, 3))
                }, false),
            };

            // Performing tests
            foreach (var testCase in testCases)
            {
                // Actual result
                var result = Analyzer().ContainsNeedlessObjects(configuration, testCase.Item1);

                // Comparing to expected result
                Assert.AreEqual(result, testCase.Item2);
            }
        }

        [Test]
        public void Test_With_Complex_Configuration()
        {
            // Loose points
            var loosePoints = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 1}, // A
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 2}, // B
                new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 3}, // C
            };

            // Midpoint AB - D
            var arguments1 = new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(loosePoints[0]), // A
                    new ObjectConstructionArgument(loosePoints[1]), // B
                })
            };
            var midpoint1 = new ConstructedConfigurationObject(Midpoint(), arguments1, 0) {Id = 4};

            // Midpoint AC - E
            var arguments2 = new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(loosePoints[0]), // A
                    new ObjectConstructionArgument(loosePoints[2]), // C
                })
            };
            var midpoint2 = new ConstructedConfigurationObject(Midpoint(), arguments2, 0) {Id = 5};

            // Centroid G
            var arguments3 = new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new SetConstructionArgument(new List<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(loosePoints[1]), // B
                        new ObjectConstructionArgument(midpoint2), // E
                    }),
                    new SetConstructionArgument(new List<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(loosePoints[2]), // C
                        new ObjectConstructionArgument(midpoint1), // D
                    }),
                })
            };
            var centroid = new ConstructedConfigurationObject(Intersection(), arguments3, 0) {Id = 6};

            // Configuration
            var configuration = new Configuration(loosePoints, new List<ConstructedConfigurationObject>
            {
                midpoint1,
                midpoint2,
                centroid
            });

            // Helper function to construct a point
            PointObject Point(int looseObjectIndex, int id) => new PointObject(id, configuration.LooseObjects[looseObjectIndex]);

            // Test cases (first value is input, second is expected result)
            var testCases = new List<Tuple<List<GeometricalObject>, bool>>
            {
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new PointObject(1, centroid)
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new PointObject(1, midpoint1),
                    new PointObject(2, midpoint2)
                }, true),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new LineObject(0, Point(0, 2), new PointObject(1, centroid)),
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new LineObject(0, new PointObject(1, midpoint1), new PointObject(2, midpoint2)),
                }, true),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new CircleObject(0, new PointObject(1, centroid), Point(0, 2), Point(1, 3))
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new CircleObject(0, new PointObject(1, centroid), Point(0, 2), Point(1, 3), Point(2, 4))
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new CircleObject(0, new PointObject(1, centroid), Point(0, 2), Point(1, 3), Point(2, 4)),
                    new LineObject(5, new PointObject(6, midpoint2), Point(0, 7), Point(1, 8), Point(2, 9)),
                    new LineObject(10, new PointObject(11, midpoint1), Point(0, 12), Point(1, 13), Point(2, 14)),
                }, false),
                new Tuple<List<GeometricalObject>, bool>(new List<GeometricalObject>
                {
                    new CircleObject(0, new PointObject(1, midpoint1), new PointObject(2, midpoint2), Point(1, 3), Point(2, 4)),
                    new LineObject(5, new PointObject(6, midpoint2), Point(0, 7), Point(1, 8), Point(2, 9)),
                    new LineObject(10, new PointObject(11, midpoint1), Point(0, 12), Point(1, 13), Point(2, 14)),
                    new LineObject(15, new PointObject(16, midpoint1), Point(0, 17), Point(1, 18), Point(2, 19)),
                    new PointObject(20, midpoint1),
                    new PointObject(21, midpoint2)
                }, true)
            };

            // Performing tests
            foreach (var testCase in testCases)
            {
                // Actual result
                var result = Analyzer().ContainsNeedlessObjects(configuration, testCase.Item1);

                // Comparing to expected result
                Assert.AreEqual(result, testCase.Item2);
            }
        }
    }
}