using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometrical 2D circle. This circle is defined by its center and radius.
    /// </summary>
    public class Circle : IAnalyticObject, IEquatable<Circle>
    {
        #region Public properties

        /// <summary>
        /// Gets the center of the circle.
        /// </summary>
        public Point Center { get; }

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        public double Radius { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> structure
        /// given by three non-collinear points that should lie on it.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        public Circle(Point point1, Point point2, Point point3)
        {
            // The center has coordinates [x, y] satisfying
            //
            // (x - x1)^2 + (y - y1)^2 = (x - x2)^2 + (y - y2)^2
            // (x - x1)^2 + (y - y1)^2 = (x - x3)^2 + (y - y3)^2
            //
            //
            // From this we can find:
            // 
            //      x1^2(y2 - y3) + x2^2(y3 - y1) + x3^2(y1 - y2) - (y1 - y2)(y2 - y3)(y3 - y1) 
            // x = -----------------------------------------------------------------------------
            //                        2(x1(y2 - y3) + x2(y3 - y1) + x3(y1 - y2))
            //
            //      y1^2(x3 - x2) + y2^2(x1 - x3) + y3^2(x2 - x1) + (x1 - x2)(x2 - x3)(x3 - x1)
            // y = ----------------------------------------------------------------------------------
            //                        2(x1(y2 - y3) + x2(y3 - y1) + x3(y1 - y2))
            // 
            // If you want to verify it, then please, take a photo of those calculations and mail them to me, thanks :)

            // Let's declare variables matching the ones in the equations
            var x1 = point1.X;
            var x2 = point2.X;
            var x3 = point3.X;
            var y1 = point1.Y;
            var y2 = point2.Y;
            var y3 = point3.Y;

            // Calculate the denominator
            var denominator = 2 * (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));

            // If it's zero, then we have collinear points
            if (denominator.Rounded() == 0)
                throw new AnalyticException("Cannot construct a circle from collinear points");

            // Now calculate x and y
            var x = (x1.Squared() * (y2 - y3) + x2.Squared() * (y3 - y1) + x3.Squared() * (y1 - y2) - (y1 - y2) * (y2 - y3) * (y3 - y1)) / denominator;
            var y = (y1.Squared() * (x3 - x2) + y2.Squared() * (x1 - x3) + y3.Squared() * (x2 - x1) + (x1 - x2) * (x2 - x3) * (x3 - x1)) / denominator;

            // Set the center
            Center = new Point(x, y);

            // Calculate the radius
            Radius = point1.DistanceTo(Center);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> structure given by its center and radius.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if a given point lies on this circle.
        /// </summary>
        /// <param name="point">The given point.</param>
        /// <returns>true, if the point lies on the circle; false otherwise.</returns>
        public bool Contains(Point point)
        {
            // We simply check whether the coordinates of the point meet the equation of this circle
            return ((point.X - Center.X).Squared() + (point.Y - Center.Y).Squared() - Radius.Squared()).Rounded() == 0;
        }

        /// <summary>
        /// Finds out if a given circle is tangent to this circle.
        /// </summary>
        /// <param name="otherCircle">The other circle.</param>
        /// <returns>true, if they are tangent to each other; false otherwise.</returns>
        public bool IsTangentTo(Circle otherCircle)
        {
            // Calculate the distance between their centers
            var distanceBetweenCenters = Center.DistanceTo(otherCircle.Center).Rounded();

            // If this distance is the same as the sum of their radii, then they're tangent externally
            if (distanceBetweenCenters == (Radius + otherCircle.Radius).Rounded())
                return true;

            // If this distance is the same as the difference between their radii, then they're tangent internally
            if (distanceBetweenCenters == Math.Abs(Radius - otherCircle.Radius).Rounded())
                return true;

            // Otherwise they're not tangent
            return false;
        }

        /// <summary>
        /// Finds out if a given line is tangent to this circle.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>true, if they are tangent to each other; false otherwise.</returns>
        public bool IsTangentTo(Line line)
        {
            // 
            return Math.Abs(line.A * Center.X + line.B * Center.Y + line.C).Rounded() == Radius.Rounded();
        }

        /// <summary>
        /// Finds all the intersections of this circle with a given other circle.
        /// The given circle can't be equal to this. 
        /// </summary>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        public Point[] IntersectWith(Circle otherCircle)
        {
            // Make sure it is not equal
            if (this == otherCircle)
                throw new AnalyticException("The circles are not supposed to be equal");

            // Find the distance between the centers.
            var dist = Center.DistanceTo(otherCircle.Center);

            // See how many solutions there are.
            if (dist.Rounded() > (Radius + otherCircle.Radius).Rounded())
            {
                return new Point[0];
            }
            else if (dist.Rounded() < Math.Abs(Radius - otherCircle.Radius).Rounded())
            {
                return new Point[0];
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
                    return new [] { p1 };
                else
                    return new [] { p1, p2 };
            }
        }

        /// <summary>
        /// Finds all the intersections of this circle with a given line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The list of intersections. An empty list, if there isn't any.</returns>
        public Point[] IntersectWith(Line line)
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
        private Point[] IntersectWithLine(double a, double b, double c)
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

            var aCoef = (b/a).Squared() + 1;
            var bCoef = 2 * b * c / a.Squared() + 2 * b * m / a - 2 * n;
            var cCoef = (c / a + m).Squared() + n.Squared() - r.Squared();

            // Let the helper method solve the quadratic equation for y.
            var yRoots = MathematicalHelpers.SolveQuadraticEquation(aCoef, bCoef, cCoef);

            // If there are no solutions, we won't have any intersection
            if (yRoots.IsEmpty())
                return new Point[0];

            // Otherwise we compute the corresponding x-roots using x = (-c - by) / a
            var xRoots = yRoots.Select(y => (-c - b * y) / a).ToList();

            // Now we can construct the result. We can't forget that we might
            // have changed the meaning of x and y
            return Enumerable.Range(0, xRoots.Count).Select(i =>
            {
                var xRoot = xRoots[i];
                var yRoot = yRoots[i];

                return changingVariables ? new Point(yRoot, xRoot) : new Point(xRoot, yRoot);
            }).ToArray();
        }

        #endregion

        #region Overloaded operators

        /// <summary>
        /// Checks if two circles are equal.
        /// </summary>
        /// <param name="left">The left circle.</param>
        /// <param name="right">The right circle.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public static bool operator ==(Circle left, Circle right) => left.Equals(right);

        /// <summary>
        /// Checks if two circles are not equal.
        /// </summary>
        /// <param name="left">The left circle.</param>
        /// <param name="right">The right circle.</param>
        /// <returns>true, if they are not equal; false otherwise.</returns>
        public static bool operator !=(Circle left, Circle right) => !left.Equals(right);

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Do the null and then the type check and then call the other Equals method
            return otherObject != null && otherObject is Circle circle && Equals(circle);
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // Let's use the built-in .NET hash code calculator for anonymous types
            return new { r = Radius.Rounded(), o = Center }.GetHashCode();
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Finds out if the passed circle is equal to this one.
        /// </summary>
        /// <param name="otherCircle">The other circle.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(Circle otherCircle)
        {
            // Check if the centers and radii are equal
            return Center == otherCircle.Center && Radius.Rounded() == otherCircle.Radius.Rounded();
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