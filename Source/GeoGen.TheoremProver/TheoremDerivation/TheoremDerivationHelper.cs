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
        #region Public properties

        /// <summary>
        /// The map of theorems of which we're looking for a proof. 
        /// </summary>
        public TheoremMap Theorems { get; }

        #endregion

        #region Public events

        /// <summary>
        /// Raises when there is a new proven theorem by the underlying back-end.
        /// </summary>
        public event Action<Theorem> TheoremProven = _ => { };

        #endregion

        #region Private fields

        /// <summary>
        /// The knowledge base that performs the logical deduction and composes
        /// proofs of theorems.
        /// </summary>
        private readonly KnowledgeBase<Theorem, TheoremDerivationData> _knowledgeBase;

        /// <summary>
        /// The dictionary mapping theorem types to theorems that are left to prove. 
        /// The theorems are a subset of <see cref="Theorems"/>.
        /// </summary>
        private readonly Dictionary<TheoremType, HashSet<Theorem>> _unprovenTheorems;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremDerivationHelper"/> class.
        /// </summary>
        /// <param name="theorems">The map of theorems of which we're looking for a proof. </param>
        public TheoremDerivationHelper(TheoremMap theorems)
        {
            // Set the theorems to prove
            Theorems = theorems ?? throw new ArgumentNullException(nameof(theorems));

            // Initially they are unproven
            _unprovenTheorems = Theorems.ToDictionary(pair => pair.Key, pair => pair.Value.ToSet());

            // Initialize the underlying knowledge base
            _knowledgeBase = new KnowledgeBase<Theorem, TheoremDerivationData>();

            // Listen to any proof 
            _knowledgeBase.TheoremProven += provenTheorem =>
            {
                // Get the type of the proven theorem for comfort
                var type = provenTheorem.Type;

                // If this theorem has been marked as one to be proved
                if (_unprovenTheorems.ContainsKey(type))
                {
                    // Get the set of theorems to prove of this type
                    var theoremsToProve = _unprovenTheorems[type];

                    // Remove the theorem from it
                    theoremsToProve.Remove(provenTheorem);

                    // If the set is empty
                    if (theoremsToProve.IsEmpty())
                        // Make sure the type is not longer needed
                        _unprovenTheorems.Remove(type);
                }

                // Raise an event of this class that we've proven theorem
                TheoremProven(provenTheorem);
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Find out whether there is any of <see cref="Theorems"/> still left to be proven.
        /// </summary>
        /// <returns>true, if there is a theorem to be proven; false otherwise.</returns>
        public bool AnyTheoremLeftToProve() => _unprovenTheorems.Any();

        /// <summary>
        /// Gets the theorems that are not proven yet.
        /// </summary>
        /// <returns>The theorems to be proven.</returns>
        public IEnumerable<Theorem> UnprovenTheoremsOfTypes(params TheoremType[] types)
            // Merge the theorems of the requested types
            => types.SelectMany(type => _unprovenTheorems.GetOrDefault(type) ?? Enumerable.Empty<Theorem>());

        /// <summary>
        /// Finds out if there still is a theorem of a given type that is left to be proven.
        /// </summary>
        /// <param name="type">The theorem type.</param>
        /// <returns>true, if there is a theorem of the given type to be proven; false otherwise.</returns>
        public bool AnyTheoremLeftToProveOfType(TheoremType type) => _unprovenTheorems.ContainsKey(type);

        /// <summary>
        /// Adds the derivation of a given theorem using a given derivation rule and 
        /// possibly some dependent theorems.
        /// </summary>
        /// <param name="derivedTheorem">The derived theorem.</param>
        /// <param name="rule">The used derivation rule.</param>
        /// <param name="neededTheorems">The theorems needed to be assumed for this derivation.</param>
        public void AddDerivation(Theorem derivedTheorem, DerivationRule rule, IEnumerable<Theorem> neededTheorems)
            // Delegate the call to the back-end
            => _knowledgeBase.AddDerivation(derivedTheorem, new TheoremDerivationData(rule), neededTheorems);

        /// <summary>
        /// Adds the derivation of a given theorem using a given theorem derivation data and 
        /// possibly some dependent theorems.
        /// </summary>
        /// <param name="derivedTheorem">The derived theorem.</param>
        /// <param name="data">The metadata of the derivation.</param>
        /// <param name="neededTheorems">The theorems needed to be assumed for this derivation.</param>
        public void AddDerivation(Theorem derivedTheorem, TheoremDerivationData data, IEnumerable<Theorem> neededTheorems)
            // Delegate the call to the back-end
            => _knowledgeBase.AddDerivation(derivedTheorem, data, neededTheorems);

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
            foreach (var theorem in Theorems.AllObjects)
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
            // Use the helper method to examine the inner attempts at our theorems
            // This way we might miss our main theorems that had no attempt, but 
            // that's okay, because we don't want to include them anyway
            var unprovenDiscoveredTheorems = GetAllUnprovenTheorems(unprovenTheorems.SelectMany(pair => pair.Value))
                // Exclude the attempts at our main theorems
                .Where(attempt => !Theorems.ContainsTheorem(attempt.Key))
                // Convert to a dictionary
                .ToDictionary(pair => pair.Key, pair => (IReadOnlyList<TheoremProofAttempt>)pair.Value.ToArray());

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
            var proofAttempt = new TheoremProofAttempt
            (
                theorem: derivation.Theorem,
                data: derivation.Data
            );

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
        /// Traverses given initial attempts in order to find all unproven theorems with their
        /// unfinished attempts (there might be no attempt). This recursively includes attempts 
        /// at attempted theorems.
        /// </summary>
        /// <param name="initialAttempts">The initial attempts that are to be examined.</param>
        /// <returns>The dictionary mapping theorems to their unfinished proof attempts.</returns>
        private static Dictionary<Theorem, HashSet<TheoremProofAttempt>> GetAllUnprovenTheorems(IEnumerable<TheoremProofAttempt> initialAttempts)
        {
            // Prepare the result
            var unprovenTheorems = new Dictionary<Theorem, HashSet<TheoremProofAttempt>>();

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

                // If it's been examined, we won't do it again
                if (examined.Contains(proofAttempt))
                    continue;
                // Otherwise we mark it as examined
                else
                    examined.Add(proofAttempt);

                // If it's proven, we're not interested
                if (proofAttempt.IsSuccessful)
                    continue;

                // Otherwise mark sure it's in the result
                unprovenTheorems.GetOrAdd(proofAttempt.Theorem, () => new HashSet<TheoremProofAttempt>()).Add(proofAttempt);

                // Go through the unproven assumptions
                proofAttempt.UnprovenAssumptions.ForEach(pair =>
                {
                    // Deconstruct
                    var (theorem, unfinishedProofs) = pair;

                    // Mark sure the theorem is marked as an unproven one
                    if (!unprovenTheorems.ContainsKey(theorem))
                        unprovenTheorems.Add(theorem, new HashSet<TheoremProofAttempt>());

                    // Make sure the inner attempts will be examined as well
                    unfinishedProofs.ForEach(toBeExamined.Enqueue);
                });
            }

            // Return the result
            return unprovenTheorems;
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
            var convertedAttempt = new TheoremProofAttempt
            (
                theorem: proofAttempt.Theorem,
                data: proofAttempt.Data
            );

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
                                data: new TheoremDerivationData(Transitivity),
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
                data: new TheoremDerivationData(Transitivity),
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