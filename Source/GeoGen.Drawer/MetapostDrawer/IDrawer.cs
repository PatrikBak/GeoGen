using GeoGen.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a drawer of configurations and theorems.
    /// </summary>
    public interface IDrawer
    {
        /// <summary>
        /// Draws given configurations with its theorems.
        /// </summary>
        /// <param name="configurationWithTheorems">The configurations with theorems to be drawn.</param>
        /// <returns>The task representing the result.</returns>
        Task DrawAsync(IEnumerable<(Configuration configuration, Theorem theorem)> configurationWithTheorems);
    }
}
