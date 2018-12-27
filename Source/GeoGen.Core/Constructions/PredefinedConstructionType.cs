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
        /// A random points lying on the line constructed from 2 points (signature {P, P}).
        /// </summary>
        RandomPointOnLineFromPoints,

        /// <summary>
        /// The midpoint of the line segment constructed from 2 points (signature {P, P}).
        /// </summary>
        MidpointFromPoints,

        /// <summary>
        /// The perpendicular line constructed from 3 points (signature P, {P, P}).
        /// </summary>
        PerpendicularLineFromPoints
    }
}