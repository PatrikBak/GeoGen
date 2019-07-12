using GeoGen.Core;

namespace GeoGen.Theorems
{
    /// <summary>
    /// Represents a service that is able to tell if a given <see cref="Theorem"/> is a subsequence of another given <see cref="Theorem"/>.
    /// </summary>
    public interface ISubtheoremAnalyzer
    {
        /// <summary>
        /// Performs the sub-theorem analysis on a given input.
        /// </summary>
        /// <returns>The result of the sub-theorem analysis.</returns>
        SubtheoremAnalyzerOutput Analyze(SubtheoremAnalyzerInput input);
    }
}