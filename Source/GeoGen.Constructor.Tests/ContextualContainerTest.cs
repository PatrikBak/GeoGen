using FluentAssertions;
using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor.Tests
{
    /// <summary>
    /// The test class for <see cref="ContextualContainer"/>.
    /// </summary>
    [TestFixture]
    public class ContextualContainerTest
    {
        #region Creating container

        /// <summary>
        /// Creates a contextual containers holding given objects. These objects are given as 
        /// lists of arrays. Each array represents one analytic representation of a certain
        /// geometric situation.
        /// </summary>
        /// <param name="allObjects">The list of analytic representations of a geometric situation, each given as an array of analytic objects.</param>
        /// <returns>The contextual container holding given objects.</returns>
        private static ContextualContainer CreateContainer(List<IAnalyticObject[]> allObjects)
        {
            // First we create a configuration 
            // We can simply make all the objects loose, it doesn't matter
            // We're assuming every array has the same length representing the number of needed objects
            // We're also assuming that the objects in different pictures at the same position have the same type
            var looseObjects = Enumerable.Range(0, allObjects[0].Length)
                // For each index create an object
                .Select(i =>
                {
                    // Decide what to create according to the type of the analytic object
                    switch (allObjects[0][i])
                    {
                        case Point _:
                            return new LooseConfigurationObject(ConfigurationObjectType.Point);
                        case Line _:
                            return new LooseConfigurationObject(ConfigurationObjectType.Line);
                        case Circle _:
                            return new LooseConfigurationObject(ConfigurationObjectType.Circle);
                        default:
                            throw new GeoGenException($"Unknown type of analytic object");
                    }
                })
                // Enumerate to a list
                .ToList();


            // Create object containers holding the objects
            var containers = allObjects.Select(objects =>
            {
                // Create an empty container
                var container = new ObjectsContainer();

                // Add all the objects to it
                for (var i = 0; i < objects.Length; i++)
                {
                    // Pull the current one
                    var analyticObject = objects[i];

                    // Add a given one
                    container.TryAdd(looseObjects[i], () => analyticObject, out var constructed, out var equalObject);

                    // Constructed will be true. Equal object might not be
                    // Just in case...
                    if (equalObject != null)
                        throw new GeoGenException("Don't add two equal objects to a container :(");
                }

                // Return the final container
                return container;
            }).ToList();

            // Mock a manager that doesn't care about inconsistencies
            var manager = new Mock<IObjectsContainersManager>();
            manager.Setup(s => s.GetEnumerator()).Returns(() => containers.GetEnumerator());

            // Mock the configuration
            var configuration = new Mock<ContextualContainerSettings>();

            // And create the final result
            return new ContextualContainer(configuration.Object, looseObjects, manager.Object);
        }

        #endregion

        #region Tests

        [Test]
        public void Test_With_Triangle()
        {
            // Create container (it's good to draw it)
            var container = CreateContainer(new List<IAnalyticObject[]>
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
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).Should().HaveCount(3);

            // Test new lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeLines = true
            }).Should().HaveCount(2);

            // Test old lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeLines = true
            }).Should().HaveCount(1);

            // Test that the lines knows about its 2 points
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).All(line => line.Points.Count == 2).Should().BeTrue();

            #endregion

            #region Circles

            // Test all circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).Should().HaveCount(1);

            // Test new circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeCirces = true
            }).Should().HaveCount(1);

            // Test old circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeCirces = true
            }).Should().HaveCount(0);

            // Test that the circle knows about its 3 points
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).All(circle => circle.Points.Count == 3).Should().BeTrue();

            #endregion

            #region Points

            // Test all points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Should().HaveCount(3);

            // Test new points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludePoints = true
            }).Should().HaveCount(1);

            // Test old points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludePoints = true
            }).Should().HaveCount(2);

            // Test that the points know about 1 circle and 2 lines passing through it
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).All(point => point.Lines.Count == 2 && point.Circles.Count == 1).Should().BeTrue();

            // Test that the points have their configuration object set
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).All(point => point.ConfigurationObject != null).Should().BeTrue();

            #endregion
        }

        [Test]
        public void Test_With_Four_Points_And_Line_Through_Them_And_Circle_Passing_Through_One()
        {
            // Create container (it's good to draw it)
            var container = CreateContainer(new List<IAnalyticObject[]>
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(1, 1),
                    new Point(2, 2),
                    new Line(new Point(4, 4), new Point(5, 5)),
                    new Point(3, 3),
                    new Circle(new Point(-2, 1), Math.Sqrt(5))
                },
                new IAnalyticObject[]
                {
                    new Point(0, 1),
                    new Point(0, 2),
                    new Point(0, 3),
                    new Line(new Point(0, 5), new Point(0, 6)),
                    new Point(0, 4),
                    new Circle(new Point(-1, 0), Math.Sqrt(2))
                }
            });

            #region Lines

            // Test all lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).Should().HaveCount(1);

            // Test new lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeLines = true
            }).Should().HaveCount(0);

            // Test old lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeLines = true
            }).Should().HaveCount(1);

            // Test that the line knows about its 4 points
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).All(line => line.Points.Count == 4).Should().BeTrue();

            // Test that the line has the configuration object set
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).All(line => line.ConfigurationObject != null).Should().BeTrue();

            #endregion

            #region Circles

            // Test all circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).Should().HaveCount(1);

            // Test new circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeCirces = true
            }).Should().HaveCount(1);

            // Test old circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeCirces = true
            }).Should().HaveCount(0);

            // Test that the circle knows about its 1 point 
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).All(circle => circle.Points.Count == 1).Should().BeTrue();

            // Test that the circle has the configuration object set
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).All(circle => circle.ConfigurationObject != null).Should().BeTrue();

            #endregion

            #region Points

            // Test all points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Should().HaveCount(4);

            // Test new points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludePoints = true
            }).Should().HaveCount(0);

            // Test old points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludePoints = true
            }).Should().HaveCount(4);

            // Test there is a point that lines on 1 line and 1 circle
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Any(point => point.Circles.Count == 1 && point.Lines.Count == 1).Should().BeTrue();

            // Test the number of points with 0 circles and 1 lines
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Count(point => point.Circles.Count == 0 && point.Lines.Count == 1).Should().Be(3);

            // Test that the points have their configuration object set
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).All(point => point.ConfigurationObject != null).Should().BeTrue();

            #endregion
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Centroid()
        {
            // Create container (it's good to draw it)
            var container = CreateContainer(new List<IAnalyticObject[]>
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
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).Should().HaveCount(9);

            // Test new lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeLines = true
            }).Should().HaveCount(0);

            // Test old lines
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeLines = true
            }).Should().HaveCount(9);

            // Test that the number of lines with 3 points
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).Count(line => line.Points.Count == 3).Should().Be(6);

            // Test that the number of lines with 2 points
            container.GetGeometricalObjects<LineObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).Count(line => line.Points.Count == 2).Should().Be(3);

            #endregion

            #region Circles

            // Test all circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).Should().HaveCount(29, "there are binomial[7,3] - 6 of them");

            // Test new circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludeCirces = true
            }).Should().HaveCount(12, "no four points are concyclic and the centroid adds binomial[6,2] - 3");

            // Test old circles
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludeCirces = true
            }).Should().HaveCount(17, "there are 29 (all) - 12 (new) of them");

            // Test that all the circles pass through 3 points
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).All(circle => circle.Points.Count == 3).Should().BeTrue();

            #endregion

            #region Points

            // Test all points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Should().HaveCount(7);

            // Test new points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.New,
                IncludePoints = true
            }).Should().HaveCount(1);

            // Test old points
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.Old,
                IncludePoints = true
            }).Should().HaveCount(6);

            // Test the number of points with 3 lines
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Count(point => point.Lines.Count == 3).Should().Be(4);

            // Test the number of points with 4 lines
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Count(point => point.Lines.Count == 4).Should().Be(3);

            // Test that 3 points (the midpoints) have binomial[6,2] - 2 circles
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).Count(point => point.Circles.Count == 13).Should().Be(3);

            // Test that the points have their configuration object set
            container.GetGeometricalObjects<PointObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludePoints = true
            }).All(point => point.ConfigurationObject != null).Should().BeTrue();

            #endregion
        }

        [Test]
        public void Test_Triangle_With_Midpoints_And_Feets_Of_Altitudes_Which_Should_Be_Concyclic()
        {
            // Create container (it's good to draw it)
            var container = CreateContainer(new List<IAnalyticObject[]>
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
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true,
            }).Count(circle => circle.Points.Count == 6).Should().Be(1);

            // Test the number of circles passing through 4 points
            container.GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
            {
                Type = ContextualContainerQuery.ObjectsType.All,
                IncludeCirces = true,
            }).Count(circle => circle.Points.Count == 4).Should().Be(3, "two vertices and corresponding feet are concyclic");
        }

        [Test]
        public void Test_That_Collinear_Inconsistency_Causes_Inconsistent_Containers_Exception()
        {
            // Create action that should cause an exception
            Action act = () => CreateContainer(new List<IAnalyticObject[]>
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
            act.Should().Throw<InconsistentContainersException>("The points are collinear in one container and not in the other.");
        }

        [Test]
        public void Test_That_Concyclic_Inconsistency_Causes_Inconsistent_Containers_Exception()
        {
            // Create action that should cause an exception
            Action act = () => CreateContainer(new List<IAnalyticObject[]>
            {
                new IAnalyticObject[]
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, 1),
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
            act.Should().Throw<InconsistentContainersException>("The points are collinear in one container and not in the other.");
        }

        #endregion
    }
}