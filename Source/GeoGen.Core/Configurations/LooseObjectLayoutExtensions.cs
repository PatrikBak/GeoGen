using System.Collections.Generic;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectLayout;

namespace GeoGen.Core
{
    /// <summary>
    /// Extension methods for <see cref="LooseObjectLayout"/>.
    /// </summary>
    public static class LooseObjectLayoutExtensions
    {
        /// <summary>
        /// Gets the types of objects required by this layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>The list of object types.</returns>
        public static IReadOnlyList<ConfigurationObjectType> ObjectTypes(this LooseObjectLayout layout) =>

            // Switch based on the layout
            layout switch
            {
                // 2 points
                LineSegment => new[] { Point, Point },

                // 3 points
                Triangle => new[] { Point, Point, Point },

                // 3 points
                RightTriangle => new[] { Point, Point, Point },

                // 4 points
                Quadrilateral => new[] { Point, Point, Point, Point },

                // 4 points
                CyclicQuadrilateral => new[] { Point, Point, Point, Point },

                // 1 line, 1 point
                LineAndPoint => new[] { Line, Point },

                // 1 line, 2 points
                LineAndTwoPoints => new[] { Line, Point, Point },

                // Unhandled cases
                _ => throw new GeoGenException($"The layout '{layout}' doesn't have the object types defined."),
            };
    }
}