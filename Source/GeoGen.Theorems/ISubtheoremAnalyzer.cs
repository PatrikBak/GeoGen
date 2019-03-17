using GeoGen.Core;

namespace GeoGen.Theorems
{
    /// <summary>
    /// Represents a service that is able to tell if a given <see cref="Theorem"/> is a subsequence of another given <see cref="Theorem"/>.
    /// </summary>
    public interface ISubtheoremAnalyzer
    {
        /// <summary>
        /// Finds out of a given original theorem is a consequence of another given theorem.
        /// </summary>
        /// <param name="originalTheorem">The original theorem that serves like a template.</param>
        /// <param name="potentialConsequence">The theorem that might be a consequence of the original theorem.</param>
        /// <returns>The result of the sub-theorem analysis.</returns>
        SubtheoremData Analyze(Theorem originalTheorem, Theorem potentialConsequence);
    }
}