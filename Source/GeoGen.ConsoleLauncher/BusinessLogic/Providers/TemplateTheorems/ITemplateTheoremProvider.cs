using GeoGen.Core;
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
        /// <returns>The list of loaded configuration with its theorems.</returns>
        Task<IReadOnlyList<(Configuration, TheoremMap)>> GetTemplateTheoremsAsync();
    }
}
