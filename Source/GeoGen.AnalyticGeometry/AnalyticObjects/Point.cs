﻿using System;
using System.Linq;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a geometrical 2D point.
    /// </summary>
    public class Point : AnalyticObjectBase<Point>
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
        public Point Midpoint(Point otherPoint)
        {
            return (this + otherPoint) / 2;
        }

        /// <summary>
        /// Calculates the euclidean distance to a given other point.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>The distance to the other point.</returns>
        public double DistanceTo(Point otherPoint)
        {
            var dx = X - otherPoint.X;
            var dy = Y - otherPoint.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Constructs the internal angle bisector of the angle given by [ray1Point][this][ray2Point]
        /// (or equivalently [ray2Point][this][ray1Point]). All three points
        /// must be distinct and not collinear. 
        /// </summary>
        /// <param name="ray1Point">The first point on the ray starting from this point.</param>
        /// <param name="ray2Point">The second point on the ray starting from this point.</param>
        /// <returns>The internal angle bisector.</returns>
        public Line InternalAngleBisector(Point ray1Point, Point ray2Point)
        {
            // First we create the circle with center in [this] and radius
            // equal to distance between dist and first ray point
            var circle = new Circle(this, ray1Point.DistanceTo(this));

            // Now we create the circle connecting [this] and the second ray point
            var line = new Line(this, ray2Point);

            // And intersect them
            var intersections = circle.IntersectWith(line);

            // Logically we must have 2 intersections. We need to pick
            // the one lying on the ray from [this] to the second point

            // Generally, if we have a point C that lies on the line AB, then
            // there exists a real number 't' such that t(B-A) = (C-A)/(B-A).
            // Point C then lies on the ray AB if and only if t >= 0 (if t=0, then C=A)
            // Since we know that C lies on AB, so we can choose whether to calculate
            // the t from X, or from Y coordinates. However, one of these differences
            // could be 0, hence we have to put the non-zero one. At least one of them
            // must be non zero (cause points C, A are distinct).
            //
            // A = [this]
            // B = ray2Point
            // C = one of intersections
            //
            // The intersection must logically exist
            var correctIntersection = intersections.First(i =>
            {
                // Calculate the X coordinate of the vector (C-A)
                var dx1 = ray2Point.X - X;

                // If it's not zero
                if (dx1.Rounded() != 0)
                {
                    // Then we calculate the X coordinate of the vector B-A
                    var dx2 = i.X - X;

                    // And return if the corresponding t, that is equal to dx1 / dx2, is at least zero
                    return (dx1 / dx2).Rounded() >= 0;
                }

                // Otherwise dx1 is zero and we have to use the Y differences. Calculate them
                var dy1 = ray2Point.Y - Y;
                var dy2 = i.Y - Y;

                // And return if their ration is at least zero...
                return (dy1 / dy2).Rounded() >= 0;
            });

            // Now we just return the perpendicular bisector of the line 
            // connecting the first ray point with this correct intersection
            return ray1Point.PerpendicularBisector(correctIntersection);
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
            // Get the perpendicular line
            var perpendicularLine = line.PerpendicularLine(this);

            // Intersect it with the given line
            var intersection = perpendicularLine.IntersectionWith(line);

            // Get the result. It definitely should exist
            var result = intersection ?? throw new Exception("Math has stopped working. The world is about to end.");

            // Return the result
            return result;
        }

        #endregion

        #region Overloaded operators

        /// <summary>
        /// Adds up two points by adding their corresponding coordinates.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>The sum of the points.</returns>
        public static Point operator +(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        /// <summary>
        /// Scales a point by dividing the coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator /(Point point, double factor)
        {
            return new Point(point.X / factor, point.Y / factor);
        }

        /// <summary>
        /// Scales a point by multiplying the coordinates by a given factor.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The scaled point.</returns>
        public static Point operator *(Point point, double factor)
        {
            return new Point(point.X * factor, point.Y * factor);
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
            return new { x = X.Rounded(), y = Y.Rounded() }.GetHashCode();
        }

        /// <summary>
        /// Returns if a given analytic object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected override bool IsEqualTo(Point other)
        {
            return X.Rounded() == other.X.Rounded() && Y.Rounded() == other.Y.Rounded();
        }

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