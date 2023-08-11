namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of an inference that uses <see cref="InferenceRuleType.CustomRule"/>.
    /// </summary>
    public class CustomInferenceData : TheoremInferenceData
    {
        #region Public properties

        /// <summary>
        /// The used inference rule.
        /// </summary>
        public InferenceRule Rule { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomInferenceData"/> class.
        /// </summary>
        /// <param name="rule">The used inference rule.</param>
        public CustomInferenceData(InferenceRule rule)
            : base(InferenceRuleType.CustomRule)
        {
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        #endregion
    }
}