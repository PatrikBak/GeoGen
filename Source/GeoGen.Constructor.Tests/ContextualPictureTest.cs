using FluentAssertions;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using static GeoGen.Core.ComposedConstructions;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructions;

namespace GeoGen.Constructor.Tests
{
    /// <summary>
    /// The test class for <see cref="ContextualPicture"/>.
    /// </summary>
    [TestFixture]
    public class ContextualPictureTest
    {
        #region Creating contextual picture

        /// <summary>
        /// Creates a contextual picture holding given configuration and its
        /// analytic representation in possibly more pictures.
        /// </summary>
        /// <param name="configuration">The configuration to be represented.</param>
        /// <param name="allObjects">The analytic representation of the configuration.</returns>
        /// <returns>The contextual picture.</returns>
        private static ContextualPicture CreatePicture(Configuration configuration, IAnalyticObject[][] allObjects)
        {
            // Create pictures
            var pictures = new Pictures(configuration, new PicturesSettings
            {
                MaximalAttemptsToReconstructAllPictures = 1,
                MaximalAttemptsToReconstructOnePicture = 1,
                NumberOfPictures = allObjects.Length
            });

            // Add all objects to them. For a current picture
            pictures.ForEach((picture, pictureIndex) =>
            {
                // Get the objects
                var objects = allObjects[pictureIndex].ToArray();

                // Add all of them to it
                for (var i = 0; i < objects.Length; i++)
                {
                    // Get the current object 
                    var analyticObject = objects[i];

                    // Add it to the picture
                    picture.TryAdd(configuration.AllObjects[i], () => analyticObject, out var _, out var _);
                }
            });

            // Return the result
            return new ContextualPicture(pictures);
        }

        #endregion

        #region Tests

