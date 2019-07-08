namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a construction whose constructor is implemented directly in the code.
    /// </summary>
    public enum PredefinedConstructionType
    {
        /// <summary>
        /// The line through 2 points A, B (signature {A, B}).
        /// </summary>
        LineFromPoints,

        /// <summary>
        /// The circle through 3 points A, B, C (signature {A, B, C}).
        /// </summary>
        Circumcircle,

        /// <summary>
        /// The circle with center A passing through point B (signature A, B).
        /// </summary>
        CircleWithCenterThroughPoint,

        /// <summary>
        /// The center of circle c. (signature c).
        /// </summary>
        CenterOfCircle,

        /// <summary>
        /// The intersection of 2 lines l, m (signature {l, m}).
        /// </summary>
        IntersectionOfLines,

        /// <summary>
        /// The internal angle bisector of angle BAC (signature A, {B, C}).
        /// </summary>
        InternalAngleBisector,

        /// <summary>
        /// The reflection of point A by point B (signature A, B).
        /// </summary>
        PointReflection,

        /// <summary>
        /// The midpoint of line segment AB (signature {A, B}).
        /// </summary>
        Midpoint,

        /// <summary>
        /// The perpendicular projection of point A on line l. (signature A, l)
        /// </summary>
        PerpendicularProjection,

        /// <summary>
        /// The line passing through point A and perpendicular to line l. (signature A, l)
        /// </summary>
        PerpendicularLine,

        /// <summary>
        /// The line passing through point A and parallel to line l (signature A, l).
        /// </summary>
        ParallelLine,

        /// <summary>
        /// The second intersection of the circle given by points A, B, C and the circle given by points A, D, E (signature A, {{B, C}, {D, E}}).
        /// </summary>
        SecondIntersectionOfTwoCircumcircles,

        /// <summary>
        /// The second intersection of line AB, and circle given by points A, C, D (signature A, B, {C, D}).
        /// </summary>
        SecondIntersectionOfCircleAndLineFromPoints,

        /// <summary>
        /// The second intersection of line AB with the circle with center C passing through A (signature A, B, C).
        /// </summary>
        SecondIntersectionOfCircleWithCenterAndLineFromPoints,

        /// <summary>
        /// A random point lying on line AB (signature {A, B}).
        /// </summary>
        RandomPointOnLineFromPoints,

        /// <summary>
        /// A random point lying on line l (signature l).
        /// </summary>
        RandomPointOnLine,

        /// <summary>
        /// A random point lying on circle c (signature c).
        /// </summary>
        RandomPointOnCircle,

        /// <summary>
        /// A random point in a plane (empty signature).
        /// </summary>
        RandomPoint
    }
}