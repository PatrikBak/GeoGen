using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a type of reason based on which a theorem has been established true.
    /// </summary>
    public enum InferenceRuleType
    {
        /// <summary>
        /// The theorem is assumed to be true.
        /// </summary>
        AssumedProven,

        /// <summary>
        /// The theorem can be stated in a configuration that does not contain every object
        /// of the current configuration. 
        /// </summary>
        DefinableSimpler,

        /// <summary>
        /// The theorem can be inferable from the symmetry of the configuration where it holds. For example,
        /// if we have Triangle ABC with midpoints P, Q, R of its sides BC, CA, AB, then knowing BC || QR
        /// means we can infer CA || RP and AB || PQ just from the symmetry.
        /// </summary>
        InferableFromSymmetry,

        /// <summary>
        /// The theorem that can be inferred directly from the construction of an object.
        /// </summary>
        TrivialTheorem,

        /// <summary>
        /// The theorem can be reformulated to another theorem using equalities.
        /// </summary>
        ReformulatedTheorem,

        /// <summary>
        /// The theorem is true because a custom <see cref="InferenceRule"/> has been used.
        /// </summary>
        CustomRule,

        /// <summary>
        /// The theorem with type <see cref="TheoremType.EqualObjects"/> can be inferred from the transitivity rule
        /// (a = b AND b = c IMPLIES a = c).
        /// </summary>
        EqualityTransitivity
    }
}