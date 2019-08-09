using System.Collections.Generic;
using static GeoGen.Core.ConfigurationObjectType;

namespace GeoGen.Core
{
    /// <summary>
    /// Extension methods for <see cref="LooseObjectsLayout"/>.
    /// </summary>
    public static class LooseObjectsLayoutExtentions
    {
        /// <summary>
        /// Gets the type of objects required by this layout. The layout cannot be <see cref="LooseObjectsLayout.NoLayout"/>.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns>The list of object types.</returns>
        public static IReadOnlyList<ConfigurationObjectType> ObjectTypes(this LooseObjectsLayout layout) =>

            // Switch based on the layout
            layout switch
            {
                // 3 points
                LooseObjectsLayout.ThreePoints => new List<ConfigurationObjectType> { Point, Point, Point },

                // 6 points
                LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints => new List<ConfigurationObjectType> { Point, Point, Point, Point, Point, Point },

                // 4 points
                LooseObjectsLayout.Trapezoid => new List<ConfigurationObjectType> { Point, Point, Point, Point },

                // 1 circle, 2 points
                LooseObjectsLayout.CircleAndItsTangentLineFromPoints => new List<ConfigurationObjectType> { Circle, Point, Point },

                // 4 points
                LooseObjectsLayout.FourPoints => new List<ConfigurationObjectType> { Point, Point, Point, Point },

                // 4 points
                LooseObjectsLayout.FourConcyclicPoints => new List<ConfigurationObjectType> { Point, Point, Point, Point },

                // 1 line, 1 point
                LooseObjectsLayout.LineAndPoint => new List<ConfigurationObjectType> { Line, Point },

                // 1 line, 2 points
                LooseObjectsLayout.LineAndTwoPoints => new List<ConfigurationObjectType> { Line, Point, Point },

                // 2 points
                LooseObjectsLayout.TwoPoints => new List<ConfigurationObjectType> { Point, Point },

                // 3 points
                LooseObjectsLayout.IsoscelesTriangle => new List<ConfigurationObjectType> { Point, Point, Point },

                // Default case
                _ => throw new GeoGenException($"The layout '{layout}' doesn't have the object types defined."),
            };

    }
}