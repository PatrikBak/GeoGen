namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of a theorem inference. By default it includes only the <see cref="InferenceRuleType"/>.
    /// </summary>
    public class TheoremInferenceData
    {
        #region Public properties

        /// <summary>
        /// The type of the used inference rule.
        /// </summary>
        public InferenceRuleType RuleType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the <see cref="TheoremInferenceData"/> class.
        /// </summary>
        /// <param name="ruleType">The type of the used inference rule.</param>
        public TheoremInferenceData(InferenceRuleType ruleType)
        {
            RuleType = ruleType;
        }

        #endregion
    }
}