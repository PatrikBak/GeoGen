namespace GeoGen.Core
{
    /// <summary>
    /// Represents a way in which the loose objects of a configuration are arranged.
    /// </summary>
    public enum LooseObjectsLayout
    {
        /// <summary>
        /// Represents 2 points.
        /// </summary>
        LineSegment,

        /// <summary>
        /// Represents three points that are not collinear. When drawn, the triangle they
        /// make have all its angles are acute.
        /// </summary>
        Triangle,

        /// <summary>
        /// Represents three points A, B, C, where AB = AC.
        /// </summary>
        IsoscelesTriangle,

        /// <summary>
        /// Represents three points A, B, C such that angle BAC is right.
        /// </summary>
        RightTriangle,

        /// <summary>
        /// Represents 4 points, where no 3 of them are collinear. When drawn they make 
        /// a convex quadrilateral. 
        /// </summary>
        Quadrilateral,

        /// <summary>
        /// Represents 4 points that lie on a single circle.
        /// </summary>
        CyclicQuadrilater,

        /// <summary>
        /// Represents points A, B, C, D such that AB is parallel to CD.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Represents points A, B, C, D such that DA is tangent to circle (ABC).
        /// </summary>
        CircleAndTangentLine,

        /// <summary>
        /// Represents a line
        /// </summary>
        ExplicitLine,

        /// <summary>
        /// Represents a line and a point that doesn't lie on it.
        /// </summary>
        ExplicitLineAndPoint,

        /// <summary>
        /// Represents a line and two distinct points that don't lie on it.
        /// </summary>
        ExplicitLineAndTwoPoints,

        /// <summary>
        /// Represents a circle.
        /// </summary>
        ExplicitCircle,

        /// <summary>
        /// Represents a circle and a point that doesn't lie on it.
        /// </summary>
        ExplicitCircleAndPoint,

        /// <summary>
        /// Represents a circle and two distinct points that don't lie on it.
        /// </summary>
        ExplicitCircleAndTwoPoints
    }
}