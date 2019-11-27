using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a thing that can be ranked on theorems
    /// </summary>
    public enum RankedAspect
    {
        /// <summary>
        /// The coefficient of theorem or configuration symmetry.
        /// </summary>
        Symmetry,

        /// <summary>
        /// The coefficient taking into account <see cref="TheoremType"/>.
        /// </summary>
        Type
    }
}