using GeoGen.Utilities;
using System.Linq;
using static GeoGen.AnalyticGeometry.AnalyticHelpers;
using static GeoGen.AnalyticGeometry.MathHelpers;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A static helper class containing helper methods for constructing random triangles, quadrilaterals, etc.
    /// </summary>
    public static class RandomLayoutsHelpers
    {
        /// <summary>
        /// Constructs a random scalene acute-angled triangle. 
        /// </summary>
        /// <returns>
        /// Three points A, B, C that make a scalene acute triangle. BC will always be (0,0)--(1,0). 
        /// A will have a positive y coordinate. It will hold that beta is the largest angle.
        /// </returns>
        public static (Point, Point, Point) ConstructRandomScaleneAcuteTriangle()
        {
            // First we normally place two points
            var B = new Point(0, 0);
            var C = new Point(1, 0);

            // In order to generate a third point A, we need to establish the rules. We want each two 
            // angles <A, <B, <C to have the absolute difference at least d, where d is a constant. Now 
            // we WLOG assume that <B is the largest angle and <C the smallest angle. We also want to have 
            // <B to be at least 'd' and 90 - <B to be at least 'd'. In that way we get a triangle that is 
            // acute, isn't too flat, isn't too close to a right-angled triangle and isn't close to an 
            // isosceles triangle. It can be shown that if we generate <B from the interval (60+d, 90-d)
            // and then <C from the interval ((180+d-B)/2, B-d), that we will get the wanted result (simple
            // math). In order to be able to generate <B, we need to have d from the interval (0,15). 
            // Generally the bigger, the better, but not very close to 15, because in that case  we might 
            // limit the variety of possible triangles (alpha would always be very close to 75). 
            const double d = 10;

            // Let us generate angles according to our formulas
            var beta = RandomnessHelper.NextDouble(60 + d, 90 - d);
            var gamma = RandomnessHelper.NextDouble((180 + d - beta) / 2, beta - d);

            // Construct the lines BA and CA
            var lineBA = new Line(B, C.Rotate(B, beta));
            var lineCA = new Line(C, B.Rotate(C, -gamma));

            // Intersection them to find the last point A
            var A = lineBA.IntersectionWith(lineCA).Value;

            // Return the points
            return (A, B, C);
        }

        /// <summary>
        /// Constructs a random triangle. 
        /// </summary>
        /// <returns>
        /// Three points A, B, C that make a triangle. BC will always be (0,0)--(1,0). 
        /// A will have a positive y coordinate. It will hold that beta is the largest angle.
        /// </returns>
        public static (Point, Point, Point) ConstructRandomTriangle()
        {
            // First we normally place two points
            var B = new Point(0, 0);
            var C = new Point(1, 0);

            // We will want to have each angle at least 'd' degrees (so it's not too flat)
            const double d = 10;

            // Clearly <B must be at most 180 - 2d, since it is equal to 180 - <A - <C
            var beta = RandomnessHelper.NextDouble(d, 180 - 2 * d);

            // Now when we have <B, we need to generate <C in [d, 180 - 2d) so that the last angle 
            // <A = 180 - <B - <C is at least d. Therefore <C should be then from the interval
            // [d, 180 - <B - d). That can be easily arranged
            var gamma = RandomnessHelper.NextDouble(d, 180 - beta - d);

            // Construct the lines BA and CA
            var lineBA = new Line(B, C.Rotate(B, beta));
            var lineCA = new Line(C, B.Rotate(C, -gamma));

            // Intersection them to find the last point A
            var A = lineBA.IntersectionWith(lineCA).Value;

            // Return the points
            return (A, B, C);
        }

        /// <summary>
        /// Constructs a random right triangle.
        /// </summary>
        /// <returns>
        /// Three points A, B, C making a right triangle. BC will always be (0,0)--(1,0) and
        /// the right angle will be at A.
        /// </returns>
        public static (Point, Point, Point) ConstructRandomRightTriangle()
        {
            // Fix the first two points
            var B = new Point(0, 0);
            var C = new Point(1, 0);

            // Let's make angle B between 8 and 37, so that it's not 
            // close to 45, nor close to 0, and the other angle is not 
            // close to 45 either, nor 90
            var beta = RandomnessHelper.NextDouble(8, 37);

            // The angle C can then be easily calculated
            var gamma = 90 - beta;

            // Construct the lines BA and CA
            var lineBA = new Line(B, C.Rotate(B, beta));
            var lineCA = new Line(C, B.Rotate(C, -gamma));

            // Intersection them to find the last point A
            var A = lineBA.IntersectionWith(lineCA).Value;

            // Return the points
            return (A, B, C);
        }

        /// <summary>
        /// Constructs a random convex quadrilateral that appear 'uniformly', which is achieved
        /// by making specific angles acute and ordered (see the documentation for returns).
        /// </summary>
        /// <returns>
        /// Four points A, B, C, D making a convex quadrilateral. BC will always will (0,0)--(1,0).
        /// A, D will have a positive y coordinate, where A will be closer to BC than D. 
        /// The angles at B and C will always be acute, and the angle at B will be smaller. 
        /// </returns>
        public static (Point, Point, Point, Point) ConstructRandomUniformConvexQuadrilateral()
        {
            // First we construct an acute scalene triangle PBC, where P will be the intersection
            // point of the lines BA and CD.
            var (P, B, C) = ConstructRandomScaleneAcuteTriangle();

            // On lines PB and PC we can find points A, D, respectively. A is generated slightly
            // below D, so that BC and AD are not parallel. From the construction also BC and AD
            // will not be parallel (since they intersect at P). 
            var A = B + RandomnessHelper.NextDouble(0.3, 0.5) * (P - B);
            var D = C + RandomnessHelper.NextDouble(0.6, 0.8) * (P - C);

            // Return the points
            return (A, B, C, D);
        }

        /// <summary>
        /// Constructs a random cyclic quadrilateral that appear 'uniformly', which is achieved
        /// by making specific angles acute and ordered (see the documentation for returns).
        /// </summary>
        /// <returns>
        /// Four points A, B, C, D making a cyclic quadrilateral. BC will always will (0,0)--(1,0).
        /// and the angles at A, B and C will be acute.
        /// </returns>
        public static (Point, Point, Point, Point) ConstructRandomUniformCyclicQuadrilateral()
        {
            // First we construct an acute scalene triangle ABC
            var (A, B, C) = ConstructRandomScaleneAcuteTriangle();

            // We want to find point D such that the angle CBD is smaller than CBA. 
            // Let's calculate the angle CBA first
            var CBA = AngleBetweenLines(new Line(B, A), new Line(B, C));

            // We will take the angle CBD as something around CBA/2. Let's say
            // something between 0.3 CBA and 0.7 CBA
            var CBD = RandomnessHelper.NextDouble(0.3, 0.7) * CBA;

            // Now we need to construct D. We can use rotation to get the line BD
            var lineBD = new Line(B, C.Rotate(B, ToDegrees(CBD)));

            // We intersect this line with the circumcenter ABC
            var D = new Circle(A, B, C).IntersectWith(lineBD)
                    // And take the intersection different from B
                    .Single(intersection => intersection != B);

            // Return the points
            return (A, B, C, D);
        }

        /// <summary>
        /// Constructs a random convex quadrilateral whose one side will be (0,0)--(1,0).
        /// </summary>
        /// <returns>
        /// Four points A, B, C, D making a convex quadrilateral. BC will always be (0,0)--(1,0) and
        /// A, D will have positive y-coordinates.
        /// </returns>
        public static (Point, Point, Point, Point) ConstructRandomConvexQuadrilateralWithHorizontalSide()
        {
            // First we construct a random scalene triangle PBC, where P will be the intersection
            // point of the lines BA and CD.
            var (P, B, C) = ConstructRandomScaleneAcuteTriangle();

            // On lines PB and PC we can find points A, D, respectively. We can make them not too
            // close to each other by making them a bit further from B, C, P
            var A = B + RandomnessHelper.NextDouble(0.2, 0.8) * (P - B);
            var D = C + RandomnessHelper.NextDouble(0.2, 0.8) * (P - C);

            // Return the points
            return (A, B, C, D);
        }

        /// <summary>
        /// Constructs a random convex quadrilateral whose one side will be (0,0)--(1,0).
        /// </summary>
        /// <returns>
        /// Four points A, B, C, D making a convex quadrilateral. BD will always be (0,0)--(1,0),
        /// A will have its y-coordinate positive, whereas C will have it negative.
        /// </returns>
        public static (Point, Point, Point, Point) ConstructRandomConvexQuadrilateralWithHorizontalDiagonal()
        {
            // Start with a convex quadrilateral with a horizontal side
            var (A, B, D, P) = ConstructRandomConvexQuadrilateralWithHorizontalSide();

            // Now we just flip point P in BD. Since BD is the x-axis, this means changing the
            // sign of the y coordinate
            var C = new Point(P.X, -P.Y);

            // Return the points
            return (A, B, C, D);
        }

        /// <summary>
        /// Constructs a line and a point that does not lie on it. 
        /// </summary>
        /// <returns>
        /// A line and a point. The line will always be the x-axis, and the point will have its x,y
        /// coordinates from the intervals [0,0.5], [0.5,1], respectively.
        /// </returns>
        public static (Line, Point) ConstructLineAndRandomPointNotLyingOnIt()
        {
            // Let the line by just the x-axis
            var line = new Line(new Point(0, 0), new Point(1, 0));

            // The point will have its coordinates from the intervals
            // [0,0.5], [0.5,1], so it's not too close to the line
            var point = new Point(RandomnessHelper.NextDouble(0, 0.5), RandomnessHelper.NextDouble(0.5, 1));

            // Return the line and point
            return (line, point);
        }

        /// <summary>
        /// Constructs a line and two random points that don't lie on it.
        /// </summary>
        /// <returns>
        /// A line l and two points. The line will always be the x-axis. The first point will have
        /// it's coordinates from the intervals [0,0.5], [0.5,1], respectively, and the second one 
        /// from the intervals [1,1.5], [1.5,2], respectively. This ensures the angle between the
        /// lines is between 45 and about 71.57 degrees (precisely arctan(3)).
        /// </returns>
        public static (Line, Point, Point) ConstructLineAndTwoRandomPointsNotLyingOnIt()
        {
            // Let the line by just the x-axis
            var line = new Line(new Point(0, 0), new Point(1, 0));

            // The first point will have its coordinates from the intervals
            // [0,0.5], [0.5,1], so it's not too close to the line
            var point1 = new Point(RandomnessHelper.NextDouble(0, 0.5), RandomnessHelper.NextDouble(0.5, 1));

            // The second point will have its coordinates from the intervals
            // [1,1.5], [1.5,2], so it's far from the line and from the first point too.
            // These numbers also make sure that the angle between the line and the line from 
            // our points is between 45 and about 71.57 degrees (precisely arctan(3)).
            var point2 = new Point(RandomnessHelper.NextDouble(1.5, 2), RandomnessHelper.NextDouble(1.5, 2));

            // Return the line and points
            return (line, point1, point2);
        }
    }
}
