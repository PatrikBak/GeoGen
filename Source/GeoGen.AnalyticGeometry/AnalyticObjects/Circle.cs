using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometrical 2D circle.
    /// </summary>
    public class Circle : AnalyticObjectBase<Circle>
    {
        #region Public properties

        /// <summary>
        /// Gets the centers of the circle.
        /// </summary>
        public Point Center { get; }

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        public double Radius { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new circle given by 3 given points. These points
        /// must be mutually distinct and can't be collinear.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        public Circle(Point point1, Point point2, Point point3)
        {
           // The center has coordinates[x, y] satisfying
           //
           // (x - x1) ^ 2 + (y - y1) ^ 2 = (x - x2) ^ 2 + (y - y2) ^ 2
           // (x - x1) ^ 2 + (y - y1) ^ 2 = (x - x3) ^ 2 + (y - y3) ^ 2
           //
           //
           //  They can be easily reduced to


             //x cit(x1^ 2(y2 - y3) + x2 ^ 2(y3 - y1) + x3 ^ 2(y1 - y2) - (y1 - y2)(y2 - y3)(y3 - y1))
              // 2(x1(y2 - y3) + x2(y3 - y1) + x3(y1 - y2))

//y cit(x1(y2^ 2 - y3 ^ 2) +x2(y3 ^ 2 - y1 ^ 2) + x3(y1 ^ 2 - y2 ^ 2) + (x1 - x2)(x2 - x3)(x3 - x1))
              // 2(x1(y2 - y3) + x2(y3 - y1) + x3(y1 - y2))


            // We create the perpendicular bisectors of lines P1P2, P1P3
            var bisector1 = point1.PerpendicularBisector(point2);
            var bisector2 = point1.PerpendicularBisector(point3);

            // They should intersect in the center
            var center = bisector1.IntersectionWith(bisector2);

            // If the center is null, we have collinear points.
            Center = center ?? throw new AnalyticException("Collinear points");

            // Otherwise the situation is fine and radius is the distance
            // from any point to the center.
            Radius = point1.DistanceTo(Center);
        }

        /// <summary>
        /// Constructs a new circle given by a center and a radius.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if a given point lies on this circle.
        /// </summary>
        /// <param name="point">The given point.</param>
        /// <returns>true, if the point lies on the circle, false otherwise.</returns>
        public bool Contains(Point point)
        {
            // We simply check whether the coordinates of the point meet the equation
            var dx = point.X - Center.X;
            var dy = point.Y - Center.Y;

            return (dx * dx + dy * dy - Radius * Radius).Rounded() == 0;
        }

        /// <summary>
        /// Finds out if a given circle is tangent to this circle.
        /// </summary>
        /// <param name="line">The other circle.</param>
        /// <returns>true, if they are tangent to each other; false otherwise.</returns>
        public bool IsTangentTo(Circle otherCircle)
        {
            var dist = Center.DistanceTo(otherCircle.Center).Rounded();

            if (dist == (Radius + otherCircle.Radius).Rounded())
                return true;

            if (dist  == Math.Abs(Radius - otherCircle.Radius).Rounded())
                return true;

            return false;
        }

        /// <summary>
        /// Finds out if a given line is tangent to this circle.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>true, if they are tangent to each other; false otherwise.</returns>
        public bool IsTangentTo(Line line)
        {
            return Math.Abs(line.A * Center.X + line.B * Center.Y + line.C).Rounded() == Radius.Rounded();
        }

        /// <summary>
        /// Finds all the intersections of this circle with a given other circle.
        /// The given circle can't be equal to this. 
        /// </summary>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        public List<Point> IntersectWith(Circle otherCircle)
        {
            // Make sure it is not equal
            if (this == otherCircle)
                throw new AnalyticException("The circles are not supposed to be equal");

            // Find the distance between the centers.
            var dist = Center.DistanceTo(otherCircle.Center);

            // See how many solutions there are.
            if (dist.Rounded() > (Radius + otherCircle.Radius).Rounded())
            {
                return new List<Point>();
            }
            else if (dist.Rounded() < Math.Abs(Radius - otherCircle.Radius).Rounded())
            {
                return new List<Point>();
            }
            else
            {
                // Find a and h.
                double a = (Radius * Radius - otherCircle.Radius * otherCircle.Radius + dist * dist) / (2 * dist);
                double h = Math.Sqrt(Radius * Radius - a * a);

                // Find P2.
                double cx2 = Center.X + a * (otherCircle.Center.X - Center.X) / dist;
                double cy2 = Center.Y + a * (otherCircle.Center.Y - Center.Y) / dist;

                var p1 = new Point(cx2 + h * (otherCircle.Center.Y - Center.Y) / dist, cy2 - h * (otherCircle.Center.X - Center.X) / dist);
                var p2 = new Point(cx2 - h * (otherCircle.Center.Y - Center.Y) / dist, cy2 + h * (otherCircle.Center.X - Center.X) / dist);

                if (p1 == p2)
                    return new List<Point> { p1 };
                else
                    return new List<Point> { p1, p2 };
            }
        }

        /// <summary>
        /// Finds all the intersections of this circle with a given line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        public List<Point> IntersectWith(Line line)
        {
            // Pull the coefficients of the equation of the line
            var a = line.A;
            var b = line.B;
            var c = line.C;

            // Call the internal method
            return IntersectWithLine(a, b, c);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds all the intersections of this circle with a line given by its
        /// coefficients of the equation ax + by + c = 0. It is assumed that 
        /// at least one of the coefficients a, b is not zero. 
        /// </summary>
        /// <param name="a">The a coefficient.</param>
        /// <param name="b">The b coefficient.</param>
        /// <param name="c">The c coefficient.</param>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        private List<Point> IntersectWithLine(double a, double b, double c)
        {
            // Pull the parameters of the equation of the circle
            var m = Center.X;
            var n = Center.Y;
            var r = Radius;

            // We're solving the system:
            //
            // ax + by + c = 0               (1)
            // (x-m)^2 + (y-n)^2 = r^2       (2)
            //
            // At least one of a,b is not zero.
            // If it's a, we'll express a by terms of b,c. 
            // If it's b, then mapping (a,b,m,n) --> (b,a,n,m) we'll give
            // us the second case. The new equation would have the solution [x',y']
            // if and only if the previous one had the solution [y',x']

            // First we determine if we'll do the mapping. 
            var changingVariables = a.Rounded() == 0;

            // If yes, we use the helper method to do so
            if (changingVariables)
            {
                GeneralUtilities.Swap(ref a, ref b);
                GeneralUtilities.Swap(ref m, ref n);
            }

            // Now we're sure that a != 0. (1) gives x = (-c - by) / a;
            // The (2) becomes ((-c - by) / a - m)^2 + (y - n)^2 = r^2
            // We have a quadratic equation The coefficients are:
            // 
            // x^2 --> (b/a)^2 + 1
            // x   --> 2bc/a^2 + 2bm/a - 2n
            // 1   --> (c/a + m)^2 + n^2 - r^2

            var aCoef = b * b / (a * a) + 1;
            var bCoef = 2 * b * c / (a * a) + 2 * b * m / a - 2 * n;
            var cCoef = (c / a + m) * (c / a + m) + n * n - r * r;

            // Let the helper method solve the quadratic equation for y.
            var yRoots = MathematicalHelpers.SolveQuadraticEquation(aCoef, bCoef, cCoef);

            // If there are no solutions, we won't have any intersection
            if (yRoots.IsEmpty())
                return new List<Point>();

            // Otherwise we compute the corresponding x-roots using x = (-c - by) / a
            var xRoots = yRoots.Select(y => (-c - b * y) / a).ToList();

            // Now we can construct the result. We can't forget that we might
            // have changed the meaning of x and y
            return Enumerable.Range(0, xRoots.Count).Select(i =>
            {
                var xRoot = xRoots[i];
                var yRoot = yRoots[i];

                return changingVariables ? new Point(yRoot, xRoot) : new Point(xRoot, yRoot);
            }).ToList();
        }

        #endregion

        #region Abstract HashCode and Equals implementation

        /// <summary>
        /// Calculates the hash code of the object. This method is called once per
        /// object, unlike GetHashCode, which will reuse the result of this method.
        /// </summary>
        /// <returns>The hash code.</returns>
        protected override int CalculateHashCode()
        {
            return new { r = Radius.Rounded(), o = Center }.GetHashCode();
        }

        /// <summary>
        /// Returns if a given analytic object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected override bool IsEqualTo(Circle other)
        {
            return Center == other.Center && Radius.Rounded() == other.Radius.Rounded();
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts a given circle to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the circle.</returns>
        public override string ToString()
        {
            // Prepare the result
            var result = "";

            // Add the x-part, i.e. (x-Center.X)^2.
            if (Center.X == 0)
                result += "x^2";
            else if (Center.X < 0)
                result += $"(x + {(-Center.X).ToString()})^2";
            else
                result += $"(x - {Center.X})^2";

            // The middle plus
            result += " + ";

            // Add the y-part, i.e. (y-Center.Y)^2.
            if (Center.Y == 0)
                result += "y^2";
            else if (Center.Y < 0)
                result += $"(y + {(-Center.Y).ToString()})^2";
            else
                result += $"(y - {Center.Y})^2";

            // And the end
            result += $" = {(Radius * Radius).ToString()}";

            // Return it
            return result;
        }

        #endregion
    }
}