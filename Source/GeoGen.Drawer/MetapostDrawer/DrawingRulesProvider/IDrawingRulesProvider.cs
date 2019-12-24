using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a service that gets <see cref="DrawingRule"/>s.
    /// </summary>
    public interface IDrawingRulesProvider
    {
        /// <summary>
        /// Gets drawing rules.
        /// </summary>
        /// <returns>The drawing rules.</returns>
        Task<IReadOnlyList<DrawingRule>> GetDrawingRulesAsync();
    }
}
