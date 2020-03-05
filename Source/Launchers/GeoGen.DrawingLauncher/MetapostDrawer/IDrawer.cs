using GeoGen.TheoremRanker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents a drawer of <see cref="RankedTheorem"/>s. 
    /// </summary>
    public interface IDrawer
    {
        /// <summary>
        /// Draws given <see cref="RankedTheorem"/>s.
        /// </summary>
        /// <returns>The task representing the result.</returns>
        Task DrawAsync(IEnumerable<RankedTheorem> rankedTheorems, int startingId);
    }
}
