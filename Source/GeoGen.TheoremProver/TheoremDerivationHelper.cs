using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.TheoremProver.DerivationRule;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a class that performs logical derivation of <see cref="Theorem"/>.
    /// It internally reuses <see cref="KnowledgeBase{TTheorem, TData}"/>. It also
    /// constructs the final <see cref="TheoremProverOutput"/> that even has flattened
    /// transitivities (<see cref="GetRidOfNestedTransitivity(TheoremProverOutput)"/>).
    /// </summary>
    public class TheoremDerivationHelper
    {
        #region TheoremDerivationData class

        /// <summary>
        /// Represents metadata of a subtheorem derivation used as TData type for
        /// <see cref="KnowledgeBase{TTheorem, TData}"/>.
        /// </summary>
        private class TheoremDerivationData
        {
            #region Public properties

            /// <summary>
            /// The used derivation rule.
            /// </summary>
            public DerivationRule Rule { get; }

            #endregion

            #region Constructor

            /// <summary>
            /// Initialize a new instance of the <see cref="TheoremDerivationData"/> class.
            /// </summary>
            /// <param name="rule">The used derivation rule.</param>
            public TheoremDerivationData(DerivationRule rule)
            {
                Rule = rule;
            }

            #endregion
        }

        #endregion

        #region SubtheoremDerivationData class

        /// <summary>
        /// Represents metadata of a subtheorem derivation.
        /// </summary>
        private class SubtheoremDerivationData : TheoremDerivationData
        {
            #region Public properties

            /// <summary>
            /// The theorem that is assumed to be true and was found implying some theorem.
            /// </summary>
            public Theorem TemplateTheorem { get; }

            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="SubtheoremDerivationData"/> class.
            /// </summary>
            /// <param name="templateTheorem">The theorem that is assumed to be true and was found implying some theorem.</param>
            public SubtheoremDerivationData(Theorem templateTheorem)
                : base(DerivationRule.Subtheorem)
            {
                TemplateTheorem = templateTheorem ?? throw new ArgumentNullException(nameof(templateTheorem));
            }

            #endregion
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The map of theorems of which we're looking for a proof. 
        /// </summary>
        public TheoremsMap MainTheorems { get; }

        #endregion

        #region Private fields

        /// <summary>
        /// The knowledge base that performs the logical deduction and composes
        /// proofs of theorems.
        /// </summary>
        private readonly KnowledgeBase<Theorem, TheoremDerivationData> _knowledgeBase;

        /// <summary>
        /// The dictionary mapping theorem types to theorems that are left to prove. 
        /// This might include even theorems that are not <see cref="MainTheorems"/>, 
        /// but have been added as needed theorems of some derivation.
        /// </summary>
        private readonly Dictionary<TheoremType, HashSet<Theorem>> _allTheoremsToProve;

        /// <summary>
        /// The set of the <see cref="MainTheorems"/> that are still unproven.
        /// </summary>
        private readonly HashSet<Theorem> _mainTheoremsLeftToProve;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremDerivationHelper"/> class.
        /// </summary>
        /// <param name="mainTheorems">The map of theorems of which we're looking for a proof. </param>
        public TheoremDerivationHelper(TheoremsMap mainTheorems)
        {
            // Set the theorems to prove
            MainTheorems = mainTheorems ?? throw new ArgumentNullException(nameof(mainTheorems));

            // Initially all of them are left to prove
            _mainTheoremsLeftToProve = new HashSet<Theorem>(mainTheorems.AllObjects);

            // We want to mark them in the dictionary of all theorems to prove as well
            _allTheoremsToProve = _mainTheoremsLeftToProve.GroupBy(t => t.Type).ToDictionary(group => group.Key, group => group.ToSet());

            // Initialize the underlying knowledge base
            _knowledgeBase = new KnowledgeBase<Theorem, TheoremDerivationData>();

            // Listen to any proof 
            _knowledgeBase.TheoremProven += provenTheorem =>
            {
                // Get the type of the proven theorem for comfort
                var type = provenTheorem.Type;

                // If this theorem has been marked as one to be proved
                if (_allTheoremsToProve.ContainsKey(type))
                {
                    // Get the set of theorems to prove of this type
                    var theoremsToProve = _allTheoremsToProve[type];

                    // Remove the theorem from it
                    theoremsToProve.Remove(provenTheorem);

                    // If the set is empty
                    if (theoremsToProve.IsEmpty())
                        // Make sure the type is not longer needed
                        _allTheoremsToProve.Remove(type);
                }

                // Make sure it's not in the main theorems set now (no matter whether it's main or not, it's proven)
                _mainTheoremsLeftToProve.Remove(provenTheorem);
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Find out whether there is any of <see cref="MainTheorems"/> still left to be proven.
        /// </summary>
        /// <returns>true, if there is a main theorem to be proven; false otherwise.</returns>
        public bool AnyMainTheoremLeftToProve() => _mainTheoremsLeftToProve.Any();

        /// <summary>
        /// Finds out if there still is a theorem of a given type that is left to be proven.
        /// This theorem doesn't have to be one of <see cref="MainTheorems"/>, but it might 
        /// be useful to prove one of these theorems.
        /// </summary>
        /// <param name="type">The theorem type.</param>
        /// <returns>true, if there is a theorem of the given type to be proven; false otherwise.</returns>
        public bool AnyTheoremLeftToProveOfType(TheoremType type) => _allTheoremsToProve.ContainsKey(type);

        /// <summary>
        /// Adds the derivation of a given theorem using a given derivation rule and 
        /// possibly some dependent theorems.
        /// </summary>
        /// <param name="derivedTheorem">The derived theorem.</param>
        /// <param name="rule">The used derivation rule.</param>
        /// <param name="neededTheorems">The theorems needed to be assumed for this derivation.</param>
        public void AddDerivation(Theorem derivedTheorem, DerivationRule rule, IEnumerable<Theorem> neededTheorems)
            // Delegate the call to the private method
            => AddDerivation(derivedTheorem, new TheoremDerivationData(rule), neededTheorems);

        /// <summary>
        /// Adds the derivation with the rule <see cref="Subtheorem"/>
        /// of a given theorem using possibly some dependent theorems.
        /// </summary>
        /// <param name="derivedTheorem">The derived theorem.</param>
        /// <param name="templateTheorem">The used template theorem.</param>
        /// <param name="neededTheorems">The theorems needed to be assumed for this derivation.</param>
        public void AddSubtheoremDerivation(Theorem derivedTheorem, Theorem templateTheorem, IEnumerable<Theorem> neededTheorems)
            // Delegate the call to the private method
            => AddDerivation(derivedTheorem, new SubtheoremDerivationData(templateTheorem), neededTheorems);

        /// <summary>
        /// Constructs a <see cref="TheoremProverOutput"/> representing the current state of the 
        /// underlying knowledge base of the helper.
        /// </summary>
        /// <returns>The constructed output.</returns>
        public TheoremProverOutput ConstructResult()
        {
            // Prepare the dictionary for finished proofs
            var provenTheorems = new Dictionary<Theorem, TheoremProofAttempt>();

            // Prepare the dictionary for unfinished proofs
            var unprovenTheorems = new Dictionary<Theorem, IReadOnlyList<TheoremProofAttempt>>();

            // Prepare the cache dictionary of already finished theorem derivations
            var cache = new Dictionary<TheoremDerivation<Theorem, TheoremDerivationData>, TheoremProofAttempt>();

            // Go through the considered theorems
            foreach (var theorem in MainTheorems.AllObjects)
            {
                // Handle the case where there is a proof
                if (_knowledgeBase.IsProven(theorem))
                {
                    // Convert the proof
                    var finalDerivation = TheoremDerivationToTheoremProofAttempt(_knowledgeBase.GetProof(theorem), cache);

                    // Add it to the proven theorems dictionary
                    provenTheorems.Add(theorem, finalDerivation);
                }
                // Handle the case where there are only attempts
                else
                {
                    // Get the derivation attempts from the knowledge base 
                    var attempts = _knowledgeBase.GetDerivationAttempts(theorem)
                        // Convert each to a proof attempt
                        .Select(derivation => TheoremDerivationToTheoremProofAttempt(derivation, cache))
                        // Enumerate
                        .ToArray();

                    // Add them to the unfinished proofs dictionary
                    unprovenTheorems.Add(theorem, attempts);
                }
            }

            // Find all unproven discovered theorems
            // Use a helper method to take all unfinished proof attempts of all theorems
            var unprovenDiscoveredTheorems = GetUnfishinedProofAttempts(unprovenTheorems.SelectMany(pair => pair.Value))
                // Exclude the attempts at our main theorems
                .Where(attempt => !MainTheorems.ContainsTheorem(attempt.Theorem))
                // Group the attempts by the theorem
                .GroupBy(attempt => attempt.Theorem)
                // Convert to a dictionary
                .ToDictionary(group => group.Key, group => (IReadOnlyList<TheoremProofAttempt>)group.ToArray());

            // Construct the result
            var theoremProverOutput = new TheoremProverOutput
            (
                provenTheorems: provenTheorems,
                unprovenTheorems: unprovenTheorems,
                unprovenDiscoveredTheorems: unprovenDiscoveredTheorems
            );

            // Return the output with handled transitivity paths
            return GetRidOfNestedTransitivity(theoremProverOutput);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Adds the derivation of a given theorem using a given theorem derivation data and 
        /// possibly some dependent theorems.
        /// </summary>
        /// <param name="derivedTheorem">The derived theorem.</param>
        /// <param name="data">The metadata of the derivation.</param>
        /// <param name="neededTheorems">The theorems needed to be assumed for this derivation.</param>
        private void AddDerivation(Theorem derivedTheorem, TheoremDerivationData data, IEnumerable<Theorem> neededTheorems)
        {
            // If this theorem is proven, don't do anything
            if (_knowledgeBase.IsProven(derivedTheorem))
                return;

            // Make sure the needed theorems are marked as to be proven, if they are not already
            foreach (var theorem in neededTheorems)
                // If the current one is not proven
                if (!_knowledgeBase.IsProven(theorem))
                    // Add it to the dictionary mapping types to theorems to prove
                    _allTheoremsToProve.GetOrAdd(theorem.Type, () => new HashSet<Theorem>()).Add(theorem);

            // Add the derivation to the knowledge base
            _knowledgeBase.AddDerivation(derivedTheorem, data, neededTheorems);
        }

        /// <summary>
        /// Converts the type <see cref="TheoremDerivation{TTheorem, TData}"/> used by the <see cref="KnowledgeBase{TTheorem, TData}"/>
        /// to the type <see cref="TheoremProofAttempt"/> required to be returned by the prover service.
        /// </summary>
        /// <param name="derivation">The derivation to be converted.</param>
        /// <param name="cache">The cache dictionary mapping already converted derivations to proof attempts.</param>
        /// <returns>The converted proof attempt.</returns>
        private TheoremProofAttempt TheoremDerivationToTheoremProofAttempt(TheoremDerivation<Theorem, TheoremDerivationData> derivation,
                                                                           Dictionary<TheoremDerivation<Theorem, TheoremDerivationData>, TheoremProofAttempt> cache)
        {
            // Try to look to the cache first
            if (cache.ContainsKey(derivation))
                // If the derivation it's there, return it
                return cache[derivation];

            // Prepare the resulting attempt
            var proofAttempt = derivation.Data switch
            {
                // Handle the subtheorem proof case
                SubtheoremDerivationData subtheoremDerivationData => new SubtheoremProofAttempt
                (
                    theorem: derivation.Theorem,
                    templateTheorem: subtheoremDerivationData.TemplateTheorem
                ),

                // By default construct a standard proof attempt
                _ => new TheoremProofAttempt
                (
                    theorem: derivation.Theorem,
                    rule: derivation.Data.Rule
                )
            };

            // Cache the result
            // NOTE: It is important to do it now, because there might be (and usually is) a cycle
            cache.Add(derivation, proofAttempt);

            // Convert the proven theorems
            var finishedDerivations = derivation.ProvenTheorems
                // Each needed theorem certainly has a proof, which is recursively converted
                .Select(neededTheorem => TheoremDerivationToTheoremProofAttempt(_knowledgeBase.GetProof(neededTheorem), cache))
                // Enumerate
                .ToArray();

            // Convert the theorems to be proven
            var unfinishedDerivations = derivation.TheoremsToBeProven
                // For each theorem we take all potential derivations
                .Select(neededTheorem => (neededTheorem, attempts: _knowledgeBase.GetDerivationAttempts(neededTheorem)
                    // Recursively convert each
                    .Select(theoremDerivation => TheoremDerivationToTheoremProofAttempt(theoremDerivation, cache))
                    // Wrap these results in an array
                    .ToArray() as IReadOnlyList<TheoremProofAttempt>))
                // Enumerate the whole thing
                .ToArray();

            // Set the created proven and unproven assumptions
            proofAttempt.ProvenAssumptions = finishedDerivations;
            proofAttempt.UnprovenAssumptions = unfinishedDerivations;

            // Return the final proof attempt
            return proofAttempt;
        }

        /// <summary>
        /// Traverses given initial attempts in order to find all unfinished proof attempts,
        /// including the attempts at inner theorems.
        /// </summary>
        /// <param name="initialAttempts">The initial attempts that are to be examined.</param>
        /// <returns>The enumerable of unfinished proof attempts.</returns>
        private static IEnumerable<TheoremProofAttempt> GetUnfishinedProofAttempts(IEnumerable<TheoremProofAttempt> initialAttempts)
        {
            // Prepare the set of proof attempts that have been already examined
            var examined = new HashSet<TheoremProofAttempt>();

            // Prepare the queue where we will stored attempts to be examined
            var toBeExamined = new Queue<TheoremProofAttempt>();

            // Initially we want our initials attempt to be in the queue
            initialAttempts.ForEach(toBeExamined.Enqueue);

            // Do the examination until there is something to examine
            while (!toBeExamined.IsEmpty())
            {
                // Get the current proof attempt from the queue
                var proofAttempt = toBeExamined.Dequeue();

                // Return it
                yield return proofAttempt;

                // If it's been examined, we won't do it again
                if (examined.Contains(proofAttempt))
                    continue;
                // Otherwise we mark it as examined
                else
                    examined.Add(proofAttempt);

                // From the unproved assumptions take all unfinished proofs and mark them to be examined
                proofAttempt.UnprovenAssumptions.SelectMany(pair => pair.unfinishedProofs).ForEach(toBeExamined.Enqueue);
            }
        }

        /// <summary>
        /// Converts all theorem attempts from a given output to theorem attempts where all 
        /// related transitivity attempts are flattened to a single attempt.
        /// </summary>
        /// <param name="output">The output to be converted.</param>
        /// <returns>The converted output without long transitivities.</returns>
        private static TheoremProverOutput GetRidOfNestedTransitivity(TheoremProverOutput output)
        {
            // Prepare the cache for already handled proof attempts
            var cache = new Dictionary<TheoremProofAttempt, TheoremProofAttempt>();

            // Convert the proven theorems using the helper method
            var provenTheorems = output.ProvenTheorems.ToDictionary(pair => pair.Key, pair => GetRidOfNestedTransitivity(pair.Value, cache));

            // Convert the unproven theorems using the helper method
            var unprovenTheorems = output.UnprovenTheorems.ToDictionary(pair => pair.Key, pair => GetRidOfNestedTransitivity(pair.Value, cache));

            // Convert the unproven discovered theorems using the helper method
            var unprovenDiscoveredTheorems = output.UnprovenDiscoveredTheorems.ToDictionary(pair => pair.Key, pair => GetRidOfNestedTransitivity(pair.Value, cache));

            // Recreate the result
            return new TheoremProverOutput
            (
                provenTheorems: provenTheorems,
                unprovenTheorems: unprovenTheorems,
                unprovenDiscoveredTheorems: unprovenDiscoveredTheorems
            );
        }

        /// <summary>
        /// Convert a given proof attempt to a proof attempt where all related transitivities 
        /// are flattened to a single attempt.
        /// </summary>
        /// <param name="proofAttempt">The proof attempt to be converted.</param>
        /// <param name="cache">The cache of currently converted attempts.</param>
        /// <returns>The theorem proof attempt with flattened transitivities.</returns>
        private static TheoremProofAttempt GetRidOfNestedTransitivity(TheoremProofAttempt proofAttempt,
                                                                      Dictionary<TheoremProofAttempt, TheoremProofAttempt> cache)
        {
            // Try to look to the cache first
            if (cache.ContainsKey(proofAttempt))
                // If the attempt is there, return it
                return cache[proofAttempt];

            // If this is a transitivity attempt...
            if (proofAttempt.Rule == Transitivity)
                // Use the helper method
                return ConvertTransitiviteAttempts(new[] { proofAttempt }, cache);

            // Prepare the resulting attempt
            var convertedAttempt = proofAttempt switch
            {
                // Handle the subtheorem proof case
                SubtheoremProofAttempt subtheoremProofAttempt => new SubtheoremProofAttempt
                (
                    theorem: proofAttempt.Theorem,
                    templateTheorem: subtheoremProofAttempt.TemplateTheorem
                ),

                // By default construct a standard proof attempt
                _ => new TheoremProofAttempt
                (
                    theorem: proofAttempt.Theorem,
                    rule: proofAttempt.Rule
                )
            };

            // Cache it
            cache.Add(proofAttempt, convertedAttempt);

            // Recursively resolve the proven assumptions
            var provenAssumptions = proofAttempt.ProvenAssumptions.Select(attempt => GetRidOfNestedTransitivity(attempt, cache)).ToList();

            // Recursive resolve unproven assumptions
            var unprovenAssumptions = proofAttempt.UnprovenAssumptions.Select(pair => (pair.theorem, GetRidOfNestedTransitivity(pair.unfinishedProofs, cache))).ToList();

            // Set them to the result
            convertedAttempt.ProvenAssumptions = provenAssumptions;
            convertedAttempt.UnprovenAssumptions = unprovenAssumptions;

            // Return the result
            return convertedAttempt;
        }

        /// <summary>
        /// Convert given proof attempts at a single theorem a list of proof attempt 
        /// where all related transitivities are flattened to a single attempt.
        /// </summary>
        /// <param name="unfinishedProofs">The list of unfinished proof attempts.</param>
        /// <param name="cache">The cache of currently converted attempts.</param>
        /// <returns>The theorem proof attempt with flattened transitivities.</returns>
        private static IReadOnlyList<TheoremProofAttempt> GetRidOfNestedTransitivity(IReadOnlyList<TheoremProofAttempt> unfinishedProofs,
                                                                                     Dictionary<TheoremProofAttempt, TheoremProofAttempt> cache)
        {
            // Prepare the result
            var convertedProofAttempts = new List<TheoremProofAttempt>();

            // Prepare the result of transitivity attempts
            var transitivityProofAttempts = new List<TheoremProofAttempt>();

            // Go through the proof attempts
            foreach (var proofAttempt in unfinishedProofs)
            {
                // If it's not transitive, we can convert it right away
                if (proofAttempt.Rule != Transitivity)
                    convertedProofAttempts.Add(GetRidOfNestedTransitivity(proofAttempt, cache));
                // Otherwise we store it in the list and handle later
                else
                    transitivityProofAttempts.Add(proofAttempt);
            }

            // If there are any transitivity attempts...
            if (transitivityProofAttempts.Any())
                // Use helper function to glue all of them to one
                convertedProofAttempts.Add(ConvertTransitiviteAttempts(transitivityProofAttempts, cache));

            // Return the result
            return convertedProofAttempts;
        }

        /// <summary>
        /// Converts a given list of transitive attempts to a single attempt wrapping every inner
        /// transitive attempt.
        /// </summary>
        /// <param name="transitiveAttempts">The transitive attempts at the same theorem to be considered.</param>
        /// <param name="cache">The cache of currently converted attempts.</param>
        /// <returns>The single transitive theorem proof attempt.</returns>
        private static TheoremProofAttempt ConvertTransitiviteAttempts(IReadOnlyList<TheoremProofAttempt> transitiveAttempts,
                                                                       Dictionary<TheoremProofAttempt, TheoremProofAttempt> cache)
        {
            // Find all assumptions of all the transitive attempts that will be merged
            var allAssumptionAttemptsOfTransitiveAttempts = FindInnerTheoremsOfTransitiveAttempts(transitiveAttempts);

            // Prepare the list of proven assumptions of this transitive attempt
            var provenAssumptions = new List<TheoremProofAttempt>();

            // Prepare the dictionary mapping theorems to lists of their non-transitivity unsuccessful attempts
            var unprovenAssumptionsMap = new Dictionary<Theorem, List<TheoremProofAttempt>>();

            // Get the theorem being proven for comfort
            var theoremBeingProven = transitiveAttempts.First().Theorem;

            // Go through all the transitivity assumptions
            allAssumptionAttemptsOfTransitiveAttempts.ForEach(pair =>
            {
                // Deconstruct
                var (assumptionTheorem, assumptionProofAttempts) = pair;

                // If this theorem has been proven successfully, i.e. there is exactly one successful attempt
                if (assumptionProofAttempts.Count == 1 && assumptionProofAttempts.First().IsSuccessful)
                {
                    // Get the attempt for comfort
                    var assumptionProofAttempt = assumptionProofAttempts.First();

                    // If we don't have a transitivity proof
                    if (assumptionProofAttempt.Rule != Transitivity)
                        // We certainly want to include this proof, which recursively flattened transitivities
                        provenAssumptions.Add(GetRidOfNestedTransitivity(assumptionProofAttempt, cache));

                    // If we don't have a transitivity proof
                    else
                    {
                        // And the theorem at which this attempt was made wasn't proved
                        if (!transitiveAttempts.First().IsSuccessful)
                            // Then we want to include this attempt, but without the reason, because
                            // it obviously follows from the accompanied attempts of the final result
                            // Therefore we just add an empty transitive proof of the concerned theorem
                            provenAssumptions.Add(new TheoremProofAttempt
                            (
                                theorem: assumptionTheorem,
                                rule: Transitivity,
                                provenAssumptions: new List<TheoremProofAttempt>(),
                                unprovenAssumptions: new List<(Theorem, IReadOnlyList<TheoremProofAttempt>)>()
                            ));
                    }
                }
                // If it hasn't been successful
                else
                {
                    // We don't want to get into this obvious cycle where the theorem is its assumption as well
                    if (assumptionTheorem.Equals(theoremBeingProven))
                        return;

                    // Get the list
                    var attemptsList = unprovenAssumptionsMap.GetOrAdd(assumptionTheorem, () => new List<TheoremProofAttempt>());

                    // We want to include all non-transitive attempts
                    assumptionProofAttempts.Where(attempt => attempt.Rule != Transitivity)
                        // with recursively flattened transitivities                    
                        .ForEach(assumptionProofAttempt => attemptsList.Add(GetRidOfNestedTransitivity(assumptionProofAttempt, cache)));
                }
            });

            // Convert the map of unsuccessful attempts to the list as required in the constructed type
            var unprovenAssumptions = unprovenAssumptionsMap.Select(pair => (pair.Key, (IReadOnlyList<TheoremProofAttempt>)pair.Value.ToList())).ToList();

            // Construct the final result 
            return new TheoremProofAttempt
            (
                theorem: theoremBeingProven,
                rule: Transitivity,
                provenAssumptions: provenAssumptions,
                unprovenAssumptions: unprovenAssumptions
            );
        }

        /// <summary>
        /// Map theorems of transitive attempts (and sub-attempts) to the sets of these attempts. 
        /// </summary>
        /// <param name="transitiveAttempts">The initial transitive attempts.</param>
        /// <returns>The dictionary mapping assumption theorems to sets of the attempts at them.</returns>
        private static Dictionary<Theorem, HashSet<TheoremProofAttempt>> FindInnerTheoremsOfTransitiveAttempts(IReadOnlyList<TheoremProofAttempt> transitiveAttempts)
        {
            // Prepare the result
            var attempts = new Dictionary<Theorem, HashSet<TheoremProofAttempt>>();

            // Prepare the set of proof attempts that have been already examined
            var examined = new HashSet<TheoremProofAttempt>();

            // Prepare the queue where we will stored attempts to be examined
            var toBeExamined = new Queue<TheoremProofAttempt>();

            // Initially we want our initial attempts to be in the queue
            transitiveAttempts.ForEach(toBeExamined.Enqueue);

            // Do the examination until there is something to examine
            while (!toBeExamined.IsEmpty())
            {
                // Get the current proof attempt from the queue
                var currentAttempt = toBeExamined.Dequeue();

                // If it's been examined, we won't do it again
                if (examined.Contains(currentAttempt))
                    continue;
                // Otherwise we mark it as examined
                else
                    examined.Add(currentAttempt);

                // If we have a non-transitive attempt, we don't want to include its assumptions
                if (currentAttempt.Rule != Transitivity)
                    continue;

                // Add the proven assumptions
                currentAttempt.ProvenAssumptions.ForEach(assumptionAttempt =>
                {
                    // Add the assumption
                    attempts.GetOrAdd(assumptionAttempt.Theorem, () => new HashSet<TheoremProofAttempt>()).Add(assumptionAttempt);

                    // Enqueue it for further examination
                    toBeExamined.Enqueue(assumptionAttempt);
                });

                // Add the unproven assumptions
                currentAttempt.UnprovenAssumptions.ForEach(pair =>
                {
                    // Deconstruct
                    var (assumption, assumptionAttempts) = pair;

                    // Add all attempts
                    attempts.GetOrAdd(assumption, () => new HashSet<TheoremProofAttempt>()).UnionWith(assumptionAttempts);

                    // Enqueue them for further examination
                    assumptionAttempts.ForEach(toBeExamined.Enqueue);
                });
            }

            // Return the result
            return attempts;
        }

        #endregion
    }
}