using System;
using GeoGen.Utilities;

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
        public RoundedDouble A { get; }

        /// <summary>
        /// Gets the B coefficient of the equation Ax + By + C = 0.
        /// </summary>
        public RoundedDouble B { get; }

        /// <summary>
        /// Gets the C coefficient of the equation Ax + By + C = 0.
        /// </summary>
        public RoundedDouble C { get; }

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
            if(point1 == point2)
                throw new AnalyticException("An attempt to construct a line from two equal points.");

            // Calculate the coefficients of the direction vector
            var dx = point1.X.OriginalValue - point2.X.OriginalValue;
            var dy = point1.Y.OriginalValue - point2.Y.OriginalValue;

            // The pair (a,b) should be the normal vector, which is (dy, -dx)
            var a = dy;
            var b = -dx;

            // And the c coefficient is calculated so that the point1 lies on the line
            var c = -a * point1.X - b * point1.Y;

            // For any line they're infinitely many equations of the form Ax + By + C = 0.
            // In order for us to have the unique representation for each one, we would
            // want to have A^2 + B^2 + C^2 = 1 and A > 0 if A != 0 or B > 0 otherwise.
            // Then the representation will be unique

            // Round a
            var roundedA = (RoundedDouble) a;

            // If a is not zero, we want it to be positive
            if (roundedA != RoundedDouble.Zero)
            {
                // If it's not positive
                if (roundedA < RoundedDouble.Zero)
                {
                    // We multiply the whole equation by -1
                    a = -a;
                    b = -b;
                    c = -c;
                }
            }
            // Otherwise if a is 0, we want b to be positive
            else
            {
                // If b is negative
                if ((RoundedDouble)b < RoundedDouble.Zero)
                {
                    // We multiply the whole equation by -1
                    b = -b;
                    c = -c;
                }
            }

            // Now we can finally scale the coefficients so that A^2 + B^2 + C^2 = 1 holds true
            var scale = Math.Sqrt(a * a + b * b + c * c);

            // And set the coefficients
            A = (RoundedDouble) (a / scale);
            B = (RoundedDouble) (b / scale);
            C = (RoundedDouble) (c / scale);
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
            var a1 = A.OriginalValue;
            var b1 = B.OriginalValue;
            var c1 = C.OriginalValue;

            var a2 = otherLine.A.OriginalValue;
            var b2 = otherLine.B.OriginalValue;
            var c2 = otherLine.C.OriginalValue;

            // Calculate the common delta from the last 2 equations
            var delta = a2 * b1 - a1 * b2;

            // If it's 0, then the lines are either parallel, or equal.
            // But we know they're not equal.
            if ((RoundedDouble)delta == RoundedDouble.Zero)
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
        /// Finds a point that lines on this line. This point might not be
        /// random, i.e. it might be deterministic for the same line.
        /// </summary>
        /// <returns>The point.</returns>
        public Point PointOnLine()
        {
            // Either A or B is not zero. We need to meet the equation Ax + By + C =0.
            // So we try to take x=0, if B is not 0; or y=0 otherwise.
            return B != RoundedDouble.Zero
                    // If B is not zero, return this point
                    ? new Point(B, (-C + A * B) / B)
                    // Otherwise A is not zero and return
                    : new Point(A, -(C + A * B) / A);
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
            // Local function to find out the oriented angle between x-axis and a line
            double Angle(Line line)
            {
                // If A==0, then the line is verticular and thus the angle is PI/2. 
                if (line.A == RoundedDouble.Zero)
                    return 0;

                // Otherwise we calculate the scope of the line. The directional vector of the line is (-B, A).
                var scope = (RoundedDouble)Math.Atan(-line.A / line.B);

                // If the scope is less than zero, we'll normalize it to the interval [0, pi].
                if (scope < RoundedDouble.Zero)
                    return scope + Math.PI;

                // And return it
                return scope;
            }

            // Now we calculate the relative angles for our two lines and return their absolute difference
            var difference =  (RoundedDouble)Math.Abs(Angle(this) - Angle(otherLine));

            // If the difference happens to be at least PI/2, we want to normalize it to the interval [0,PI/2].
            if (difference >= Math.PI / 2)
                return Math.PI - difference;
            
            // And return the result
            return difference;
        }

        /// <summary>
        /// Determines if a given point lies on this line.
        /// </summary>
        /// <param name="point">The given point.</param>
        /// <returns>true, if the point lies on the line, false otherwise.</returns>
        public bool Contains(Point point)
        {
            // We simply check if the point's coordinates meets the equation
            return (RoundedDouble)(A * point.X + B * point.Y + C) == RoundedDouble.Zero;
        }

        /// <summary>
        /// Finds out if a given line is parallel to this one.
        /// </summary>
        /// <param name="otherLine">The line.</param>
        /// <returns>true, if the lines are parallel; false otherwise.s</returns>
        public bool IsParallelTo(Line otherLine)
        {
            // We simply check if these lines aren't equal and if there is no intersection between them
            return this == otherLine || IntersectionWith(otherLine) == null;
        }

        /// <summary>
        /// Finds out if a given line is perpendicular to this one.
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns></returns>
        public bool IsPerpendicularTo(Line otherLine)
        {
            // We construct some perpendicular line to this one
            // and check if it is parallel to the given line

            // First we need a some point on line
            var pointOnThisLine = PointOnLine();

            // Then we construct a perpendicular line at this point
            var perpendicularLine = PerpendicularLine(pointOnThisLine);

            // And return if this line is parallel to the given one
            return perpendicularLine.IsParallelTo(otherLine);
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
            return HashCodeUtilities.GetOrderDependentHashCode(A, B, C);
        }

        /// <summary>
        /// Returns if a given analytic object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected override bool IsEqualTo(Line other)
        {
            return A == other.A && B == other.B && C == other.C;
        }

        #endregion
    }
}