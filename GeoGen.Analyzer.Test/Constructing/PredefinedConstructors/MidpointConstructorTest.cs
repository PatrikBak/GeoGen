using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Test.TestHelpers;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Constructing.PredefinedConstructors
{
    [TestFixture]
    public class MidpointConstructorTest
    {
        private static MidpointConstructor Constructor() => new MidpointConstructor();

        [Test]
        public void Test_Type_Is_Correct()
        {
            Assert.AreEqual(typeof(Midpoint), Constructor().PredefinedConstructionType);
        }

        [Test]
        public void Test_Constructed_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Constructor().Construct(null));
        }

        [Test]
        public void Test_Constructed_Objects_Must_Have_Count_One()
        {
            Assert.Throws<AnalyzerException>(() => Constructor().Construct(new List<ConstructedConfigurationObject>()));
            Assert.Throws<AnalyzerException>(() => Constructor().Construct(new List<ConstructedConfigurationObject> {null, null}));
        }

        [Test]
        public void Test_With_Incorrectly_Constructed_Objects()
        {
            var points = ConfigurationObjects.Objects(2, ConfigurationObjectType.Point);

            var arguments = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(points[0]),
                new ObjectConstructionArgument(points[1])
            };

            var constructedObject = new ConstructedConfigurationObject(new Midpoint(), arguments, 0);

            var listOfObjects = new List<ConstructedConfigurationObject> {constructedObject};

            Assert.Throws<AnalyzerException>(() => Constructor().Construct(listOfObjects));
        }

        [Test]
        public void Test_Construction_With_Correct_Input()
        {
            var points = ConfigurationObjects.Objects(22, ConfigurationObjectType.Point);

            var arguments = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(points[0]),
                        new ObjectConstructionArgument(points[1])
                    }
                )
            };

            var constructedObject = new ConstructedConfigurationObject(new Midpoint(), arguments, 0);

            var listOfObjects = new List<ConstructedConfigurationObject> {constructedObject};

            var result = Constructor().Construct(listOfObjects);

            var theorems = result.Theorems;

            Assert.AreEqual(1, theorems.Count);

            Assert.IsTrue(theorems[0].Type == TheoremType.CollinearPoints);

            Assert.IsTrue(theorems.ContainsTheoremWithObjects(points[0], points[1], constructedObject));

            var function = result.ConstructorFunction;

            Assert.NotNull(function);

            Assert.Throws<ArgumentNullException>(() => function(null));

            var representations = new List<List<Point>>
            {
                new List<Point> {new Point(0, 0), new Point(0, 1)},
                new List<Point> {new Point(0, 0), new Point(1, 1)}
            };

            var expected = new List<List<IAnalyticalObject>>
            {
                new List<IAnalyticalObject> {new Point(0, 0.5)},
                new List<IAnalyticalObject> {new Point(0.5, 0.5)}
            };

            bool Equals(List<IAnalyticalObject> l1, List<IAnalyticalObject> l2) => l1 == l2 || l1.SequenceEqual(l2);

            for (var i = 0; i < representations.Count; i++)
            {
                var container = new ObjectsContainer();

                for (var j = 0; j < 2; j++)
                {
                    container.Add(representations[i][j], points[j]);
                }

                var output = function(container);

                Assert.IsTrue(Equals(expected[i], output));
            }
        }
    }
}