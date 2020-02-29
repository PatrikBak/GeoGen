namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of <see cref="Theorem"/> that are examined.
    /// </summary>
    public enum TheoremType
    {
        /// <summary>
        /// Three or more points are collinear
        /// </summary>
        CollinearPoints,

        /// <summary>
        /// Four or more points are concyclic
        /// </summary>
        ConcyclicPoints,

        /// <summary>
        /// Three objects are concurrent, each of them is either a line or circle
        /// </summary>
        ConcurrentObjects,

        /// <summary>
        /// Three lines are concurrent. NOTE: This is a subset of ConcurrentObjects.
        /// </summary>
        ConcurrentLines,

        /// <summary>
        /// Two lines are parallel to each other
        /// </summary>
        ParallelLines,

        /// <summary>
        /// Two lines are perpendicular to each other
        /// </summary>
        PerpendicularLines,

        /// <summary>
        /// Two circles are tangent to each other
        /// </summary>
        TangentCircles,

        /// <summary>
        /// A line is tangent to a circle
        /// </summary>
        LineTangentToCircle,

        /// <summary>
        /// Two line segments have equal lengths
        /// </summary>
        EqualLineSegments,

        /// <summary>
        /// Two pairs of lines make the same angles
        /// </summary>
        EqualAngles,

        /// <summary>
        /// Two objects with formally different definitions represent the same geometric object
        /// </summary>
        EqualObjects,

        /// <summary>
        /// A point lies on a line or circle.
        /// </summary>
        Incidence
    }
}