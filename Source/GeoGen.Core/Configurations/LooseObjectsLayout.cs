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
        /// Represents three points A, B, C, where AB is supposed to be equal to AC.
        /// </summary>
        IsoscelesTriangle,

        /// <summary>
        /// Represents three points A, B, C such that angle BAC is right.
        /// </summary>
        RightTriangle,

        /// <summary>
        /// Represents four points A, B, C, D such that CD is the perpendicular bisector of AB.
        /// </summary>
        LineSegmentBisectedByLineFromPoints,

        /// <summary>
        /// Represents 4 points, where no 3 of them are collinear. When drawn they make 
        /// a convex quadrilateral. 
        /// </summary>
        FourPoints,

        /// <summary>
        /// Represents 4 points, where no 3 of them are collinear, that lie on a single circle.
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
        /// Represents points A, B, C, D, E, F such that there are three circles
        /// (ABCD), (CDEF), (EFAB) (from the radical axis theorem this means that
        /// lines AB, CD, EF are concurrent).
        /// </summary>
        ThreeCyclicQuadrilatersOnSixPoints,

        /// <summary>
        /// Represents a circle and two points A, B such that line AB is tangent to the circle.
        /// </summary>
        CircleAndItsTangentLineFromPoints
    }
}