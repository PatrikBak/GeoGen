using GeoGen.TheoremProver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="ObjectIntroductionRule"/>s.
    /// </summary>
    public interface IObjectIntroductionRuleProvider
    {
        /// <summary>
        /// Gets object introduction rules.
        /// </summary>
        /// <returns>The loaded object introduction rules.</returns>
        Task<IReadOnlyList<ObjectIntroductionRule>> GetObjectIntroductionRulesAsync();
    }
}