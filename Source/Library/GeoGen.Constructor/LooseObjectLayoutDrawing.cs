using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The static class that contains default drawing of <see cref="LooseObjectLayout"/> objects.
    /// </summary>
    public static class LooseObjectLayoutDrawing
    {
        /// <summary>
        /// Constructs analytic objects having a given layout. These objects are arranged randomly in a
        /// way that objects not too close or far from each other and the generated layouts are quite uniform,
        /// for example a random triangle is scalene and acute, a random quadrilateral has its angles ordered
        /// uniformly. This is good for finding theorems in multiple pictures, because it makes it very improbable
        /// that the limited imprecise floating point arithmetics will fail.
        /// </summary>
        /// <param name="layout">The layout of loose objects to be drawn.</param>
        /// <returns>The constructed analytic objects. Their count depends on the layout.</returns>
        public static IAnalyticObject[] ConstructUniformLayout(LooseObjectLayout layout)
        {
            // Switch based on the layout
            switch (layout)
            {
                // In line segment case everything is fixed
                case LooseObjectLayout.LineSegment:

                    // Return the points in an array
                    return new IAnalyticObject[] { new Point(0, 0), new Point(1, 0) };

                // With three points we'll create a random acute scalene triangle
                case LooseObjectLayout.Triangle:
                {
                    // Create the points
                    var (point1, point2, point3) = RandomLayoutsHelpers.ConstructRandomScaleneAcuteTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // In quadrilateral case we will create a random uniform convex one
                case LooseObjectLayout.Quadrilateral:
                {
                    // Create the points
                    var (point1, point2, point3, point4) = RandomLayoutsHelpers.ConstructRandomUniformConvexQuadrilateral();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3, point4 };
                }

                // In cyclic quadrilateral case we will create a random uniform one
                case LooseObjectLayout.CyclicQuadrilateral:
                {
                    // Create the points
                    var (point1, point2, point3, point4) = RandomLayoutsHelpers.ConstructRandomUniformCyclicQuadrilateral();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3, point4 };
                }

                // In line and point case the line is fixed and the point is arbitrary
                case LooseObjectLayout.LineAndPoint:
                {
                    // Create the objects
                    var (line, point) = RandomLayoutsHelpers.ConstructLineAndRandomPointNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { line, point };
                }

                // In line and two points case the line is fixed and the points are arbitrary
                case LooseObjectLayout.LineAndTwoPoints:
                {
                    // Create the objects
                    var (line, point1, point2) = RandomLayoutsHelpers.ConstructLineAndTwoRandomPointsNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { line, point1, point2 };
                }

                // In right triangle case the right angle will be at the first point
                case LooseObjectLayout.RightTriangle:
                {
                    // Create the points
                    var (point1, point2, point3) = RandomLayoutsHelpers.ConstructRandomRightTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled value of {nameof(LooseObjectLayout)}: {layout}.");
            }
        }
    }
}