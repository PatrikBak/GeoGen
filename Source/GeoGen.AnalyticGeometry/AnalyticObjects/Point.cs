using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometric 2D point.
    /// </summary>
    public struct Point : IAnalyticObject, IEquatable<Point>
    {
        #region Public properties

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public double Y { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs the perpendicular of the line consisting of this
        /// point and a given other point. The other point must be
        /// distinct from this one.
        /// </summary>
        /// <param name="otherPoint">The other points.</param>
        /// <returns>The perpendicular bisector.</returns>
        public Line PerpendicularBisector(Point otherPoint)
        {
            // First we get the midpoint between these two points
            // This is the first point lying on the desired line
            var midpoint = Midpoint(otherPoint);

            // Rotate this point around the midpoint by 90 degrees 
            // This is the second point lying on the desired line
            var rotatedPoint = Rotate(midpoint, 90);

            // We can construct the line from these two points
            return new Line(midpoint, rotatedPoint);
        }

        /// <summary>
        /// Rotates this point around a given center by a given angle. 
        /// </summary>
        /// <param name="center">The center of the rotation.</param>
        /// <param name="angleInDegrees">The given angle in degrees.</param>
        /// <returns>The rotated point.</returns>
        public Point Rotate(Point center, double angleInDegrees)
        {
            // First we convert the angle to radians
            var angleInRadians = MathematicalHelpers.ToRadians(angleInDegrees);

            // Precalculate sin and cos of the angle
            var cosT = Math.Cos(angleInRadians);
            var sinT = Math.Sin(angleInRadians);

            // The general rotation matrix in 2D is 
            // 
            // cos(T)  -sin(T)
            // sin(T)   cos(T)
            //
            // To use this we first need to translate the point to the origin

            // Calculate the coordinates of the translated point
            var dx = X - center.X;
            var dy = Y - center.Y;

            // Now we use the matrix
            var newX = cosT * dx - sinT * dy;
            var newY = sinT * dx + cosT * dy;

            // And we use the reversed translation to get the result
            return new Point(newX + center.X, newY + center.Y);
        }

        /// <summary>
        /// Gets the midpoint of the line segment connecting
        /// this point with a given other point.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>The midpoint.</returns>
        public Point Midpoint(Point otherPoint) => (this + otherPoint) / 2;

        /// <summary>
        /// Calculates the euclidean distance to a given other point.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>The distance to the other point.</returns>
        public double DistanceTo(Point otherPoint) => Math.Sqrt((X - otherPoint.X).Squared() + (Y - otherPoint.Y).Squared());

        /// <summary>
        /// Constructs the internal angle bisector of the angle [ray1Point][this][ray2Point]
        /// (or equivalently [ray2Point][this][ray1Point]). All three points must be distinct
        /// and not collinear. 
        /// </summary>
        /// <param name="ray1Point">The first point on the ray starting from [this] point.</param>
        /// <param name="ray2Point">The second point on the ray starting from [this] point.</param>
        /// <returns>The internal angle bisector of the angle [ray1Point][this][ray2Point].</returns>
        public Line InternalAngleBisector(Point ray1Point, Point ray2Point)
        {
            // Denote A = [this], B = [ray1Point], C = [ray2Point]
            // We're going to construct the line AX, where X is the intersection
            // of the internal angle bisector of BAC with BC. It's known that
            // BX / CX = AB / AC. One can use this to calculate the relation
            // X = B + ||AB||/(||AB||+||AC||) . (C-B) (this is a vector relation)

            // First calculate the distances
            var distanceAB = DistanceTo(ray1Point);
            var distanceAC = DistanceTo(ray2Point);

            // Make sure their sum is not null...
            if ((distanceAB + distanceAC).Rounded() == 0)
                throw new AnalyticException("Cannot construct the angle bisector using equal points");

            // Calculate the ratio
            var ratio = distanceAB / (distanceAB + distanceAC);

            // Construct the coordinates of point X
            var x = ray1Point.X + ratio * (ray2Point.X - ray1Point.X);
            var y = ray1Point.Y + ratio * (ray2Point.Y - ray1Point.Y);

            // Construct X
            var X = new Point(x, y);

            // And finally line AX
            return new Line(this, X);
        }

        /// <summary>
        /// Creates the perpendicular projection of this point onto
        /// a given line. If this point lines on the line, the result
        /// will be the point itself.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The projection.</returns>
        public Point Project(Line line)
        {
            // Get the perpendicular line passing through this point
            var perpendicularLine = line.PerpendicularLine(this);

            // Intersect it with the given line
            var intersection = perpendicularLine.IntersectionWith(line);

            // Return the intersection. It definitely should exist
            return intersection ?? throw new AnalyticException("This intersection should have existed. The precision is really flawed.");
        }

        #endregion

        #region Overloaded operators

        /// <summary>
        /// Adds up two points by adding their corresponding coordinates.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>The sum of the points.</returns>
        public static Point operator +(Point point1, Point point2) => new Point(point1.X + point2.X, point1.Y + point2.Y);

        /// <summary>
        /// Scales a point by dividing the coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator /(Point point, double factor) => new Point(point.X / factor, point.Y / factor);

        /// <summary>
        /// Scales a point by multiplying the coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator *(Point point, double factor) => new Point(point.X * factor, point.Y * factor);

        /// <summary>
        /// Checks if two points are equal.
        /// </summary>
        /// <param name="left">The left point.</param>
        /// <param name="right">The right point.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public static bool operator ==(Point left, Point right) => left.Equals(right);

        /// <summary>
        /// Checks if two points are not equal.
        /// </summary>
        /// <param name="left">The left point.</param>
        /// <param name="right">The right point.</param>
        /// <returns>true, if they are not equal; false otherwise.</returns>
        public static bool operator !=(Point left, Point right) => !left.Equals(right);

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
            return otherObject != null && otherObject is Point point && Equals(point);
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // Let's use the built-in .NET hash code calculator for anonymous types
            return new { x = X.Rounded(), y = Y.Rounded() }.GetHashCode();
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Finds out if the passed point is equal to this one.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(Point otherPoint) => X.Rounded() == otherPoint.X.Rounded() && Y.Rounded() == otherPoint.Y.Rounded();

        #endregion

        #region To String

        /// <summary>
        /// Converts a given point to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the point.</returns>
        public override string ToString() => $"({X}, {Y})";

        #endregion
    }
}