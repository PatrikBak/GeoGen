using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a type of reason based on which a theorem has been established true.
    /// </summary>
    public enum InferenceRuleType
    {
        /// <summary>
        /// The theorem is true in the configuration that does not contain the last object
        /// of the examined configuration, i.e. the theorem's proof is not interesting at this point.
        /// </summary>
        TrueInPreviousConfiguration,

        /// <summary>
        /// The theorem can be stated in a configuration that does not contain every object
        /// of the current configuration. 
        /// <para>
        /// NOTE: The difference between this rule and <see cref="TrueInPreviousConfiguration"/> 
        ///       is that in this case the smaller configuration might not have been examined or
        ///       explicitly defined.
        /// </para>
        /// </summary>
        DefinableSimpler,

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