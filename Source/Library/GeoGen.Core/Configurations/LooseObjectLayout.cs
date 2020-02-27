namespace GeoGen.Core
{
    /// <summary>
    /// Represents a way in which the loose objects of a configuration are arranged.
    /// </summary>
    public enum LooseObjectLayout
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
        /// Represents three points A, B, C such that angle BAC is right.
        /// </summary>
        RightTriangle,

        /// <summary>
        /// Represents 4 points, where no 3 of them are collinear. When drawn they make 
        /// a convex quadrilateral. 
        /// </summary>
        Quadrilateral,

        /// <summary>
        /// Represents 4 points on a circle, where no 3 of them are collinear. When drawn they make 
        /// a convex (cyclic) quadrilateral. 
        /// </summary>
        CyclicQuadrilateral,

        /// <summary>
        /// Represents a line and a point that doesn't lie on it.
        /// </summary>
        LineAndPoint,

        /// <summary>
        /// Represents a line and two distinct points that don't lie on it.
        /// </summary>
        LineAndTwoPoints
    }
}