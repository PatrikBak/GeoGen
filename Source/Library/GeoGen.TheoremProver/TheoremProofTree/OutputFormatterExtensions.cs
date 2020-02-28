using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.TheoremProver.InferenceRuleType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Extension methods for <see cref="OutputFormatter"/>.
    /// </summary>
    public static class OutputFormatterExtensions
    {
        /// <summary>
        /// Creates a formatted string describing a given theorem proof.
        /// </summary>
        /// <param name="formatter">The output formatter.</param>
        /// <param name="proof">The theorem proof to be formatted.</param>
        /// <returns>The string representing the theorem proof.</returns>
        public static string FormatTheoremProof(this OutputFormatter formatter, TheoremProof proof)
        {
            // Prepare a dictionary of theorem tags that are used to avoid repetition in the proof tree
            var theoremTags = new Dictionary<Theorem, string>();

            // Local function that recursively converts a given proof to a string, starting the 
            // explanation with a given tag
            string FormatTheoremProof(TheoremProof proof, string tag)
            {
                // Start composing the result by formatting the theorem and the proof explanation
                var result = $"{formatter.FormatTheorem(proof.Theorem)} - {GetProofExplanation(proof, formatter)}";

                // If there is nothing left to write, we're done
                if (proof.ProvedAssumptions.Count == 0)
                    return result;

                // Otherwise we want an empty line
                result += "\n\n";

                // Create an enumerable of reports of the proven assumptions
                var assumptionsString = proof.ProvedAssumptions
                    // Ordered by theorem
                    .OrderBy(assumptionProof => formatter.FormatTheorem(assumptionProof.Theorem))
                    // Process a given one
                    .Select((assumptionProof, index) =>
                    {
                        // Get the theorem for comfort
                        var theorem = assumptionProof.Theorem;

                        // Construct the new tag from the previous one and the ordinal number of the assumption
                        var newTag = $"  {tag}{index + 1}.";

                        // Find out if the theorem already has a tag
                        var theoremIsUntagged = !theoremTags.ContainsKey(theorem);

                        // If the theorem is not tagged yet, tag it
                        if (theoremIsUntagged)
                            theoremTags.Add(theorem, newTag.Trim());

                        // Find out the reasoning for the theorem
                        var reasoning = theoremIsUntagged ?
                                // If it's untagged, recursively find the proof string for it
                                FormatTheoremProof(assumptionProof, newTag) :
                                // Otherwise just state it again and add the explanation and the reference to it
                                $"{formatter.FormatTheorem(theorem)} - {GetProofExplanation(assumptionProof, formatter)} - theorem {theoremTags[theorem]}";

                        // The final result is the new tag (even if it's tagged already) and the reasoning
                        return $"{newTag} {reasoning}";
                    });

                // Append the particular assumptions, each on a separate line, while making 
                // sure there is exactly one line break at the end
                return $"{result}{assumptionsString.ToJoinedString("\n").TrimEnd()}\n";
            }

            // Call the local recursive function with no previous tag
            return FormatTheoremProof(proof, tag: "");
        }

        /// <summary>
        /// Returns a string representation the explanation of how a theorem has been proved.
        /// </summary>
        /// <param name="proof">The theorem proof to be examined.</param>
        /// <param name="formatter">The formatter of the configuration where the proved theorem holds.</param>
        /// <returns>The explanation string.</returns>
        private static string GetProofExplanation(TheoremProof proof, OutputFormatter formatter)
            // Switch based on the type of the used inference rule
            => proof.Rule switch
            {
                // Case when it's a theorem from a previous configuration
                TrueInPreviousConfiguration => "true in a previous configuration",

                // Case when it's a trivial consequence of the object's construction
                TrivialTheorem => "trivial consequence of the object's construction",

                // Case when the theorem has been inferred as a reformulation of another theorem using equalities
                ReformulatedTheorem => "reformulation of a theorem using equalities",

                // Case when it's been inferred as a consequence of a custom inference rule
                CustomRule => $"consequence of {((CustomInferenceData)proof.Data).Rule}",

                // Case when the transitivity rule has been applied
                EqualityTransitivity => "true because of the transitivity of equality",

                // Case when we're using symmetry
                InferableFromSymmetry => "true because of the symmetry of the configuration",

                // Case when the theorem can be stated without all objects
                DefinableSimpler => $"can be stated without " +
                    // State the redundant objects with their names
                    $"{((DefinableSimplerInferenceData)proof.Data).RedundantObjects.Select(formatter.GetObjectName).ToJoinedString()}",

                // Unhandled cases
                _ => throw new TheoremProverException($"Unhandled value of {nameof(InferenceRuleType)}: {proof.Rule}"),
            };
    }
}