using static GeoGen.Core.TheoremType;

namespace GeoGen.Core
{
    /// <summary>
    /// The extensions methods for <see cref="TheoremType"/>.
    /// </summary>
    public static class TheoremTypeExtensions
    {
        /// <summary>
        /// Gets the number of <see cref="TheoremObject"/> that are required for a theorem of this type.
        /// </summary>
        /// <param name="type">The theorem type.</param>
        /// <returns>The needed number of numbers.</returns>
        public static int GetNumberOfNeededObjects(this TheoremType type) => type switch
        {
            // 4 points are concyclic
            ConcyclicPoints => 4,

            // 3 points are collinear
            CollinearPoints => 3,

            // 3 lines are concurrent
            ConcurrentLines => 3,

            // 3 line/circles are concurrent
            ConcurrentObjects => 3,

            // 2 angles are equal
            EqualAngles => 2,

            // 2 line segments are equal
            EqualLineSegments => 2,

            // 2 objects are equal
            EqualObjects => 2,

            // 1 line is tangent to 1 circle
            LineTangentToCircle => 2,

            // 2 circles are tangent
            TangentCircles => 2,

            // 2 lines are parallel
            ParallelLines => 2,

            // 2 lines are perpendicular
            PerpendicularLines => 2,

            // 1 points lies on 1 line / circle
            Incidence => 2,

            // Unhandled case
            _ => throw new GeoGenException($"Unhandled type of theorem: {type}")
        };
    }
}
