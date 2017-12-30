using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Utilities;
using GeoGen.Utilities.Helpers;

namespace GeoGen.AnalyticalGeometry.AnalyticalObjects
{
    /// <summary>
    /// Represents a geometrical 2D circle.
    /// </summary>
    public class Circle : IAnalyticalObject
    {
        private Lazy<int> _hashCOde;

        #region Public properties

        /// <summary>
        /// Gets the centers of the circle.
        /// </summary>
        public Point Center { get; }

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        public RoundedDouble Radius { get; }

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
            // Check if we have 3 mutually distinct points
            if (point1 == point2 || point2 == point3 || point3 == point1)
                throw new ArgumentException("Equal points");

            // We create the perpendicular bisectors of lines P1P2, P1P3
            var bisector1 = point1.PerpendicularBisector(point2);
            var bisector2 = point1.PerpendicularBisector(point3);

            // They should intersect in the center
            var center = bisector1.IntersectionWith(bisector2);

            // If the center is null, we have collinear points.
            Center = center ?? throw new ArgumentException("Collinear points");

            // Otherwise the situation is fine and radius is the distance
            // from any point to the center.
            Radius = point1.DistanceTo(Center);

            var radiusCopy = Radius;
            var centerCopy = Center;

            _hashCOde = new Lazy<int>(() => (centerCopy.GetHashCode() * 397) ^ radiusCopy.GetHashCode());
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

            if (Radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius), "The radius must be positive.");

            var radiusCopy = Radius;
            var centerCopy = Center;

            _hashCOde = new Lazy<int>(() => (centerCopy.GetHashCode() * 397) ^ radiusCopy.GetHashCode());
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

            return (RoundedDouble) (dx * dx + dy * dy) == Radius * Radius;
        }

        /// <summary>
        /// Finds all the intersections of this circle with a given other circle.
        /// The given circle can't be equal to this. 
        /// </summary>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        public List<Point> IntersectWith(Circle otherCircle)
        {
            // First we check if the circles aren't the equal
            if (this == otherCircle)
                throw new ArgumentException("Equal circles");

            // If they're distinct and concentric, then there's no intersection
            if (Center == otherCircle.Center)
                return new List<Point>();

            // Otherwise we're solving the system:
            // 
            // (x-m1)^2 + (y-n1)^2 = r1^2       (1)
            // (x-m2)^2 + (y-n2)^2 = r2^2       (2)
            //
            // The idea is to solve the equivalent system 
            // (1), (1) - (2) instead. Since (1) - (2)
            // is the equation of line (as it turns out,
            // the radical axis of these two circles),
            // we can use the coded method. The coefficients
            // of the line are
            //
            // x --> 2(m2 - m1)
            // y --> 2(n2 - n1)
            // 1 --> m1^2 - m2^2 + n1^2 - n2^2 + r2^2 - r1^2

            // Pull the parameters of this circle
            var m1 = Center.X;
            var n1 = Center.Y;
            var r1 = Radius;

            // Pull the parameters of the other circle
            var m2 = otherCircle.Center.X;
            var n2 = otherCircle.Center.Y;
            var r2 = otherCircle.Radius;

            // Create coefficients
            var aCoef = 2 * (m2 - m1);
            var bCoef = 2 * (n2 - n1);
            var cCoef = m1 * m1 - m2 * m2 + n1 * n1 - n2 * n2 + r2 * r2 - r1 * r1;

            // Call the internal method
            return IntersectWithLine(aCoef, bCoef, cCoef);
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

        #region Overloaded operators

        /// <summary>
        /// Determines if two given circles are equal.
        /// </summary>
        /// <param name="left">The left circle.</param>
        /// <param name="right">The right circle.</param>
        /// <returns>true, if they are equal, false otherwise.</returns>
        public static bool operator ==(Circle left, Circle right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines if two given circles are not equal.
        /// </summary>
        /// <param name="left">The left circle.</param>
        /// <param name="right">The right circle.</param>
        /// <returns>true, if they are not equal, false otherwise.</returns>
        public static bool operator !=(Circle left, Circle right)
        {
            return !Equals(left, right);
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
            var changingVariables = (RoundedDouble) a == 0;

            // If yes, we use the helper method to do so
            if (changingVariables)
            {
                Utils.Swap(ref a, ref b);
                Utils.Swap(ref m, ref n);
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
            var yRoots = MathUtils.SolveQuadraticEquation(aCoef, bCoef, cCoef);

            // If there are no solutions, we won't have any intersection
            if (yRoots.Empty())
                return new List<Point>();

            // Otherwise we compute the corresponding x-roots using x = (-c - by) / a
            var xRoots = yRoots.Select(y => (-c - b * y) / a).ToList();

            // Now we can construct the result. We can't forget that we might
            // have changed the meaning of x and y
            return Enumerable.Range(0, xRoots.Count).Select
                    (
                        i =>
                        {
                            var xRoot = xRoots[i];
                            var yRoot = yRoots[i];

                            return changingVariables ? new Point(yRoot, xRoot) : new Point(xRoot, yRoot);
                        }
                    )
                    .ToList();
        }

        /// <summary>
        /// Determines if this circle is equal to a given other one.
        /// </summary>
        /// <param name="other">The other line.</param>
        /// <returns>true, if they are equal, false otherwise.</returns>
        private bool Equals(Circle other)
        {
            return Center == other.Center && Radius == other.Radius;
        }

        #endregion

        #region Equals and HashCode

        /// <summary>
        /// Determines if a given object is equal to this circle.
        /// </summary>
        /// <param name="obj">A given object.</param>
        /// <returns>true, if they are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (!(obj is Circle circle))
                return false;

            return Equals(circle);
        }

        /// <summary>
        /// Gets the hash code of the circle.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _hashCOde.Value;
            unchecked
            {
                
                //return (Center.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }

        #endregion
    }
}