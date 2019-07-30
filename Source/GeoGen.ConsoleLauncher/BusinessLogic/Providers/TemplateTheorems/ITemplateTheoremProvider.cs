using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="TemplateTheorem"/>s for the algorithm.
    /// </summary>
    public interface ITemplateTheoremProvider
    {
        /// <summary>
        /// Gets template theorems.
        /// </summary>
        /// <returns>The template theorems.</returns>
        Task<List<TemplateTheorem>> GetTemplateTheoremsAsync();
    }
}
