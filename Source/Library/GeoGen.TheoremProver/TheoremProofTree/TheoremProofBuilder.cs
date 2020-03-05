using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;
using static GeoGen.TheoremProver.InferenceRuleType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// A helper class that can build <see cref="TheoremProof"/>s. It assumes that assumptions of implications are 
    /// either already marked proven, or they are theorems of type <see cref="EqualObjects"/> that can be logically 
    /// inferred from the previous equality theorems (this means the builder can make inferences on its own of type 
    /// <see cref="EqualityTransitivity"/> and <see cref="ReformulatedTheorem"/>).
    /// </summary>
    public class TheoremProofBuilder
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping proved theorems to the way they have been proved represented by a <see cref="TheoremInferenceData"/> 
        /// and needed assumptions.
        /// </summary>
        private readonly Dictionary<Theorem, (TheoremInferenceData inferenceData, IEnumerable<Theorem> assumptions)> _knowledgeBase = new Dictionary<Theorem, (TheoremInferenceData, IEnumerable<Theorem>)>();

        /// <summary>
        /// The helper that keeps track of theorems of type <see cref="EqualObjects"/> and establishes new ones via
        /// <see cref="EqualityTransitivity"/> rule.
        /// </summary>
        private readonly EqualityHelper _equalityHelper = new EqualityHelper();

        #endregion

        #region Public method

        /// <summary>
        /// Builds theorem proofs for those given theorems that have been proved. 
        /// </summary>
        /// <param name="theorems">The theorems for which we want to build proofs.</param>
        /// <returns>The dictionary mapping proved theorems to their proofs.</returns>
        public IReadOnlyDictionary<Theorem, TheoremProof> BuildProofs(IEnumerable<Theorem> theorems)
        {
            // Prepare a dictionary mapping theorems to their proofs
            var proofDictionary = new Dictionary<Theorem, TheoremProof>();

            #region Add proof function

            // A local recursive function that constructs and adds the theorem proof of a given theorem to the proof dictionary
            void AddProof(Theorem theorem)
            {
                // If the theorem hasn't been inferred, we can't do much
                if (!_knowledgeBase.ContainsKey(theorem))
                    return;

                // If the proof is already in our local proof dictionary, we're done too
                if (proofDictionary.ContainsKey(theorem))
                    return;

                // Otherwise we need to build the proof, deconstruct the inference
                var (inferenceData, assumptions) = _knowledgeBase[theorem];

                // We need to construct the proofs of assumptions
                var assumptionProofs = assumptions
                    // For every assumption
                    .Select(assumption =>
                    {
                        // Make sure the proof is constructed
                        AddProof(assumption);

                        // Return it (it should be here as AddImplication method takes care of it)
                        return proofDictionary[assumption];
                    })
                    // Enumerate
                    .ToArray();

                // Finally we can construct the proof of the current theorem
                var newProof = new TheoremProof(theorem, inferenceData, assumptionProofs);

                // And mark it in the proof dictionary
                proofDictionary.Add(theorem, newProof);
            }

            #endregion

            // Try to proof the passed theorems
            theorems.ForEach(AddProof);

            // Finally take the theorems
            return theorems
                // That have a proof in the proof dictionary
                .Where(proofDictionary.ContainsKey)
                // Enumerate to a dictionary
                .ToDictionary(theorem => theorem, theorem => proofDictionary[theorem]);
        }

        /// <summary>
        /// Mark that a given conclusion can be proved by a given rule with given assumptions. The assumptions have to be
        /// either already proved, or <see cref="EqualObjects"/> theorems that can be inferred from already established
        /// theorems of this type.
        /// </summary>
        /// <param name="ruleType">The type of rule that is used in the proof.</param>
        /// <param name="conclusion">The proved conclusion.</param>
        /// <param name="assumptions">The assumptions that imply the conclusion.</param>
        public void AddImplication(InferenceRuleType ruleType, Theorem conclusion, IEnumerable<Theorem> assumptions)
            // Delegate the call to the most general method with the type wrapped in a data class
            => AddImplication(new TheoremInferenceData(ruleType), conclusion, assumptions);

        /// <summary>
        /// Mark that a given conclusion can be proved with a given inference data and with given assumptions. The assumptions 
        /// have to be either already proved, or <see cref="EqualObjects"/> theorems that can be inferred from already established
        /// theorems of this type.
        /// </summary>
        /// <param name="inferenceData">The inference data explaining the proof.</param>
        /// <param name="conclusion">The proved conclusion.</param>
        /// <param name="assumptions">The assumptions that imply the conclusion.</param>
        public void AddImplication(TheoremInferenceData inferenceData, Theorem conclusion, IEnumerable<Theorem> assumptions)
        {
            #region Trying to infer unproved EqualObjects assumptions

            // The equality helper has taken care of equality assumptions that might be inferred via transitivity.
            // The problem is there are still some inferable equalities it cannot handle of type 'X=Y => f(X)=f(Y)'
            // If we come across an unproven equality A=B, we will try to find objects A', B' such that A = A', B = B',
            // and A', B' are both constructed objects with the same construction and provable equality of their arguments

            // Go through those assumptions that are not proved 
            assumptions.Where(assumption => !_knowledgeBase.ContainsKey(assumption)
                    // And are an equality, which can be inferred
                    && assumption.Type == EqualObjects)
                // Try to infer them
                .ForEach(assumption =>
                {
                    // Get the equal objects
                    var equalObjects = assumption.GetInnerConfigurationObjects();

                    // Unwrap them for comfort
                    var object1 = equalObjects[0];
                    var object2 = equalObjects[1];

                    // Prepare the potential pairs of the A', B' objects described at the beginning of this region
                    // by taking all objects equal to the first object that are even constructed
                    var equalArgumentsTriple = _equalityHelper.GetEqualObjects(object1).OfType<ConstructedConfigurationObject>()
                        // And combining them with the constructed objects equal to the second objects
                        .CombinedWith(_equalityHelper.GetEqualObjects(object2).OfType<ConstructedConfigurationObject>())
                        // Take only dost pairs that have the same construction
                        .Where(pair => pair.Item1.Construction.Equals(pair.Item2.Construction))
                        // We will try to find an order of the arguments of the first object that can be used to infer A' = B'
                        .Select(pair =>
                        {
                            // Deconstruct
                            var (innerObject1, innerObject2) = pair;

                            // Try to find the order of the arguments of the first object
                            var equalArguments = innerObject1.PassedArguments.FlattenedList.Permutations()
                                // That still represents the same object
                                .Where(innerObject1.CanWeReorderArgumentsLikeThis)
                                // Zip it with the arguments of the second object
                                .Select(permutation => permutation.Zip(innerObject2.PassedArguments.FlattenedList))
                                // Take the first such an order that represents a provable equality
                                .FirstOrDefault(pairs => pairs.All(pair => _equalityHelper.AreEqual(pair.First, pair.Second)));

                            // Return it with the given objects
                            return (innerObject1, innerObject2, equalArguments);
                        })
                        // Take the first result where there is a valid argument equality
                        .FirstOrDefault(triple => triple.equalArguments != null);

                    // If there is no result, we can't do more
                    if (equalArgumentsTriple == default)
                        return;

                    #region Finding used equalities

                    // Otherwise we might extract the used equalities to make this conclusion
                    // First we take equalities from the equal arguments
                    var usedEqualities = equalArgumentsTriple.equalArguments
                        // That are not trivial
                        .Where(pair => !pair.First.Equals(pair.Second))
                        // Such pairs make a used non-trivial equality theorem
                        .Select(pair => new Theorem(EqualObjects, pair.First, pair.Second))
                        // Enumerate, just in case to a set (we will be adding other equalities to it)
                        .ToHashSet();

                    // If the equality of the first objects is not trivial, mark it
                    if (!equalArgumentsTriple.innerObject1.Equals(object1))
                        usedEqualities.Add(new Theorem(EqualObjects, equalArgumentsTriple.innerObject1, object1));

                    // If the equality of the second objects is not trivial, mark it
                    if (!equalArgumentsTriple.innerObject2.Equals(object2))
                        usedEqualities.Add(new Theorem(EqualObjects, equalArgumentsTriple.innerObject2, object2));

                    #endregion

                    // Now we have inferred this assumption, we can mark it as a reformulated theorem
                    _knowledgeBase.Add(assumption, (new TheoremInferenceData(ReformulatedTheorem), usedEqualities));

                    // And mark it in the equality helper too
                    MarkEquality(assumption);
                });

            #endregion

            // Ensure there is no unproven assumptions, make sure this will fail
            if (assumptions.Any(assumption => !_knowledgeBase.ContainsKey(assumption)))
                throw new TheoremProverException("Cannot add an implicating containing an unproven assumption.");

            // Otherwise the proof, ensure this theorem is proven in the knowledge base
            _knowledgeBase.TryAdd(conclusion, (inferenceData, assumptions));

            // If this is an equality, mark it to the helper
            if (conclusion.Type == EqualObjects)
                MarkEquality(conclusion);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Ensures that a given theorem of type <see cref="EqualObjects"/> is marked in the <see cref="_equalityHelper"/>
        /// and all the subsequently inferred equalities are added to the <see cref="_knowledgeBase"/>.
        /// </summary>
        /// <param name="equality">The equality theorem to be marked.</param>
        private void MarkEquality(Theorem equality)
        {
            // Get the objects that are equal
            var equalobjects = equality.GetInnerConfigurationObjects();

            // Mark their equality to the marked
            _equalityHelper.MarkEqualObjects((equalobjects[0], equalobjects[1]), inferredEqualities: out var inferedEqualities);

            // Handle the subsequently inferred equalities
            foreach (var (inferedEquality, usedEqualities) in inferedEqualities)
            {
                // Prepare a data explaining that the equality transitivity rule has been used
                var data = new TheoremInferenceData(EqualityTransitivity);

                // Ensure the inferred equality is in the knowledge base
                _knowledgeBase.TryAdd(inferedEquality, (data, usedEqualities));
            }
        }

        #endregion
    }
}