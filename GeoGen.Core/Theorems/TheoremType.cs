namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a <see cref="Theorem"/>.
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
        /// Three or more concurrent objects that each is either a line, or a circle
        /// </summary>
        ConcurrentObjects,

        /// <summary>
        /// Two or more lines parallel to each other
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
        /// A line tangent to a circle
        /// </summary>
        LineTangentToCircle,

        /// <summary>
        /// Two line segments with the equal lengths
        /// </summary>
        EqualLineSegments,

        /// <summary>
        /// Two pairs of lines that make the same angles
        /// </summary>
        EqualAngles
    }
}