        [Test]
        public void Test_With_Triangle()
        {
            // Create the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C);

            // Create the picture
            var picture = CreatePicture(configuration, new[]
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 6)
                },
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 5)
                }
            });

            #region Lines

            // Test all lines
            picture.AllLines.Should().HaveCount(3);

            // Test new lines
            picture.NewLines.Should().HaveCount(2);

            // Test old lines
            picture.OldLines.Should().HaveCount(1);

            // Test that the lines knows about its 2 points
            picture.AllLines.All(line => line.Points.Count == 2).Should().BeTrue();

            #endregion

            #region Circles

            // Test all circles
            picture.AllCircles.Should().HaveCount(1);

            // Test new circles
            picture.NewCircles.Should().HaveCount(1);

            // Test old circles
            picture.OldCircles.Should().HaveCount(0);

            // Test that the circle knows about its 3 points
            picture.AllCircles.All(circle => circle.Points.Count == 3).Should().BeTrue();

            #endregion

            #region Points

            // Test all points
            picture.AllPoints.Should().HaveCount(3);

            // Test new points
            picture.NewPoints.Should().HaveCount(1);

            // Test old points
            picture.OldPoints.Should().HaveCount(2);

            // Test that the points know about 1 circle and 2 lines passing through it
            picture.AllPoints.All(point => point.Lines.Count == 2 && point.Circles.Count == 1).Should().BeTrue();

            // Test that the points have their configuration object set
            picture.AllPoints.All(point => point.ConfigurationObject != null).Should().BeTrue();

            #endregion
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Centroid()
        {
            // Create the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var D = new ConstructedConfigurationObject(Midpoint, A, B);
            var E = new ConstructedConfigurationObject(Midpoint, B, C);
            var F = new ConstructedConfigurationObject(Midpoint, C, A);
            var G = new ConstructedConfigurationObject(Centroid, A, B, C);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, D, E, F, G);

            // Create the picture 
            var picture = CreatePicture(configuration, new[]
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(3, 5),
                    new Point(1.5, 3),
                    new Point(1.5, 2.5),
                    new Point(0, 0.5),
                    new Point(1, 2)
                },
                new IAnalyticObject[]
                {
                    new Point(0, 2),
                    new Point(2, 1),
                    new Point(3, 2),
                    new Point(2.5, 1.5),
                    new Point(1.5, 2),
                    new Point(1, 1.5),
                    new Point(5.0 / 3, 5.0 / 3)
                }
            });

            #region Lines

            // Test all lines
            picture.AllLines.Should().HaveCount(9);

            // Test new lines
            picture.NewLines.Should().HaveCount(0);

            // Test old lines
            picture.OldLines.Should().HaveCount(9);

            // Test that the number of lines with 3 points
            picture.AllLines.Count(line => line.Points.Count == 3).Should().Be(6);

            // Test that the number of lines with 2 points
            picture.AllLines.Count(line => line.Points.Count == 2).Should().Be(3);

            #endregion

            #region Circles

            // Test all circles
            picture.AllCircles.Should().HaveCount(29, "there are binomial[7,3] - 6 of them");

            // Test new circles
            picture.NewCircles.Should().HaveCount(12, "no four points are concyclic and the centroid adds binomial[6,2] - 3");

            // Test old circles
            picture.OldCircles.Should().HaveCount(17, "there are 29 (all) - 12 (new) of them");

            // Test that all the circles pass through 3 points
            picture.AllCircles.All(circle => circle.Points.Count == 3).Should().BeTrue();

            #endregion

            #region Points

            // Test all points
            picture.AllPoints.Should().HaveCount(7);

            // Test new points
            picture.NewPoints.Should().HaveCount(1);

            // Test old points
            picture.OldPoints.Should().HaveCount(6);

            // Test the number of points with 3 lines
            picture.AllPoints.Count(point => point.Lines.Count == 3).Should().Be(4);

            // Test the number of points with 4 lines
            picture.AllPoints.Count(point => point.Lines.Count == 4).Should().Be(3);

            // Test that 3 points (the midpoints) have binomial[6,2] - 2 circles
            picture.AllPoints.Count(point => point.Circles.Count == 13).Should().Be(3);

            // Test that the points have their configuration object set
            picture.AllPoints.All(point => point.ConfigurationObject != null).Should().BeTrue();

            #endregion
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Feets_Of_Altitudes_Which_Should_Be_Concyclic()
        {
            // Create the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, A, C);
            var F = new ConstructedConfigurationObject(Midpoint, A, B);
            var G = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, A, B, C);
            var H = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, B, C, A);
            var I = new ConstructedConfigurationObject(PerpendicularProjectionOnLineFromPoints, C, A, B);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C, D, E, F, G, H, I);

            // Create the picture
            var picture = CreatePicture(configuration, new[]
            {
                new IAnalyticObject[]
                {
                    new Point(0, 5),
                    new Point(-2, 1),
                    new Point(6, 1),
                    new Point(2, 1),
                    new Point(3, 3),
                    new Point(-1, 3),
                    new Point(0, 1),
                    new Point(6.0 / 13, 61.0 / 13),
                    new Point(-0.4, 4.2)
                },
                new IAnalyticObject[]
                {
                    new Point(0, 3),
                    new Point(-2, 1),
                    new Point(6, 1),
                    new Point(2, 1),
                    new Point(3, 2),
                    new Point(-1, 2),
                    new Point(0, 1),
                    new Point(-1.2, 3.4),
                    new Point(2, 5)
                }
             });

            // Test that there is one circle which passes through 6 points (Feurbach circle)
            picture.AllCircles.Count(circle => circle.Points.Count == 6).Should().Be(1);

            // Test the number of circles passing through 4 points
            picture.AllCircles.Count(circle => circle.Points.Count == 4).Should().Be(3, "two vertices and corresponding feet are concyclic");
        }

        [Test]
        public void Test_That_Collinear_Inconsistency_Causes_Inconsistent_Pictures_Exception()
        {
            // Create the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(ThreePoints, A, B, C);

            // Create the action that should cause an exception
            Action action = () => CreatePicture(configuration, new[]
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(0, 2)
                },
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(1, 0),
                    new Point(2, 1)
                }
             });

            // Assert
            action.Should().Throw<InconstructibleContextualPicture>("The points are collinear in one picture and not in the other.");
        }

        [Test]
        public void Test_That_Concyclic_Inconsistency_Causes_Inconsistent_Pictures_Exception()
        {
            // Create the objects
            var A = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var B = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var C = new LooseConfigurationObject(ConfigurationObjectType.Point);
            var D = new LooseConfigurationObject(ConfigurationObjectType.Point);

            // Create the configuration
            var configuration = Configuration.DeriveFromObjects(FourPoints, A, B, C, D);

            // Create the action that should cause an exception
            Action action = () => CreatePicture(configuration, new[]
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, 1)
                },
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(2, 1)
                }
             });

            // Assert
            action.Should().Throw<InconstructibleContextualPicture>("The points are collinear in one picture and not in the other.");
        }

        #endregion
    }
}