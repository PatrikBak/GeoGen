namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The settings related to the theorem prover module.
    /// </summary>
    public class TheoremProvingSettings
    {
        #region Public properties

        /// <summary>
        /// The data for <see cref="InferenceRuleManager"/>.
        /// </summary>
        public InferenceRuleManagerData InferenceRuleManagerData { get; }

        /// <summary>
        /// The data for <see cref="ObjectIntroducer"/>.
        /// </summary>
        public ObjectIntroducerData ObjectIntroducerData { get; }

        /// <summary>
        /// The settings for <see cref="TheoremProver"/>.
        /// </summary>
        public TheoremProverSettings TheoremProverSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProvingSettings"/> class.
        /// </summary>
        /// <param name="inferenceRuleManagerData">The data for <see cref="InferenceRuleManager"/>.</param>
        /// <param name="objectIntroducerData">The data for <see cref="ObjectIntroducer"/>.</param>
        /// <param name="theoremProverSettings">The settings for <see cref="TheoremProver"/>.</param>
        public TheoremProvingSettings(InferenceRuleManagerData inferenceRuleManagerData, ObjectIntroducerData objectIntroducerData, TheoremProverSettings theoremProverSettings)
        {
            InferenceRuleManagerData = inferenceRuleManagerData ?? throw new ArgumentNullException(nameof(inferenceRuleManagerData));
            ObjectIntroducerData = objectIntroducerData ?? throw new ArgumentNullException(nameof(objectIntroducerData));
            TheoremProverSettings = theoremProverSettings ?? throw new ArgumentNullException(nameof(theoremProverSettings));
        }

        #endregion
    }
}