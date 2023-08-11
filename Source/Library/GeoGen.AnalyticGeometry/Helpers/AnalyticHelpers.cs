using GeoGen.Utilities;
using static System.Math;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A static helper class for manipulation with analytic objects.
    /// </summary>
    public static class AnalyticHelpers
    {
        /// <summary>
        /// Finds all intersections of given analytic objects. They must not
        /// contain <see cref="Point"/>s and duplicate objects.
        /// </summary>
        /// <param name="objects">The objects to be intersected.</param>
        /// <returns>The intersections array.</returns>
        public static Point[] Intersect(params IAnalyticObject[] objects)
        {
            // Find which of the passed objects are lines and circles
            var lines = objects.OfType<Line>().ToArray();
            var circles = objects.OfType<Circle>().ToArray();

            // Prepare an array of possible intersections
            Point[] intersections;

            // Prepare an array or intersected objects
            IAnalyticObject[] intersected;

            // First check if we have two lines...
            if (lines.Length >= 2)
            {
                // If yes, then we intersect them
                var intersection = lines[0].IntersectionWith(lines[1]);

                // If there is no intersection, then we can immediately return an empty array
                if (intersection == null)
                    return Array.Empty<Point>();

                // Otherwise we set the intersections to this single one
                intersections = new[] { intersection.Value };

                // And set the intersected objects
                intersected = new IAnalyticObject[] { lines[0], lines[1] };
            }
            // If we don't have two lines, check if we have at best one line
            else if (lines.Length == 1)
            {
                // If yes, we'll intersect this line with a circle (there must be at least one)
                intersections = circles[0].IntersectWith(lines[0]);

                // And set the intersected objects
                intersected = new IAnalyticObject[] { lines[0], circles[0] };
            }
            // If we don't have any line...
            else
            {
                // We intersect the first two circles
                intersections = circles[0].IntersectWith(circles[1]);

                // And set the intersected objects
                intersected = new IAnalyticObject[] { circles[0], circles[1] };
            }

            // Now we find the remaining (non-intersected) objects
            var remaningObjects = objects.Where(analyticObject => !intersected.Contains(analyticObject)).ToArray();

            // And select those intersection points that lie on all the remaining objects
            return intersections.Where(point => remaningObjects.All(analyticObject => LiesOn(analyticObject, point))).ToArray();
        }

        /// <summary>
        /// Checks if a given point lies on a given analytic object. The object must not be a point.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <param name="point">The point.</param>
        /// <returns>true, if the point lies on the object; false otherwise.</returns>
        public static bool LiesOn(IAnalyticObject analyticObject, Point point) =>

            // Switch based on the analytic object
            analyticObject switch
            {
                // Line
                Line line => line.Contains(point),

                // Circle
                Circle circle => circle.Contains(point),

                // Unhandled cases
                _ => throw new AnalyticException($"Unhandled type of {nameof(IAnalyticObject)}: {analyticObject.GetType()}"),
            };


        /// <summary>
        /// Finds out if given points are collinear.
        /// </summary>
        /// <param name="points">The points to be checked.</param>
        /// <returns>true, if all the passed points are collinear; false otherwise.</returns>
        public static bool AreCollinear(params Point[] points)
        {
            // Take two points and construct their line
            var line = new Line(points[0], points[1]);

            // Return if all other points lie on this line
            return points.Skip(2).All(line.Contains);
        }

        /// <summary>
        /// Finds out if given distinct points are concyclic.
        /// </summary>
        /// <param name="points">The points to be checked.</param>
        /// <returns>true, if all the passed points are concyclic; false otherwise.</returns>
        public static bool AreConcyclic(params Point[] points)
        {
            // Take three and construct their circle
            var circle = new Circle(points[0], points[1], points[2]);

            // Return if all other points lie on this circle
            return points.Skip(3).All(circle.Contains);
        }

        /// <summary>
        /// Constructs the perpendicular bisector of the line defined by given distinct points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>The perpendicular bisector of the line [point1][point2].</returns>
        public static Line PerpendicularBisector(Point point1, Point point2)
        {
            // First we get the midpoint between these two points
            // This is a first needed point lying on the desired line
            var midpoint = (point1 + point2) / 2;

            // Rotate the first point around the midpoint by 90 degrees.
            // This is a second needed point lying on the desired line
            var rotatedPoint = point1.Rotate(midpoint, 90);

            // We can construct the line from these two points
            return new Line(midpoint, rotatedPoint);
        }

        /// <summary>
        /// Constructs the internal angle bisector of the angle [rayPoint1][vertex][rayPoint2]
        /// (or equivalently [rayPoint2][vertex][rayPoint1]). All three points must be distinct
        /// and not collinear. 
        /// </summary>
        /// <param name="vertex">The vertex of the angle.</param>
        /// <param name="rayPoint1">The first point on the ray starting from the vertex.</param>
        /// <param name="rayPoint2">The second point on the ray starting from the vertex.</param>
        /// <returns>The internal angle bisector of the angle [rayPoint1][vertex][rayPoint2].</returns>
        public static Line InternalAngleBisector(Point vertex, Point rayPoint1, Point rayPoint2)
        {
            // Denote A = [vertex], B = [rayPoint1], C = [rayPoint2]
            // We're going to construct line AX, where X is the intersection
            // of the internal angle bisector of BAC with BC. It's known that
            // BX / CX = AB / AC. One can use this to calculate the relation
            // X = B + |AB|/(|AB|+|AC|) . (C-B) (this is a vector relation)

            // First calculate the distances
            var distanceAB = vertex.DistanceTo(rayPoint1);
            var distanceAC = vertex.DistanceTo(rayPoint2);

            // Make sure neither of these distances is 0
            if (distanceAB.Rounded() == 0 || distanceAC.Rounded() == 0)
                throw new AnalyticException("Cannot construct the angle bisector from equal points.");

            // Construct point X according to the formula
            var X = rayPoint1 + distanceAB / (distanceAB + distanceAC) * (rayPoint2 - rayPoint1);

            // And finally line AX
            return new Line(vertex, X);
        }

        /// <summary>
        /// Calculates the angle between two lines.
        /// </summary>
        /// <param name="line1">The first line.</param>
        /// <param name="line2">The second line.</param>
        /// <returns>The angle between the lines, in radians. The value will in the interval [0, PI/2].</returns>
        public static double AngleBetweenLines(Line line1, Line line2)
            // We will use the well-known formula that the angle between two vectors
            // u,v is arccos(|u.v| / (||u|| ||v||)). In our case (A, B) is the normal
            // vector of our line, and in our case it's normalized, i.e. ||u|| = ||v|| = 1. 
            // In order to find the angle between two lines we can simply find the angle
            // between its normal vectors. This yields a very simple formula:
            => Acos(Abs(line1.A * line2.A + line1.B * line2.B));

        /// <summary>
        /// Constructs a point C such that the A, B, C are collinear in this order and the
        /// length of BC is equal to the passed one.
        /// </summary>
        /// <param name="A">The first point of the segment.</param>
        /// <param name="B">The second point of the segment.</param>
        /// <param name="shiftLength">The length of the shifted segment BC.</param>
        /// <returns>The shifted point B in the direction AB so that its distance from B is the shift length.</returns>
        public static Point ShiftSegment(Point A, Point B, double shiftLength)
            // Our result point C lies on the ray opposite to BA and satisfies |BC| = shiftLength. Let us 
            // express it as C = B + t(B-A), where t >= 0 is a real number. Then the distance between C and B
            // is equal to t |AB|. In order for it to be equal to 'shiftLength', we need t = shiftLength / |AB|.
            => B + shiftLength / A.DistanceTo(B) * (B - A);
    }
}