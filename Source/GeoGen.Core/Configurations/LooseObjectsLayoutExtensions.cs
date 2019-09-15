using System.Collections.Generic;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;

namespace GeoGen.Core
{
    /// <summary>
    /// Extension methods for <see cref="LooseObjectsLayout"/>.
    /// </summary>
    public static class LooseObjectsLayoutExtensions
    {
        /// <summary>
        /// Gets the type of objects required by this layout. .
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>The list of object types.</returns>
        public static IReadOnlyList<ConfigurationObjectType> ObjectTypes(this LooseObjectsLayout layout) =>

            // Switch based on the layout
            layout switch
            {
                // 3 points
                ThreePoints => new[] { Point, Point, Point },

                // 4 points
                Trapezoid => new[] { Point, Point, Point, Point },

                // 4 points
                CircleAndTangentLine => new[] { Point, Point, Point, Point },

                // 4 points
                FourPoints => new[] { Point, Point, Point, Point },

                // 4 points
                FourConcyclicPoints => new[] { Point, Point, Point, Point },

                // 1 line, 1 point
                LineAndPoint => new[] { Line, Point },

                // 1 line, 2 points
                LineAndTwoPoints => new[] { Line, Point, Point },

                // 2 points
                TwoPoints => new[] { Point, Point },

                // 3 points
                IsoscelesTriangle => new[] { Point, Point, Point },

                // 3 points
                RightTriangle => new[] { Point, Point, Point },

                // Default case
                _ => throw new GeoGenException($"The layout '{layout}' doesn't have the object types defined."),
            };

    }
}