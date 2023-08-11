using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an input for <see cref="IInferenceRuleApplier"/>. All needed objects and assumptions for mapping are 
    /// retrieved from <see cref="MappableObjectsFactory"/> and <see cref="MappableTheoremsFactory"/>.
    /// <para>
    /// It contains options to narrow the number and type of results by allowing pre-mapping of <see cref="InferenceRule.DeclaredObjects"/> (<see cref="PremappedObject"/>), 
    /// assumptions from <see cref="InferenceRule.AssumptionGroups"/> (<see cref="PremappedAssumption"/>), and also
    /// <see cref="InferenceRule.Conclusion"/> (<see cref="PremappedConclusion"/>).
    /// </para>
    /// <para>
    /// There is also an <see cref="EqualObjectsFactory"/>, which has the following use: When there is a declared template
    /// object that internally contains another constructed template, for example Circumcenter(Incenter(A, B, C), B, C),
    /// and we do a premapping of this to a Circumcenter(X, Y, Z), then in order to be able to do this premapping we need
    /// to be able to find incenters equal to X (or Y, Z, because the order of X, Y, Z is not important).
    /// </para>
    /// <para>
    /// The applier uses also the concept of normalization, which is the reason why it requires <see cref="NormalizationFunction"/>.
    /// For more information see the documentation of <see cref="NormalizationHelper"/>.
    /// </para>
    /// </summary>
    public class InferenceRuleApplierInput
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

        /// <summary>
        /// The factory that for a theorem type retrieves all mappable theorems of this type.
        /// </summary>
        public Func<TheoremType, IEnumerable<Theorem>> MappableTheoremsFactory { get; }

        /// <summary>
        /// The factory that for a construction retrieves all mappable constructed objects with this construction. 
        /// </summary>
        public Func<Construction, IEnumerable<ConstructedConfigurationObject>> MappableObjectsFactory { get; }

        /// <summary>
        /// The factory that for an object and construction retrieves all mappable constructed objects
        /// equal to the given object with this construction.
        /// </summary>
        public Func<ConfigurationObject, Construction, IEnumerable<ConstructedConfigurationObject>> EqualObjectsFactory { get; }

        /// <summary>
        /// The function that finds the normal version of a passed object. (see <see cref="NormalizationHelper"/>).
        /// </summary>
        public Func<ConfigurationObject, ConfigurationObject> NormalizationFunction { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleApplierInput"/> class.
        /// </summary>
        /// <param name="inferenceRule">The inference rule that is to be applied.</param>
        /// <param name="premappedObject">The premapped declared object of <see cref="InferenceRule.DeclaredObjects"/>. This value can be null if no object premapping is wanted.</param>
        /// <param name="premappedAssumption">The premapped assumption of <see cref="InferenceRule.AssumptionGroups"/>. This value can be null if no assumption premapping is wanted.</param>
        /// <param name="premappedConclusion">The conclusion that should be inferred. This value can be null if no conclusion premapping is wanted.</param>
        /// <param name="mappableTheoremsFactory">The factory that for a theorem type retrieves all mappable theorems of this type.</param>
        /// <param name="mappableObjectsFactory">The factory that for a construction retrieves all mappable constructed objects with this construction. </param>
        /// <param name="equalObjectsFactory">The factory that for an object and construction retrieves all mappable constructed objects equal to the given object with this construction.</param>
        /// <param name="normalizationFunction">The function that finds the normal version of a passed object. (see <see cref="NormalizationHelper"/>).</param>
        public InferenceRuleApplierInput(InferenceRule inferenceRule,
                                         (ConstructedConfigurationObject declaredTemplateObject, ConstructedConfigurationObject realObject)? premappedObject,
                                         (Theorem templateAssumption, Theorem realAssumption)? premappedAssumption,
                                         Theorem premappedConclusion,
                                         Func<TheoremType, IEnumerable<Theorem>> mappableTheoremsFactory,
                                         Func<Construction, IEnumerable<ConstructedConfigurationObject>> mappableObjectsFactory,
                                         Func<ConfigurationObject, Construction, IEnumerable<ConstructedConfigurationObject>> equalObjectsFactory,
                                         Func<ConfigurationObject, ConfigurationObject> normalizationFunction)
        {
            InferenceRule = inferenceRule ?? throw new ArgumentNullException(nameof(inferenceRule));
            PremappedObject = premappedObject;
            PremappedAssumption = premappedAssumption;
            PremappedConclusion = premappedConclusion;
            MappableTheoremsFactory = mappableTheoremsFactory ?? throw new ArgumentNullException(nameof(mappableTheoremsFactory));
            MappableObjectsFactory = mappableObjectsFactory ?? throw new ArgumentNullException(nameof(mappableObjectsFactory));
            EqualObjectsFactory = equalObjectsFactory ?? throw new ArgumentNullException(nameof(equalObjectsFactory));
            NormalizationFunction = normalizationFunction ?? throw new ArgumentNullException(nameof(normalizationFunction));
        }

        #endregion
    }
}