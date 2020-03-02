using GeoGen.Utilities;
using System;
using System.Globalization;
using static System.Math;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometric 2D line. This line is represented by its analytic equation
    /// Ax + By + C = 0. In order to get the unique equation for each line this class makes
    /// sure that A^2 + B^2 = 1, and A>0 || (A==0 && B>0) (in other words, the leftmost 
    /// non-zero coefficient in the sequence A,B is positive - they can't be both zero).
    /// </summary>
    public struct Line : IAnalyticObject, IEquatable<Line>
    {
        #region Public properties

        /// <summary>
        /// Gets the A coefficient of the equation Ax + By + C = 0 of the line.
        /// </summary>
        public double A { get; }

        /// <summary>
        /// Gets the B coefficient of the equation Ax + By + C = 0 of the line.
        /// </summary>
        public double B { get; }

        /// <summary>
        /// Gets the C coefficient of the equation Ax + By + C = 0 of the line.
        /// </summary>
        public double C { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> structure using two distinct points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public Line(Point point1, Point point2)
        {
            // Make sure the points are not equal
            if (point1 == point2)
                throw new AnalyticException("Cannot construct a line from 2 equal points.");

            // Calculate the coefficients of the directional vector
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;

            // Then the normal vector should be (dy, -dx)
            var a = dy;
            var b = -dx;

            // And the c coefficient is calculated so that point1 meets the equation Ax + By + c = 0
            var c = -a * point1.X - b * point1.Y;

            // Now we calculate the scale so that (a,b) is a normalized vector (i.e. a^2+b^2 = 1)
            var scale = Sqrt(a.Squared() + b.Squared());

            // If we used this scale, we would almost have the unique representation. The problem is that ax+by=0 
            // and -ax-by=0 represent the same line even when a^2+b^2 = (-a)^2+(-b)^2 = 1. We're going to solve it by
            // expecting the first non-zero coefficient between a,b to be positive. At least one of them is non-zero. 
            // We multiply the scale by -1 if and only if a<0, or a=0 and b<0.
            scale *= a.Rounded() < 0 || (a.Rounded() == 0 && b.Rounded() < 0) ? -1 : 1;

            // Finally we set the scaled coefficients
            A = a / scale;
            B = b / scale;
            C = c / scale;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs the intersection of this line with another given one. These
        /// lines can't equal. If the lines are parallel, then this method returns null.
        /// </summary>
        /// <param name="otherLine">The other line.</param>
        /// <returns>The intersection of the lines, if there is any; or null otherwise.</returns>
        public Point? IntersectionWith(Line otherLine)
        {
            // Make sure they are not equal
            if (this == otherLine)
                throw new AnalyticException("Equal lines cannot be intersected.");

            // We want to solve the system
            //
            // a1x + b1y + c1 = 0     (1)
            // a2x + b2y + c2 = 0     (2)
            //
            // Let's have a look at the equations
            // 
            // a2 (1) - a1 (2)
            // b1 (2) - b2 (1)
            // 
            // They give us
            //
            // y (a2b1 - a1b2) + c1a2 - c2a1 = 0
            // x (a2b1 - a1b2) + c2b1 - c1b2 = 0

            // Pull the coefficients so that the code looks better
            var a1 = A;
            var b1 = B;
            var c1 = C;

            var a2 = otherLine.A;
            var b2 = otherLine.B;
            var c2 = otherLine.C;

            // Calculate the common delta from the last 2 equations
            var delta = a2 * b1 - a1 * b2;

            // If it's 0, then the lines are either parallel, or equal.
            // But we know they're not equal.
            if (delta.Rounded() == 0)
                return null;

            // Otherwise we simply solve the simple linear equations
            var x = (c1 * b2 - c2 * b1) / delta;
            var y = (c2 * a1 - c1 * a2) / delta;

            // And construct the result
            return new Point(x, y);
        }

        /// <summary>
        /// Creates a line that is perpendicular to this one and passes through a given point.
        /// </summary>
        /// <param name="point">The point that should lie on the resulting line.</param>
        /// <returns>The line perpendicular to this one passing through the given point.</returns>
        public Line PerpendicularLineThroughPoint(Point point)
        {
            // Math suggests that the directional vector of the result
            // will be (A,B). We have one point on the line, [x,y]
            // the other could be [x+A, y+B] (they will not be the same 
            // because either A != 0, or B != 0). Then we can simply
            // use the constructor for the line from 2 points

            // Create the other point
            var otherPoint = new Point(point.X + A, point.Y + B);

            // Create the line from these 2 points
            return new Line(point, otherPoint);
        }

        /// <summary>
        /// Determines if a given point lies on this line.
        /// </summary>
        /// <param name="point">The given point.</param>
        /// <returns>true, if the point lies on the line; false otherwise.</returns>
        public bool Contains(Point point)
            // We check if the point's coordinates meet the equation of the line
            => (A * point.X + B * point.Y + C).Rounded() == 0;

        /// <summary>
        /// Finds out if a given line is parallel to this one.
        /// </summary>
        /// <param name="otherLine">The other line.</param>
        /// <returns>true, if the lines are parallel; false otherwise.</returns>
        public bool IsParallelTo(Line otherLine)
            // We check if their normal vectors are the same 
            // (they are normalized, so they uniformly define the direction)
            => A.Rounded() == otherLine.A.Rounded() && B.Rounded() == otherLine.B.Rounded();

        /// <summary>
        /// Finds out if a given line is perpendicular to this one.
        /// </summary>
        /// <param name="otherLine">The line to be checked.</param>
        /// <returns>true, if the lines are perpendicular; false otherwise.</returns>
        public bool IsPerpendicularTo(Line otherLine)
            // We check if their normal vectors (A,B), (otherA, otherB) are perpendicular using the scalar product
            => (A * otherLine.A + B * otherLine.B).Rounded() == 0;

        #endregion

        #region Overloaded operators

        /// <summary>
        /// Checks if two lines are equal.
        /// </summary>
        /// <param name="left">The left line.</param>
        /// <param name="right">The right line.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public static bool operator ==(Line left, Line right) => left.Equals(right);

        /// <summary>
        /// Checks if two lines are not equal.
        /// </summary>
        /// <param name="left">The left line.</param>
        /// <param name="right">The right line.</param>
        /// <returns>true, if they are not equal; false otherwise.</returns>
        public static bool operator !=(Line left, Line right) => !left.Equals(right);

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => (A.Rounded(), B.Rounded(), C.Rounded()).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Do the null and then the type check and then call the other Equals method
            => otherObject != null && otherObject is Line line && Equals(line);

        #endregion

        #region IEquatable implementation

        /// <inheritdoc/>
        public bool Equals(Line otherLine)
            // We can simply compare the equations of our lines because they are normalized
            => A.Rounded() == otherLine.A.Rounded() && B.Rounded() == otherLine.B.Rounded() && C.Rounded() == otherLine.C.Rounded();

        #endregion

        #region To String

        /// <inheritdoc/>
        public override string ToString()
        {
            // I know this is crazy to read, but the result is pretty
            // Prepare the result
            var result = "";

            // If there is an x-part...
            if (A != 0)
            {
                // We don't want to write 1x
                if (A == 1)
                    result += "x";
                // Or -1x
                else if (A == -1)
                    result += "-x";
                // Here it's something of type 2x or -2x, which is fine
                else
                    result += $"{A.ToString(CultureInfo.InvariantCulture)}x";
            }

            // If there is an y-part...
            if (B != 0)
            {
                // If there was an x-part...
                if (A != 0)
                {
                    // We want to include the sign with spaces
                    if (B < 0)
                        result += " - ";
                    else
                        result += " + ";

                    // Again, we don't want 1y...Not even -y, we already have the sign
                    if (B == 1 || B == -1)
                        result += "y";
                    else
                        result += $"{Abs(B).ToString(CultureInfo.InvariantCulture)}y";
                }
                // If there was not an x-part
                else
                {
                    // Then we simply add the y-part. It can't be zero as well
                    // but it can be 1 or -1 
                    if (B == 1)
                        result += "y";
                    else if (B == -1)
                        result += "-y";
                    else
                        result += $"{B.ToString(CultureInfo.InvariantCulture)}y";
                }
            }

            // Add the end, which is much less iffy (pun intended)
            result += $" = {(-C).ToString(CultureInfo.InvariantCulture)}";

            // We're here!
            return result;
        }

        #endregion
    }
}