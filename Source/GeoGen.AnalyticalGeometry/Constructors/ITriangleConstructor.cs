using System.Collections.Generic;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// Represents a constructor of random triangles with given properties.
    /// </summary>
    public interface ITriangleConstructor
    {
        /// <summary>
        /// Constructs a random scalene acute-angled triangle.
        /// </summary>
        /// <returns>The list of three points that make the triangle.</returns>
        List<AnalyticalObject> NextScaleneAcuteAngedTriangle();
    }
}