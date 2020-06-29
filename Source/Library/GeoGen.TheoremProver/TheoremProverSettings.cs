namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The settings for <see cref="TheoremProver"/>.
    /// </summary>
    public class TheoremProverSettings
    {
        #region Public properties

        /// <summary>
        /// Indicates whether the prover automatically assumes that simplifiable theorems 
        /// (<see cref="InferenceRuleType.DefinableSimpler"/>) are true.
        /// </summary>
        public bool AssumeThatSimplifiableTheoremsAreTrue { get; }

        /// <summary>
        /// Indicates whether <see cref="ITrivialTheoremProducer"/> should be called
        /// only on the last object of the configuration. The purpose of this is that
        /// in a standard scenario, the trivial theorems of others objects are automatically
        /// assumed true, so there is no need to find them again.
        /// </summary>
        public bool FindTrivialTheoremsOnlyForLastObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverSettings"/> class.
        /// </summary>
        /// <param name="assumeThatSimplifiableTheoremsAreTrue"><inheritdoc cref="AssumeThatSimplifiableTheoremsAreTrue" path="/summary"/></param>
        /// <param name="findTrivialTheoremsOnlyForLastObject"><inheritdoc cref="FindTrivialTheoremsOnlyForLastObject" path="/summary"/></param>
        public TheoremProverSettings(bool assumeThatSimplifiableTheoremsAreTrue, bool findTrivialTheoremsOnlyForLastObject)
        {
            AssumeThatSimplifiableTheoremsAreTrue = assumeThatSimplifiableTheoremsAreTrue;
            FindTrivialTheoremsOnlyForLastObject = findTrivialTheoremsOnlyForLastObject;
        }

        #endregion
    }
}