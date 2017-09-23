namespace GeoGen.Core.Constructions.PredefinedConstructions
{
    /// <summary>
    /// Represents a predefined construction type,such as Midpoint, Intersection. The module 
    /// implementing actual geometry is supposed to properly implement all the cases. 
    /// </summary>
    public enum PredefinedConstructionType
    {
        /// <summary>
        /// Construction of the midpoint between two points.
        /// </summary>
        Midpoint,

        /// <summary>
        /// Construction of the intersection of two lines.
        /// </summary>
        Intersection,

        /// <summary>
        /// Construction of the orthogonal projection of a point onto a line.
        /// </summary>
        Projection,

        /// <summary>
        /// Construction of the circumcircle of a triangle.
        /// </summary>
        Circumcircle
    }
}