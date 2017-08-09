namespace GeoGen.Core.Constructions
{
    /// <summary>
    /// Represents a predifined construction type,such as Midpoint, Interestion. The module 
    /// implementing actual geometry is supposed to properly implement all the cases. 
    /// </summary>
    public enum PredefinedConstructionType
    {
        /// <summary>
        /// Construction of the midpoint between two points.
        /// </summary>
        Midpoint,

        /// <summary>
        /// Construction of the interesection of two lines.
        /// </summary>
        Intersection,

        /// <summary>
        /// Construction of the orthogonal projection of a point onto a line.
        /// </summary>
        Projection,

        /// <summary>
        /// Constructin of the circumcirle of a triangle.
        /// </summary>
        Circumcircle
    }
}