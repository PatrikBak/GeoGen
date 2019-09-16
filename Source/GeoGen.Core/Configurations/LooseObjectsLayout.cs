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
        TwoPoints,

        /// <summary>
        /// Represents three points that are not collinear. When drawn, the triangle they
        /// make have all its angles are acute.
        /// </summary>
        ThreePoints,

        /// <summary>
        /// Represents three points that might or might not be collinear.
        /// </summary>
        ThreeArbitraryPoints,

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
        FourPoints,

        /// <summary>
        /// Represents 4 points that lie on a single circle.
        /// </summary>
        FourConcyclicPoints,

        /// <summary>
        /// Represents a line and a point that doesn't lie on it.
        /// </summary>
        LineAndPoint,

        /// <summary>
        /// Represents a line l and two points A, B  where A and B don't lie on l.
        /// </summary>
        LineAndTwoPoints,

        /// <summary>
        /// Represents points A, B, C, D such that AB is parallel to CD.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Represents points A, B, C, D such that DA is tangent to circle (ABC).
        /// </summary>
        CircleAndTangentLine
    }
}