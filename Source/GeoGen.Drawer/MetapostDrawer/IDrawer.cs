using GeoGen.ConsoleLauncher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a drawer of <see cref="TheoremWithRanking"/>s. 
    /// </summary>
    public interface IDrawer
    {
        /// <summary>
        /// Draws given <see cref="TheoremWithRanking"/>s.
        /// </summary>
        /// <returns>The task representing the result.</returns>
        Task DrawAsync(IEnumerable<TheoremWithRanking> rankedTheorems, int startingId);
    }
}
