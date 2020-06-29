using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a rectangular 2D box that bounds a set of points.
    /// </summary>
    public struct BoundingBox
    {
        #region Public properties

        /// <summary>
        /// The upper-left corner of the rectangular box.
        /// </summary>
        public Point UpperLeftCorner { get; }

        /// <summary>
        /// The upper-right corner of the rectangular box.
        /// </summary>
        public Point UpperRightCorner { get; }

        /// <summary>
        /// The bottom-left corner of the rectangular box.
        /// </summary>
        public Point BottomLeftCorner { get; }

        /// <summary>
        /// The bottom-right corner of the rectangular box.
        /// </summary>
        public Point BottomRightCorner { get; }

        /// <summary>
        /// The center of the box.
        /// </summary>
        public Point Center => (BottomLeftCorner + UpperRightCorner) / 2;

        /// <summary>
        /// The line through <see cref="UpperLeftCorner"/> and <see cref="UpperRightCorner"/>.
        /// </summary>
        public Line UpperLine { get; }

        /// <summary>
        /// The line through <see cref="BottomLeftCorner"/> and <see cref="BottomRightCorner"/>.
        /// </summary>
        public Line BottomLine { get; }

        /// <summary>
        /// The line through <see cref="UpperRightCorner"/> and <see cref="BottomRightCorner"/>.
        /// </summary>
        public Line RightLine { get; }

        /// <summary>
        /// The line through <see cref="UpperLeftCorner"/> and <see cref="BottomLeftCorner"/>.
        /// </summary>
        public Line LeftLine { get; }

        /// <summary>
        /// The width of the rectangular box.
        /// </summary>
        public double Width => UpperRightCorner.X - UpperLeftCorner.X;

        /// <summary>
        /// The width of the rectangular box.
        /// </summary>
        public double Height => UpperLeftCorner.Y - BottomLeftCorner.Y;

        /// <summary>
        /// The are of the rectangular box.
        /// </summary>
        public double Area => Width * Height;

        /// <summary>
        /// A shortcut to enumerate the boundary points of the box.
        /// </summary>
        public IEnumerable<Point> BoundaryPoints => new[] { UpperRightCorner, UpperLeftCorner, BottomLeftCorner, BottomRightCorner };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> structure.
        /// </summary>
        /// <param name="points">The points to be tightly bounded by the box together with the passed circles.</param>
        /// <param name="circles">The circles to be tightly bounded by the box together with the passed points.</param>
        public BoundingBox(IEnumerable<Point> points, IEnumerable<Circle> circles)
        {
            // Every circle will 'add' four points
            var relevantPoints = circles.SelectMany(circle => new[]
                {
                    // Upper point
                    circle.Center + new Point(0, circle.Radius),

                    // Bottom point
                    circle.Center + new Point(0, -circle.Radius),

                    // Right point
                    circle.Center + new Point(circle.Radius, 0),

                    // Left point
                    circle.Center + new Point(-circle.Radius, 0)
                })
                // Also the passed points
                .Concat(points)
                // Enumerate
                .ToArray();

            // In order to find the box's corners look for the maximal and minimal X and Y
            var minX = relevantPoints.Select(point => point.X).Min();
            var maxX = relevantPoints.Select(point => point.X).Max();
            var minY = relevantPoints.Select(point => point.Y).Min();
            var maxY = relevantPoints.Select(point => point.Y).Max();

            // Now we can construct the corners
            UpperRightCorner = new Point(maxX, maxY);
            UpperLeftCorner = new Point(minX, maxY);
            BottomLeftCorner = new Point(minX, minY);
            BottomRightCorner = new Point(maxX, minY);

            // Also we can construct the lines
            UpperLine = new Line(UpperRightCorner, UpperLeftCorner);
            BottomLine = new Line(BottomRightCorner, BottomLeftCorner);
            LeftLine = new Line(UpperLeftCorner, BottomLeftCorner);
            RightLine = new Line(UpperRightCorner, BottomRightCorner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> structure that
        /// encloses the passed points.
        /// </summary>
        /// <param name="points">The points to be tightly bounded by the box.</param>
        public BoundingBox(IEnumerable<Point> points) : this(points, Enumerable.Empty<Circle>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> structure that
        /// encloses the passed points.
        /// </summary>
        /// <param name="points">The points to be tightly bounded by the box.</param>
        public BoundingBox(params Point[] points) : this((IEnumerable<Point>)points)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds out if the passed point lies inside the box or on the boundary.
        /// </summary>
        /// <param name="point">The point to be checked.</param>
        /// <returns>true, if the point is inside the box or on its boundary; false otherwise.</returns>
        public bool IsPointWithinTheBox(Point point)
            // This is true if the passed point is to the right of the leftmost point
            => point.X.Rounded() >= UpperLeftCorner.X.Rounded()
                // But to the left of the rightmost coordinate
                && point.X.Rounded() <= UpperRightCorner.X.Rounded()
                // And below the topmost coordinate
                && point.Y.Rounded() <= UpperLeftCorner.Y.Rounded()
                // But above the bottommost one
                && point.Y.Rounded() >= BottomLeftCorner.Y.Rounded();

        /// <summary>
        /// Scales the box by a given coefficient from the center.
        /// </summary>
        /// <param name="coefficient">The coefficient by which the box should be scaled.</param>
        /// <returns>The scaled box.</returns>
        public BoundingBox Scale(double coefficient)
            // We need to shift so that Center becomes (0, 0), then scale, then shift back
            => new BoundingBox((UpperLeftCorner - Center) * coefficient + Center, (BottomRightCorner - Center) * coefficient + Center);

        /// <summary>
        /// Finds all intersection points of the boxes' boundary with the passed line. 
        /// If the line is equal to one of the border lines, then the result will be equal
        /// to the corresponding corner points
        /// </summary>
        /// <param name="line">The line to be tested for intersection points.</param>
        /// <returns>The array of intersection points of the line and the box.</returns>
        public Point[] IntersectWith(Line line)
            // If the line is equal to the one of the lines
            => new[] { BottomLine, UpperLine, LeftLine, RightLine }.Contains(line)
                // Then we would return the corresponding boundary points
                ? new[] { UpperRightCorner, UpperLeftCorner, BottomLeftCorner, BottomRightCorner }
                    // Which lie on this line
                    .Where(line.Contains)
                    // Enumerate
                    .ToArray()
                // If the line is not boundary, take them
                : new[] { UpperLine, BottomLine, LeftLine, RightLine }
                    // Intersect with each
                    .Select(boundaryLine => boundaryLine.IntersectionWith(line))
                    // Take existing intersections
                    .Where(intersection => intersection != null)
                    // Unwrap
                    .Select(point => point.Value)
                    // Take those within
                    .Where(IsPointWithinTheBox)
                    // Distinct ones
                    .Distinct()
                    // Enumerate
                    .ToArray();

        /// <summary>
        /// Finds all intersection points of the boxes' boundary with the passed circle.
        /// </summary>
        /// <param name="circle">The circle to be tested for intersection points.</param>
        /// <returns>The array of intersection points of the circle and the box.</returns>
        public Point[] IntersectWith(Circle circle)
            // Take the boundary lines
            => new[] { UpperLine, BottomLine, LeftLine, RightLine }
                // Intersect each
                .SelectMany(boundaryLine => circle.IntersectWith(boundaryLine))
                // Take those within
                .Where(IsPointWithinTheBox)
                // Distinct ones
                .Distinct()
                // Enumerate
                .ToArray();
    }

    #endregion
}
