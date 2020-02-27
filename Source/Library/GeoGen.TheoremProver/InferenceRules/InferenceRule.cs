using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a rule that be used to infer a <see cref="Conclusion"/> from <see cref="AssumptionGroups"/>
    /// provided that <see cref="NegativeAssumptions"/> are not true.
    /// <para>
    /// Inner <see cref="ConstructedConfigurationObject"/> of the assumptions and conclusion should be
    /// among <see cref="DeclaredObjects"/> to speed up rule applying, though it is not compulsory. 
    /// </para>
    /// <para>
    /// <see cref="DeclaredObjects"/>, <see cref="AssumptionGroups"/> and <see cref="Conclusion"/> might
    /// contain <see cref="LooseConfigurationObject"/> that can be mapped to any object of the given type.
    /// </para>
    /// <para>
    /// Another way to speed up rule applying is to have assumptions grouped as in <see cref="AssumptionGroups"/>.
    /// For example, let's have a rule that says that if points P, Q, R lie on a line l, then P, Q, R are collinear. 
    /// The three incidence assumptions are interchangeable, therefore an <see cref="IInferenceRuleApplier"/> just 
    /// needs to try to pre-map one of these assumptions to actual true incidence theorems.
    /// </para> 
    /// </summary>
    public class InferenceRule
    {
        #region Public properties

        /// <summary>
        /// The objects that are referenced in the assumptions and the conclusion.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> DeclaredObjects { get; }

        /// <summary>
        /// The assumptions grouped in a way that assumptions in the same group are mutually isomorphic (see <see cref="InferenceRule"/>).
        /// </summary>
        public IReadOnlyHashSet<IReadOnlyHashSet<Theorem>> AssumptionGroups { get; }

        /// <summary>
        /// The assumptions that must be not true in order for inferred theorems to hold.
        /// </summary>
        public IReadOnlyList<Theorem> NegativeAssumptions { get; }

        /// <summary>
        /// The conclusion that can be inferred if the assumptions are met.
        /// </summary>
        public Theorem Conclusion { get; }

        /// <summary>
        /// The dictionary mapping <see cref="Construction"/>s to the number of objects that
        /// are used in the <see cref="DeclaredObjects"/>. This is used for quick discovery
        /// that this rule cannot be used simply because there aren't enough objects to 
        /// match declared objects.
        /// </summary>
        public IReadOnlyDictionary<Construction, int> NeededConstructionTypes { get; }

        /// <summary>
        /// The dictionary mapping <see cref="TheoremType"/>s to the number of assumptions that
        /// are used in the <see cref="AssumptionGroups"/>. This is used for quick discovery
        /// that this rule cannot be used simply because there aren't enough assumptions to 
        /// of needed types to be met.
        /// </summary>
        public IReadOnlyDictionary<TheoremType, int> NeededAssumptionTypes { get; }

        /// <summary>
        /// Represents a set of objects with points, i.e. <see cref="LineTheoremObject"/> or <see cref="CircleTheoremObject"/>, 
        /// that are <see cref="TheoremObjectWithPoints.DefinedByExplicitObject"/>, but are allowed to be used even as objects
        /// implicitly defined by points. It is convenient to allow this because we do not need to have rules in both explicit 
        /// and implicit version (for example a || b and b || c implies a || c, no matter whether lines a, b, c are defined 
        /// explicitly or implicitly by points. On the other hand, incidences don't have allowed implicit objects (that would 
        /// be equivalent to Collinearity or Concyclity). Therefore, an inner <see cref="TheoremObjectWithPoints"/> of
        /// <see cref="AssumptionGroups"/> is in this set if and only if it is not part of an incidence assumption or conclusion.
        /// </summary>
        public IReadOnlyHashSet<TheoremObjectWithPoints> ExplicitObjectsMappableToImplicitOnes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRule"/> class.
        /// </summary>
        /// <param name="declaredObjects">The objects that are referenced in the assumptions and the conclusion.</param>
        /// <param name="assumptionGroups">The assumptions grouped in a way that assumptions in the same group are mutually isomorphic (see <see cref="InferenceRule"/>).</param>
        /// <param name="negativeAssumptions">The assumptions that must be not true in order for inferred theorems to hold.</param>
        /// <param name="conclusion">The conclusion that can be inferred if the assumptions are met.</param>
        public InferenceRule(IReadOnlyList<ConstructedConfigurationObject> declaredObjects,
                             IReadOnlyHashSet<IReadOnlyHashSet<Theorem>> assumptionGroups,
                             IReadOnlyList<Theorem> negativeAssumptions,
                             Theorem conclusion)
        {
            DeclaredObjects = declaredObjects ?? throw new ArgumentNullException(nameof(declaredObjects));
            AssumptionGroups = assumptionGroups ?? throw new ArgumentNullException(nameof(assumptionGroups));
            NegativeAssumptions = negativeAssumptions ?? throw new ArgumentNullException(nameof(negativeAssumptions));
            Conclusion = conclusion ?? throw new ArgumentNullException(nameof(conclusion));

            // Find the number of needed construction by taking the declared objects
            NeededConstructionTypes = declaredObjects
                // Grouping them by their construction
                .GroupBy(template => template.Construction)
                // And counting the number of objects in each group
                .ToDictionary(group => group.Key, group => group.Count());

            // Find the number of needed assumption types by taking flattened assumptions
            NeededAssumptionTypes = assumptionGroups.Flatten()
                // Grouping them by their type
                .GroupBy(group => group.Type)
                // And counting the number of objects in each group
                .ToDictionary(group => group.Key, group => group.Count());

            // Find the explicit objects mappable to implicit ones by taking the assumptions
            ExplicitObjectsMappableToImplicitOnes = AssumptionGroups.Flatten()
                // And the conclusion
                .Concat(Conclusion)
                // Take their inner objects
                .SelectMany(templateTheorem => templateTheorem.InvolvedObjects)
                // That are distinct
                .Distinct()
                // And are objects with points
                .OfType<TheoremObjectWithPoints>()
                // And are defined explicitly
                .Where(templateObject => templateObject.DefinedByExplicitObject
                    // And their explicit object is loose
                    && templateObject.ConfigurationObject is LooseConfigurationObject
                    // And there is no assumption 
                    && !AssumptionGroups.Flatten()
                    // Or conclusion
                    .Concat(Conclusion)
                    // That is incidence 
                    .Any(templateTheorem => templateTheorem.Type == TheoremType.Incidence
                        // And contains this object
                        && templateTheorem.InvolvedObjects.Contains(templateObject)))
                // Enumerate
                .ToReadOnlyHashSet();
        }

        #endregion
    }
}