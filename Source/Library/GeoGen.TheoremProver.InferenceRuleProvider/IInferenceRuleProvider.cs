using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.TheoremProver.InferenceRuleProvider
{
    /// <summary>
    /// Represents a service that gets <see cref="LoadedInferenceRule"/>s.
    /// </summary>
    public interface IInferenceRuleProvider
    {
        /// <summary>
        /// Gets inference rules.
        /// </summary>
        /// <returns>The loaded inference rules.</returns>
        Task<IReadOnlyList<LoadedInferenceRule>> GetInferenceRulesAsync();
    }
}