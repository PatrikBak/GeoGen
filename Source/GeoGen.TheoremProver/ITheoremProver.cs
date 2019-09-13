namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to prove theorems. It tries to prove the theorems from
    /// <see cref="TheoremProverInput.NewTheorems"/>. The overall idea is that if it can
    /// prove a theorem, then it means this theorem is not olympiad-enough (those should be difficult).
    /// The purpose of proving is solely for finding out whether the theorem is easy.
    /// </summary>
    public interface ITheoremProver
    {
        /// <summary>
        /// Performs the analysis for a given input.
        /// </summary>
        /// <param name="input">The input for the analyzer.</param>
        /// <returns>The output of the analysis.</returns>
        TheoremProverOutput Analyze(TheoremProverInput input);
    }
}
