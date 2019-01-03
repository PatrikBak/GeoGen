using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometrical 2D line.
    /// </summary>
    public class Line : AnalyticObjectBase<Line>
    {
        #region Public properties

        /// <summary>
        /// Gets the A coefficient of the equation Ax + By + C = 0.
        /// </summary>
        public double A { get; }

        /// <summary>
        /// Gets the B coefficient of the equation Ax + By + C = 0.
        /// </summary>
        public double B { get; }

        /// <summary>
        /// Gets the C coefficient of the equation Ax + By + C = 0.
        /// </summary>
        public double C { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new line passing through given two points.
        /// These points can't be the same.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public Line(Point point1, Point point2)
        {
            // Check if points are not equal
            if (point1 == point2)
                throw new AnalyticException("Cannot construct a line from 2 equal points.");

            // Calculate the coefficients of the normal vector
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;

            // The pair (a,b) should be the normal vector, which is (dy, -dx)
            var a = dy;
            var b = -dx;

            // And the c coefficient is calculated so that the point1 lies on the line Ax + By + c = 0
            var c = -a * point1.X - b * point1.Y;

            // Now we calculate the scale so that (a,b) is a normalized vector (i.e. a^2+b^2 = 1)
            var scale = Math.Sqrt(a.Squared() + b.Squared());

            // Using the scale we almost have a unique representation. 
            // The problem is that ax+by+c = 0 and -ax-by-c=0 represent the same line
            // even if a^2+b^2 = 1. We're going to solve it by expecting the leftmost non-zero
            // coefficient to be positive. At least one of the numbers (a,b) is nonzero
            // The scale will be -1 if and only if a<0, or a=0 and b<0.
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
        /// lines can't be the same. If the lines are parallel, the null will be returned.
        /// </summary>
        /// <param name="otherLine">The other line.</param>
        /// <returns>The intersection, or null, if there isn't any.</returns>
        public Point IntersectionWith(Line otherLine)
        {
            // Check if they are equal
            if (this == otherLine)
                throw new AnalyticException("Equal lines cannot be interested.");

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

            // Pull the coefficients
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
        /// Finds a random point that lies on this line.
        /// </summary>
        /// <param name="provider">The randomness provider.</param>
        /// <returns>A random point.</returns>
        public Point RandomPointOnLine(IRandomnessProvider provider)
        {
            return null;
        }

        /// <summary>
        /// Creates a line that is perpendicular to this one and passes through a
        /// given point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The perpendicular line.</returns>
        public Line PerpendicularLine(Point point)
        {
            // Math suggests that the directional vector of the result
            // will be (A,B). We have one point on the line, [x,y]
            // the other could be [x+A, y+B] (they will not be the same 
            // because either A != 0, or B != 0). Then we can simply
            // use the constructor for line from 2 points

            // Create the other point
            var otherPoint = new Point(point.X + A, point.Y + B);

            // Create the line from 2 points
            return new Line(point, otherPoint);
        }

        /// <summary>
        /// Calculates the angle between this line and a given one. 
        /// The lines should be distinct.
        /// </summary>
        /// <param name="otherLine">The angle between lines.</param>
        /// <returns>The angle between the lines, in radians (0 for parallel lines). The value will in the interval [0, PI/2].</returns>
        public double AngleBetween(Line otherLine)
        {
            if (IsParallelTo(otherLine))
                return 0;

            if (IsPerpendicularTo(otherLine))
                return Math.PI / 2;

            return Math.Acos(Math.Abs(A * otherLine.A + B * otherLine.B));
        }

        /// <summary>
        /// Determines if a given point lies on this line.
        /// </summary>
        /// <param name="point">The given point.</param>
        /// <returns>true, if the point lies on the line, false otherwise.</returns>
        public bool Contains(Point point)
        {
            // We simply check if the point's coordinates meets the equation
            return (A * point.X + B * point.Y + C).Rounded() == 0;
        }

        /// <summary>
        /// Finds out if a given line is parallel to this one.
        /// </summary>
        /// <param name="otherLine">The line.</param>
        /// <returns>true, if the lines are parallel; false otherwise.s</returns>
        public bool IsParallelTo(Line otherLine)
        {
            // We check if their direction vectors are the same (they are normalized)
            return A.Rounded() == otherLine.A.Rounded() && B.Rounded() == otherLine.B.Rounded();
        }

        /// <summary>
        /// Finds out if a given line is perpendicular to this one.
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns></returns>
        public bool IsPerpendicularTo(Line otherLine)
        {
            // We check if their direction vectors are perpendicular same (they are normalized)
            return (A * otherLine.A + B * otherLine.B).Rounded() == 0;
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
            return new { a = A.Rounded(), b = B.Rounded(), c = C.Rounded() }.GetHashCode();
        }

        /// <summary>
        /// Returns if a given analytic object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected override bool IsEqualTo(Line other)
        {
            return A.Rounded() == other.A.Rounded() && B.Rounded() == other.B.Rounded() && C.Rounded() == other.C.Rounded();
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts a given line to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the line.</returns>
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
                // Here it's 2x or -2x, which is fine
                else
                    result += $"{A}x";
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

                    // Again, we don't want 1x...Not even -y, we already have the sign
                    if (B == 1 || B == -1)
                        result += "y";
                    else
                        result += $"{Math.Abs(B).ToString()}y";
                }
                // If there is not an x-part
                else
                {
                    // Then we simply add the y-part. It can't be zero as well
                    // but it can be 1 or -1 
                    if (B == 1)
                        result += "y";
                    else if (B == -1)
                        result += "-y";
                    else
                        result += $"{B}y";
                }
            }

            // Add the end, which is much less iffy
            result += $" = {(-C).ToString()}";

            // We're here!
            return result;
        }

        #endregion
    }
}