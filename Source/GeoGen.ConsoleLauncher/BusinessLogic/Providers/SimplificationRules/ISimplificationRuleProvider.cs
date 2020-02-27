using GeoGen.TheoremSimplifier;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="SimplificationRule"/>s.
    /// </summary>
    public interface ISimplificationRuleProvider
    {
        /// <summary>
        /// Gets simplification rules.
        /// </summary>
        /// <returns>The loaded simplification rules.</returns>
        Task<IReadOnlyList<SimplificationRule>> GetSimplificationRulesAsync();
    }
}
