namespace GeoGen.Core
{
    /// <summary>
    /// Represents a way in which the loose objects of a configuration are arranged.  
    /// </summary>
    public enum LooseObjectsLayout
    {
        /// Represents an empty layout, i.e. objects are arranged with no rules.
        None,

        /// <summary>
        /// Represented a triangle whose angles are all acute and their mutual differences
        /// are at least some value (so the triangle is not isosceles).
        /// </summary>
        ScaleneAcuteAngledTriangled
    }
}