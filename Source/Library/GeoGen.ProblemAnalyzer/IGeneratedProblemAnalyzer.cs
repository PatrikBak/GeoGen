using GeoGen.ProblemGenerator;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// Represents a service that takes a <see cref="ProblemGeneratorOutput"/> and performs additional GeoGen algorithms
    /// to find <see cref="GeneratedProblemAnalyzerOutputBase.InterestingTheorems"/>. This is done by combining theorem 
    /// proving, ranking, and optionally symmetry detections (specified via parameters of the analyze methods). It 
    /// can be done in two modes: With proof construction (which might be useful for debugging the prover) or without it. 
    /// All intermediate results are included in output objects of types <see cref="GeneratedProblemAnalyzerOutputWithProofs"/> 
    /// and <see cref="GeneratedProblemAnalyzerOutputWithoutProofs"/>.
    /// </summary>
    public interface IGeneratedProblemAnalyzer
    {
        /// <summary>
        /// Performs the analysis of a given generator output with explicit construction of theorem proofs.
        /// </summary>
        /// <param name="generatorOutput">The generator output to be analyzed.</param>
        /// <param name="mode">Indicates how we handle asymmetric problems with regards to generation.</param>
        /// <returns>The result of the analysis with constructed theorem proofs.</returns>
        GeneratedProblemAnalyzerOutputWithProofs AnalyzeWithProofConstruction(ProblemGeneratorOutput generatorOutput, SymmetryGenerationMode mode);

        /// <summary>
        /// Performs the analysis of a given generator output without explicit construction of theorem proofs.
        /// </summary>
        /// <param name="generatorOutput">The generator output to be analyzed.</param>
        /// <param name="mode">Indicates how we handle asymmetric problems with regards to generation.</param>
        /// <returns>The result of the analysis without constructed theorem proofs.</returns>
        GeneratedProblemAnalyzerOutputWithoutProofs AnalyzeWithoutProofConstruction(ProblemGeneratorOutput generatorOutput, SymmetryGenerationMode mode);
    }
}