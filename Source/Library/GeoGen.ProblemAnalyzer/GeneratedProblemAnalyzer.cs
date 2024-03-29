﻿using GeoGen.Core;
using GeoGen.ProblemGenerator;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;

namespace GeoGen.ProblemAnalyzer
{
    /// <summary>
    /// The default implementation of <see cref="IGeneratedProblemAnalyzer"/> that combines <see cref="ITheoremProver"/>
    /// and <see cref="ITheoremRanker"/> to perform the following steps:
    /// the following:
    /// <list type="number">
    /// <item>Theorems are attempted to be proved. Proved ones are automatically not interesting.</item>
    /// <item>If we are supposed to exclude asymmetric theorems, then we find them and mark not interesting.</item>
    /// <item>The remaining theorems are ranked and sorted by the ranking ascending. These are the final interesting theorems.</item>
    /// </list>
    /// </summary>
    public class GeneratedProblemAnalyzer : IGeneratedProblemAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The prover of theorems.
        /// </summary>
        private readonly ITheoremProver _prover;

        /// <summary>
        /// The ranker of theorems.
        /// </summary>
        private readonly ITheoremRanker _ranker;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedProblemAnalyzer"/> class.
        /// </summary>
        /// <param name="prover">The prover of theorems.</param>
        /// <param name="ranker">The ranker of theorems.</param>
        public GeneratedProblemAnalyzer(ITheoremProver prover, ITheoremRanker ranker)
        {
            _prover = prover ?? throw new ArgumentNullException(nameof(prover));
            _ranker = ranker ?? throw new ArgumentNullException(nameof(ranker));
        }

        #endregion

        #region IGeneratedProblemAnalyzer implementation

        /// <inheritdoc/>
        public GeneratedProblemAnalyzerOutputWithProofs AnalyzeWithProofConstruction(ProblemGeneratorOutput generatorOutput, SymmetryGenerationMode mode)
            // Delegate the call to the general method
            => Analyze(generatorOutput, mode, constructProofs: true);

        /// <inheritdoc/>
        public GeneratedProblemAnalyzerOutputWithoutProofs AnalyzeWithoutProofConstruction(ProblemGeneratorOutput generatorOutput, SymmetryGenerationMode mode)
            // Delegate the call to the general method
            => Analyze(generatorOutput, mode, constructProofs: false);

        /// <summary>
        /// Performs the analysis of a given generator output.
        /// </summary>
        /// <param name="output">The generator output to be analyzed.</param>
        /// <param name="mode">Indicates how we handle asymmetric problems with regards to generation.</param>
        /// <param name="constructProofs">Indicates whether we should construct proofs or not, which affects the type of result.</param>
        /// <returns>The result depending on whether we're constructing proofs or not.</returns>
        private dynamic Analyze(ProblemGeneratorOutput output, SymmetryGenerationMode mode, bool constructProofs)
        {
            // Call the prover
            var proverOutput = constructProofs
                // If we should construct proofs, do so
                ? (object)_prover.ProveTheoremsAndConstructProofs(output.OldTheorems, output.NewTheorems, output.ContextualPicture)
                // If we shouldn't construct proofs, don't do it
                : _prover.ProveTheorems(output.OldTheorems, output.NewTheorems, output.ContextualPicture);

            // Find the proved theorems 
            var provedTheorems = constructProofs
                // If we have constructed proofs, there is a dictionary
                ? (IReadOnlyCollection<Theorem>)((IReadOnlyDictionary<Theorem, TheoremProof>)proverOutput).Keys
                // Otherwise there is a collection directly
                : (IReadOnlyCollection<Theorem>)proverOutput;

            // Get the unproven theorems by taking all the new theorems
            var interestingTheorems = output.NewTheorems.AllObjects
                // Excluding those that are proven
                .Where(theorem => !provedTheorems.Contains(theorem))
                // Enumerate
                .ToArray();

            // Find the problems that should excluded based on symmetry
            var notInterestingTheorems = mode switch
            {
                // No restrictions
                SymmetryGenerationMode.GenerateBothSymmetricAndAsymmetric => (IReadOnlyList<Theorem>)Array.Empty<Theorem>(),

                // Detect symmetric theorems
                SymmetryGenerationMode.GenerateOnlySymmetric => interestingTheorems.Where(theorem => !theorem.IsSymmetric(output.Configuration)).ToArray(),

                // Detect fully symmetric theorems
                SymmetryGenerationMode.GenerateOnlyFullySymmetric => interestingTheorems.Where(theorem => !theorem.IsFullySymmetric(output.Configuration)).ToArray(),

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled value of {nameof(SymmetryGenerationMode)}: {mode}"),
            };

            // Interesting theorems can now be reseted 
            interestingTheorems = interestingTheorems
                // By the exclusion of not interesting asymmetric ones
                .Except(notInterestingTheorems)
                // Enumerate
                .ToArray();

            // Prepare the map of all theorems
            var allTheorems = new TheoremMap(output.OldTheorems.AllObjects.Concat(output.NewTheorems.AllObjects));

            // Rank the interesting theorems
            var rankedInterestingTheorems = interestingTheorems
                // Rank given one
                .Select(theorem => _ranker.Rank(theorem, output.Configuration, allTheorems))
                 // By rankings ASC (that's why -)
                 .OrderBy(rankedTheorem => -rankedTheorem.Ranking.TotalRanking)
                 // Enumerate
                 .ToArray();

            // Now we can finally return the result
            return constructProofs
                // If we're constructing proofs, then we have a proof dictionary
                ? new GeneratedProblemAnalyzerOutputWithProofs(rankedInterestingTheorems, notInterestingTheorems, (IReadOnlyDictionary<Theorem, TheoremProof>)proverOutput)
                // If we're not constructing proofs, then we have just a proved theorem collection
                : (GeneratedProblemAnalyzerOutputBase)new GeneratedProblemAnalyzerOutputWithoutProofs(rankedInterestingTheorems, notInterestingTheorems, (IReadOnlyCollection<Theorem>)proverOutput);
        }

        #endregion
    }
}