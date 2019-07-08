using FluentAssertions;
using GeoGen.Core;
using GeoGen.Constructor;
using NUnit.Framework;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.Analyzer.Tests
{
    /// <summary>
    /// The test class for <see cref="PotentialTheorem"/>.
    /// </summary>
    [TestFixture]
    public class PotentialTheoremTest
    {
        [Test]
        public void Test_In_Centroid_Situation_With_One_Possible_Definition_Per_Object()
        {
            // Initialize objects (it's good to draw it like I did)
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);
            var G = new ConstructedConfigurationObject(ComposedConstructions.IntersectionOfLinesFromPoints, B, F, C, D);

            // Assert
            new PotentialTheorem
            {
                InvolvedObjects = new[]
                {
                    new PointObject(A),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 1).Should().BeFalse("all the needed objects are: A");

            new PotentialTheorem
            {
                InvolvedObjects = new[]
                {
                    new PointObject(A),
                    new PointObject(B),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 2).Should().BeFalse("all the needed objects are: A, B");

            new PotentialTheorem
            {
                InvolvedObjects = new[]
                {
                    new PointObject(A),
                    new PointObject(D),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 4).Should().BeTrue("all the needed objects are: A, B, D");

            new PotentialTheorem
            {
                InvolvedObjects = new[]
                {
                    new PointObject(A),
                    new PointObject(G),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 7).Should().BeTrue("all the needed objects are: A, B, C, D, F, G");

            new PotentialTheorem
            {
                InvolvedObjects = new[]
                {
                    new PointObject(B),
                    new PointObject(D),
                    new PointObject(G),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 6).Should().BeFalse("all the needed objects are: A, B, C, D, F, G");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new PointObject(A),
                    new LineObject(new PointObject(B), new PointObject(D))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 3).Should().BeFalse("all the needed objects are: A, B, D");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new PointObject(D),
                    new LineObject(new PointObject(E), new PointObject(F))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 6).Should().BeFalse("all the needed objects are: A, B, C, D, E, F");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(E)),
                    new LineObject(new PointObject(B), new PointObject(G))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 7).Should().BeFalse("all the needed objects are: A, B, C, D, E, F, G");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C)),
                    new LineObject(new PointObject(E), new PointObject(G))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 8).Should().BeTrue("all the needed objects are: A, B, C, D, E, F, G");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new PointObject(A),
                    new LineObject(new PointObject(B), new PointObject(E)),
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C)),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 4).Should().BeFalse("all the needed objects are: A, B, C, E");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(E), new PointObject(F)),
                    new LineObject(new PointObject(B), new PointObject(E)),
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C)),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 5).Should().BeFalse("all the needed objects are: A, B, C, E, F");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(A), new PointObject(E)),
                    new LineObject(new PointObject(B), new PointObject(F)),
                    new LineObject(new PointObject(C), new PointObject(D)),
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 6).Should().BeFalse("all the needed objects are: A, B, C, D, E, F");
        }

        [Test]
        public void Test_In_Artificial_Situation_With_More_Definitions_Per_Object()
        {
            // Initialize objects (it's good to draw it like I did)
            var l = new LooseConfigurationObject(Line);
            var A = new LooseConfigurationObject(Point);
            var B = new ConstructedConfigurationObject(RandomPointOnLine, l);
            var C = new ConstructedConfigurationObject(RandomPointOnLine, l);
            var D = new ConstructedConfigurationObject(RandomPointOnLine, l);
            var E = new ConstructedConfigurationObject(SecondIntersectionOfCircleAndLineFromPoints, D, A, B, C);
            var F = new ConstructedConfigurationObject(RandomPointOnLineFromPoints, A, E);

            // Assert
            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(D), new PointObject(E), new PointObject(F)) { ConfigurationObject = l },
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 1).Should().BeFalse("the minimal definition needs l");

            // Assert
            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(D), new PointObject(E), new PointObject(F)) { ConfigurationObject = l },
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 2).Should().BeTrue("the minimal definition needs l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new PointObject(A)
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 2).Should().BeFalse("the minimal definition needs A, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new PointObject(A)
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 3).Should().BeTrue("the minimal definition needs A, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new PointObject(E)
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 6).Should().BeFalse("the minimal definition needs A, B, C, D, E, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new PointObject(E)
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 7).Should().BeTrue("the minimal definition needs A, B, C, D, E, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new LineObject(new PointObject(A), new PointObject(D), new PointObject(E), new PointObject(F))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 3).Should().BeFalse("the minimal definition needs A, D, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new LineObject(new PointObject(A), new PointObject(D), new PointObject(E), new PointObject(F))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 4).Should().BeTrue("the minimal definition needs A, D, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C), new PointObject(E))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 4).Should().BeFalse("the minimal definition needs A, B, C, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C), new PointObject(E))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 5).Should().BeTrue("the minimal definition needs A, B, C, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C), new PointObject(E)),
                    new LineObject(new PointObject(A), new PointObject(D), new PointObject(E), new PointObject(F))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 5).Should().BeFalse("the minimal definition needs A, B, C, D, l");

            new PotentialTheorem
            {
                InvolvedObjects = new GeometricalObject[]
                {
                    new LineObject(new PointObject(B), new PointObject(C), new PointObject(D)) { ConfigurationObject = l },
                    new CircleObject(new PointObject(A), new PointObject(B), new PointObject(C), new PointObject(E)),
                    new LineObject(new PointObject(A), new PointObject(D), new PointObject(E), new PointObject(F))
                }
            }.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: 6).Should().BeTrue("the minimal definition needs A, B, C, D, l");
        }
    }
}