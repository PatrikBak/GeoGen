namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a construction whose constructor is implemented directly in the code.
    /// </summary>
    public enum PredefinedConstructionType
    {
        /// <summary>
        /// The circumcenter constructed from 3 points (signature {P, P, P}).
        /// </summary>
        CircumcenterFromPoints,

        /// <summary>
        /// The circle with given center passing through a given point (signature P, P).
        /// </summary>
        CircleFromCenterAndPointOnIt,

        /// <summary>
        /// The circumcircle constructed from 3 points (signature {P, P, P}).
        /// </summary>
        CircumcircleFromPoints,

        /// <summary>
        /// The internal angle bisector constructed from 3 points (signature P, {P, P}).
        /// </summary>
        InternalAngleBisectorFromPoints,

        /// <summary>
        /// The intersection of the two lines constructed from 4 points (signature {{P, P}, {P, P}}).
        /// </summary>
        IntersectionOfLinesFromPoints,

        /// <summary>
        /// The intersection of 2 lines (signature {L, L}).
        /// </summary>
        IntersectionOfLines,

        /// <summary>
        /// The intersection of the two lines constructed from a line and 2 points (signature L, {P, P}).
        /// </summary>
        IntersectionOfLinesFromLineAndPoints,

        /// <summary>
        /// A random point lying on the line constructed from 2 points (signature {P, P}).
        /// </summary>
        RandomPointOnLineSegment,

        /// <summary>
        /// A random point lying on a line (signature L).
        /// </summary>
        RandomPointOnLine,

        /// <summary>
        /// The midpoint of the line segment constructed from 2 points (signature {P, P}).
        /// </summary>
        MidpointFromPoints,

        /// <summary>
        /// The perpendicular line constructed from 3 points (signature P, {P, P}).
        /// </summary>
        PerpendicularLineFromPoints,

        /// <summary>
        /// The second intersection of a line given by two points A, B, and a circle passing through A and other two given
        /// points (signature P, P, {P, P}, where the first 'P' is meant to be B, and the second one is meant to be A)
        /// </summary>
        SecondIntersectionOfCircleFromPointsAndLineFromPoints,

        /// <summary>
        /// The second intersection of a circle given by points A, B, C, and a circle given by points A, C, D
        /// (signature P, {{P, P}, {P, P}})
        /// </summary>
        SecondIntersectionOfTwoCirclesFromPoints,

        /// <summary>
        /// The reflection of a point with respect to another point (signature P, P)
        /// </summary>
        PointReflection,
    }
}