using GeoGen.Utilities;
using System;
using System.Globalization;
using static System.Math;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometric 2D point. It is given by two coordinates X and Y.
    /// </summary>
    public readonly struct Point : IAnalyticObject, IEquatable<Point>
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

        /// <summary>
        /// The distance of the point to the point [0, 0].
        /// </summary>
        public double DistanceToOrigin => DistanceTo(new Point(0, 0));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> structure.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Point(double x, double y) => (X, Y) = (x, y);

        #endregion

        #region Public methods

        /// <summary>
        /// Rotates this point around a given center by a given angle. 
        /// </summary>
        /// <param name="center">The center of the rotation.</param>
        /// <param name="angleInDegrees">The given angle in degrees.</param>
        /// <returns>The rotated point.</returns>
        public Point Rotate(Point center, double angleInDegrees)
        {
            // First we convert the angle to radians
            var angleInRadians = MathHelpers.ToRadians(angleInDegrees);

            // Precalculate sin and cos of the angle
            var cosT = Cos(angleInRadians);
            var sinT = Sin(angleInRadians);

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
        /// Calculates the Euclidean distance to a given other point.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>The distance to the other point.</returns>
        public double DistanceTo(Point otherPoint) => Sqrt((X - otherPoint.X).Squared() + (Y - otherPoint.Y).Squared());

        /// <summary>
        /// Creates the perpendicular projection of this point onto a given line.
        /// If this point lines on the line, the result will be the point itself.
        /// </summary>
        /// <param name="line">The line onto which this point should be projected.</param>
        /// <returns>The projection of this point onto the given line.</returns>
        public Point Project(Line line)
        {
            // Get the perpendicular line passing through this point
            var perpendicularLine = line.PerpendicularLineThroughPoint(this);

            // Intersect it with the given line
            var intersection = perpendicularLine.IntersectionWith(line);

            // Return the intersection. It definitely should exist
            return intersection ?? throw new AnalyticException("This intersection should have existed. The precision is really flawed.");
        }

        #endregion

        #region Overloaded operators

        /// <summary>
        /// Adds two points by adding their corresponding coordinates.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>The sum of the points.</returns>
        public static Point operator +(Point point1, Point point2) => new Point(point1.X + point2.X, point1.Y + point2.Y);

        /// <summary>
        /// Subtracts two points by subtracting their corresponding coordinates.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>The different between the points.</returns>
        public static Point operator -(Point point1, Point point2) => new Point(point1.X - point2.X, point1.Y - point2.Y);

        /// <summary>
        /// Scales a point by dividing its coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator /(Point point, double factor) => new Point(point.X / factor, point.Y / factor);

        /// <summary>
        /// Scales a point by multiplying its coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator *(Point point, double factor) => new Point(point.X * factor, point.Y * factor);

        /// <summary>
        /// Scales a point by multiplying its coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator *(double factor, Point point) => new Point(point.X * factor, point.Y * factor);

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

        /// <inheritdoc/>
        public override int GetHashCode() => (X.Rounded(), Y.Rounded()).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Do the null and then the type check and then call the other Equals method
            => otherObject != null && otherObject is Point point && Equals(point);

        #endregion

        #region IEquatable implementation

        /// <inheritdoc/>
        public bool Equals(Point otherPoint) => X.Rounded() == otherPoint.X.Rounded() && Y.Rounded() == otherPoint.Y.Rounded();

        #endregion

        #region To String

        /// <inheritdoc/>
        public override string ToString() => $"({X.ToString(CultureInfo.InvariantCulture)}, {Y.ToString(CultureInfo.InvariantCulture)})";

        #endregion
    }
}