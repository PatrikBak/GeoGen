namespace GeoGen.Core
{
    /// <summary>
    /// Represents a way in which the loose objects of a configuration are arranged.  
    /// </summary>
    public enum LooseObjectsLayout
    {
        /// <summary>
        /// Represents a layout with arbitrary number of objects of any type.
        /// </summary>
        NoLayout,

        /// <summary>
        /// Represented a triangle whose angles are all acute and distinct.
        /// </summary>
        ScaleneAcuteTriangle,

        /// <summary>
        /// Represents points A, B, C, D, E, F such that there are three circles
        /// (ABCD), (CDEF), (EFAB) (from the radical axis theorem this means that
        /// lines AB, CD, EF are concurrent).
        /// </summary>
        ThreeCyclicQuadrilatersOnSixPoints,

        /// <summary>
        /// Represents points A, B, C, D such that AB is parallel to CD.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Represents a circle and two points A, B such that line AB is tangent to the circle.
        /// </summary>
        CircleAndItsTangentLine
    }
}