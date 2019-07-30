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
        public static IReadOnlyList<ConfigurationObjectType> ObjectTypes(this LooseObjectsLayout layout)
        {
            switch (layout)
            {
                // With no layout we cannot specify the types
                case LooseObjectsLayout.NoLayout:
                    throw new GeoGenException("Objects with no layout don't have predefined types.");

                // 3 points
                case LooseObjectsLayout.ThreePoints:
                    return new List<ConfigurationObjectType> { Point, Point, Point };

                // 6 points
                case LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints:
                    return new List<ConfigurationObjectType> { Point, Point, Point, Point, Point, Point };

                // 4 points
                case LooseObjectsLayout.Trapezoid:
                    return new List<ConfigurationObjectType> { Point, Point, Point, Point };

                // 1 circle, 2 points
                case LooseObjectsLayout.CircleAndItsTangentLineFromPoints:
                    return new List<ConfigurationObjectType> { Circle, Point, Point };

                // 4 points
                case LooseObjectsLayout.FourPoints:
                    return new List<ConfigurationObjectType> { Point, Point, Point, Point };

                // 1 line, 1 point
                case LooseObjectsLayout.LineAndPoint:
                    return new List<ConfigurationObjectType> { Line, Point };

                // 1 line, 2 points
                case LooseObjectsLayout.LineAndTwoPoints:
                    return new List<ConfigurationObjectType> { Line, Point, Point };

                // 2 points
                case LooseObjectsLayout.TwoPoints:
                    return new List<ConfigurationObjectType> { Point, Point };

                default:
                    throw new GeoGenException($"The layout '{layout}' doesn't have the object types defined.");
            }
        }
    }
}