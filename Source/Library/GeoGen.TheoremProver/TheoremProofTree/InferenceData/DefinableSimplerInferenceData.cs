using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of an inference that uses <see cref="InferenceRuleType.DefinableSimpler"/>.
    /// </summary>
    public class DefinableSimplerInferenceData : TheoremInferenceData
    {
        #region Public properties

        /// <summary>
        /// The objects that are not needed to state the theorem.
        /// </summary>
        public IReadOnlyCollection<ConfigurationObject> RedundantObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinableSimplerInferenceData"/> class.
        /// </summary>
        /// <param name="redundantObjects">The objects that are not needed to state the theorem.</param>
        public DefinableSimplerInferenceData(IReadOnlyCollection<ConfigurationObject> redundantObjects)
            : base(InferenceRuleType.DefinableSimpler)
        {
            RedundantObjects = redundantObjects ?? throw new ArgumentNullException(nameof(redundantObjects));
        }

        #endregion
    }
}