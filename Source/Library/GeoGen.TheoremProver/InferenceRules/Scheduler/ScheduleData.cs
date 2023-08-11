using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents data provided by a <see cref="Scheduler"/> that are later used to create 
    /// an <see cref="InferenceRuleApplierInput"/>.
    /// </summary>
    public class ScheduleData
    {
        #region Public properties

        /// <summary>
        /// The inference rule that is to be applied.
        /// </summary>
        public InferenceRule InferenceRule { get; }

        /// <summary>
        /// The premapped declared object of <see cref="InferenceRule.DeclaredObjects"/>. This value can be null if no object premapping is wanted.
        /// </summary>
        public (ConstructedConfigurationObject declaredTemplateObject, ConstructedConfigurationObject realObject)? PremappedObject { get; }

        /// <summary>
        /// The premapped assumption of <see cref="InferenceRule.AssumptionGroups"/>. This value can be null if no assumption premapping is wanted.
        /// </summary>
        public (Theorem templateAssumption, Theorem realAssumption)? PremappedAssumption { get; }

        /// <summary>
        /// The conclusion that should be inferred. This value can be null if no conclusion premapping is wanted.
        /// </summary>
        public Theorem PremappedConclusion { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleData"/> class.
        /// </summary>
        /// <param name="inferenceRule">The inference rule that is to be applied.</param>
        /// <param name="premappedObject">The premapped declared object of <see cref="InferenceRule.DeclaredObjects"/>. This value can be null if no object premapping is wanted.</param>
        /// <param name="premappedAssumption">The premapped assumption of <see cref="InferenceRule.AssumptionGroups"/>. This value can be null if no assumption premapping is wanted.</param>
        /// <param name="premappedConclusion">The conclusion that should be inferred. This value can be null if no conclusion premapping is wanted.</param>
        public ScheduleData(InferenceRule inferenceRule,
                            (ConstructedConfigurationObject declaredTemplateObject, ConstructedConfigurationObject realObject)? premappedObject = null,
                            (Theorem templateAssumption, Theorem realAssumption)? premappedAssumption = null,
                            Theorem premappedConclusion = null)
        {
            InferenceRule = inferenceRule ?? throw new ArgumentNullException(nameof(inferenceRule));
            PremappedObject = premappedObject;
            PremappedAssumption = premappedAssumption;
            PremappedConclusion = premappedConclusion;
        }

        #endregion
    }
}