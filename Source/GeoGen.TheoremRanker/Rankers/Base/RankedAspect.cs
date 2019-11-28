using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a thing that can be ranked on theorems. The higher the rank the more interesting a theorem is.
    /// </summary>
    public enum RankedAspect
    {
        /// <summary>
        /// The coefficient of theorem or configuration symmetry. This coefficient is calculated like this. 
        /// Assume there are 'n' possible reordering of loose objects of the configuration (for example, for triangle,
        /// there are 3!-1 = 5 of them). Now assume that 'm' is the number of such reordering which after applying to the 
        /// pair (configuration, theorem) yield the same result. Then the level of symmetry is m/n. 
        /// 
        /// For example: 
        /// 
        /// 1. Triangle ABC with D = PointReflection(A, B) and theorem AB = BD has m = 0, n = 5, the level of symmetry is 0.
        /// 2. Triangle ABC with D = Midpoint(A, B) and theorem DA = DB has m = 1, n = 5, the level of symmetry is 1/5 = 20%
        /// 3. Triangle ABC with midpoints of sides and concurrent medians has m = 5, n = 5, the level of symmetry is 100%.
        /// 
        /// </summary>
        Symmetry,

        /// <summary>
        /// The coefficient taking into account <see cref="TheoremType"/>. This is done solely based on 
        /// <see cref="TypeRankerSettings.TypeRankings"/>.
        /// </summary>
        Type,

        /// <summary>
        /// The ratio of the number of objects and theorems of the configuration. The idea behind this metrics is 
        /// that if we have more theorems in a configuration, then it usually suggests the problem is not that difficult,
        /// because we can make lots of conclusions. 
        /// </summary>
        ObjectsPerTheorem
    }
}