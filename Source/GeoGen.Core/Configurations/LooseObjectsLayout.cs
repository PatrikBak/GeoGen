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
        ScaleneAcuteAngledTriangled,

        /// <summary>
        /// Represents two circles intersecting at two different points. This layout 
        /// consist of 4 loose objects, the 2 circles and the 2 intersection points
        /// </summary>
        IntersectingCircles        
    }
}