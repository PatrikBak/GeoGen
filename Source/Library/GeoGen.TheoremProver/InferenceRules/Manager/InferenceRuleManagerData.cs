namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents data for <see cref="InferenceRuleManager"/> containing available <see cref="InferenceRule"/>s.
    /// </summary>
    public class InferenceRuleManagerData
    {
        #region Public properties

        /// <summary>
        /// The available <see cref="InferenceRule"/>s.
        /// </summary>
        public IReadOnlyCollection<InferenceRule> Rules { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleManager"/> class.
        /// </summary>
        /// <param name="rules">The available <see cref="InferenceRule"/>s.</param>
        public InferenceRuleManagerData(IReadOnlyCollection<InferenceRule> rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        #endregion
    }
}