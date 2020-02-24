using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a way in which a <see cref="LooseObjectHolder"/> is drawn by <see cref="IGeometryConstructor"/>/
    /// </summary>
    public enum LooseObjectDrawingStyle
    {
        /// <summary>
        /// Layouts are drawn in such a way that objects aren't too symmetric or regular. For example, triangles are
        /// generated as scalene non-isosceles ones. 
        /// <para>
        /// The reason why this is generation friendly is that we will less likely make state generally false theorems 
        /// that hold true only in more specific configurations.
        /// </para>
        /// </summary>
        GenerationFriendly,

        /// <summary>
        /// Layouts are drawn without trying to be as general as possible, for example a triangle might be randomly close
        /// to an isosceles or even equilateral one.
        /// </summary>
        Standard
    }
}
