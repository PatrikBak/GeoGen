using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;
using static GeoGen.TheoremProver.InferenceRuleType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremProver"/>. The main idea of this implementation
    /// is to use <see cref="InferenceRule"/>s to come up with new theorems. They are retrieved by an
    /// <see cref="IInferenceRuleManager"/> and applied by an <see cref="IInferenceRuleApplier"/>. The 
    /// question which rules we should use at a particular moment is answered by a helper class 
    /// <see cref="Scheduler"/>.
    /// <para>
    /// In order for this idea to work we need to have theorems that are initially true (axioms). These
    /// theorems will get passed. Furthermore, we will use a <see cref="ITrivialTheoremProducer"/> to find 
    /// other theorems that be stated as trivial ones.
    /// </para>
    /// <para>
    /// Any result retrieved via the <see cref="IInferenceRuleApplier"/> is check geometrically, which seems
    /// like the most reasonable way to handle degenerated theorems and orientation issues. This is done 
    /// via <see cref="IGeometricTheoremVerifier"/>. When there is an incorrectly inferred theorem, it is 
    /// traced via an <see cref="IInvalidInferenceTracer"/>.
    /// </para>
    /// <para>
    /// In order to make this efficient, this class uses a <see cref="NormalizationHelper"/>, that, in short,
    /// ensures that any theorem and object has exactly one 'normal' definition, to reduce duplicity.
    /// </para>
    /// <para>
    /// Sometimes even in very simple theorems we will to introduce new objects to a configuration in order
    /// to prove something. This is done via an <see cref="IObjectIntroducer"/>. The 'boring' work that involves
    /// communication with <see cref="NormalizationHelper"/> and ensuring that we don't introduce the same
    /// object twice is then delegated to a helper class <see cref="ObjectIntroductionHelper"/>.
    /// </para>
    /// <para>
    /// The prover is even able to construct <see cref="TheoremProof"/>s of the inferred theorems. This job is 
    /// delegated to a helper class <see cref="TheoremProofBuilder"/>.
    /// </para>
    /// </summary>
    public class TheoremProver : ITheoremProver
    {
        #region ProofData class

        /// <summary>
        /// A helper class that holds data needed for <see cref="TheoremProof"/> construction.
        /// </summary>
        private class ProofData
        {
            #region Public properties

            /// <summary>
            /// The proof builder that builds actual <see cref="TheoremProof"/> instances.
            /// </summary>
            public TheoremProofBuilder ProofBuilder { get; }

            /// <summary>
            /// The data explaining a theorem inference.
            /// </summary>
            public TheoremInferenceData InferenceData { get; }

            /// <summary>
            /// The assumptions of a theorem inference.
            /// </summary>
            public IReadOnlyList<Theorem> Assumptions { get; }

            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="ProofData"/> class.
            /// </summary>
            /// <param name="proofBuilder">The proof builder that builds actual <see cref="TheoremProof"/> instances.</param>
            /// <param name="inferenceData">The data explaining a theorem inference.</param>
            /// <param name="assumptions">The assumptions of a theorem inference.</param>
            public ProofData(TheoremProofBuilder proofBuilder, TheoremInferenceData inferenceData, IReadOnlyList<Theorem> assumptions)
            {
                ProofBuilder = proofBuilder ?? throw new ArgumentNullException(nameof(proofBuilder));
                InferenceData = inferenceData ?? throw new ArgumentNullException(nameof(inferenceData));
                Assumptions = assumptions ?? throw new ArgumentNullException(nameof(assumptions));
            }

            #endregion
        }

        #endregion

        #region Dependencies

        /// <summary>
        /// The settings for the prover.
        /// </summary>
        private readonly TheoremProverSettings _settings;

        /// <summary>
        /// The manager of <see cref="InferenceRule"/> that provides them categories.
        /// </summary>
        private readonly IInferenceRuleManager _manager;

        /// <summary>
        /// The applier of <see cref="InferenceRule"/> that infers theorems.
        /// </summary>
        private readonly IInferenceRuleApplier _applier;

        /// <summary>
        /// The produced of trivial <see cref="Theorem"/>s from <see cref="ConstructedConfigurationObject"/>s.
        /// </summary>
        private readonly ITrivialTheoremProducer _producer;

        /// <summary>
        /// The geometric verifier for inferred <see cref="Theorem"/>s.
        /// </summary>
        private readonly IGeometricTheoremVerifier _verifier;

        /// <summary>
        /// The introducer of new <see cref="ConstructedConfigurationObject"/>s that might be used in proofs.
        /// </summary>
        private readonly IObjectIntroducer _introducer;

        /// <summary>
        /// The tracer of invalid inferences, i.e. inferences that yielded invalid theorems.
        /// </summary>
        private readonly IInvalidInferenceTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProver"/> class.
        /// </summary>
        /// <param name="settings"><inheritdoc cref="_settings" path="/summary"/></param>
        /// <param name="manager"><inheritdoc cref="_manager" path="/summary"/></param>
        /// <param name="applier"><inheritdoc cref="_applier" path="/summary"/></param>
        /// <param name="producer"><inheritdoc cref="_producer" path="/summary"/></param>
        /// <param name="verifier"><inheritdoc cref="_verifier" path="/summary"/></param>
        /// <param name="introducer"><inheritdoc cref="_introducer" path="/summary"/></param>
        /// <param name="tracer"><inheritdoc cref="_tracer" path="/summary"/></param>
        public TheoremProver(TheoremProverSettings settings,
                             IInferenceRuleManager manager,
                             IInferenceRuleApplier applier,
                             ITrivialTheoremProducer producer,
                             IGeometricTheoremVerifier verifier,
                             IObjectIntroducer introducer,
                             IInvalidInferenceTracer tracer)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _applier = applier ?? throw new ArgumentNullException(nameof(applier));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _verifier = verifier ?? throw new ArgumentNullException(nameof(verifier));
            _introducer = introducer ?? throw new ArgumentNullException(nameof(introducer));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        #endregion

        #region ITheoremProver implementation

        /// <inheritdoc/>
        public IReadOnlyHashSet<Theorem> ProveTheorems(TheoremMap provenTheorems, TheoremMap theoremsToProve, ContextualPicture picture)
            // Delegate the call to the general method and cast the result
            => ProveTheorems(provenTheorems, theoremsToProve, picture, shouldWeConstructProofs: false);

        /// <inheritdoc/>
        public IReadOnlyDictionary<Theorem, TheoremProof> ProveTheoremsAndConstructProofs(TheoremMap provenTheorems, TheoremMap theoremsToProve, ContextualPicture picture)
            // Delegate the call to the general method and cast the result
            => ProveTheorems(provenTheorems, theoremsToProve, picture, shouldWeConstructProofs: true);

        /// <summary>
        /// Proves given theorems that are true in the configuration drawn in a given picture.
        /// </summary>
        /// <param name="provenTheorems">The theorems that hold in the configuration without the last object.</param>
        /// <param name="theoremsToProve">The theorems that say something about the last object.</param>
        /// <param name="picture">The picture where the configuration in which the theorems hold is drawn.</param>
        /// <param name="shouldWeConstructProofs">Indicates whether we should construct proofs. This will affect the type of returned result.</param>
        /// <returns>
        /// Either the output as for <see cref="ProveTheoremsAndConstructProofs(TheoremMap, TheoremMap, ContextualPicture)"/>,
        /// if we are constructing proof, or the output as for <see cref="ProveTheorems(TheoremMap, TheoremMap, ContextualPicture)"/> otherwise.
        /// </returns>
        private dynamic ProveTheorems(TheoremMap provenTheorems, TheoremMap theoremsToProve, ContextualPicture picture, bool shouldWeConstructProofs)
        {
            // Pull the configuration for comfort
            var configuration = picture.Pictures.Configuration;

            // Find the trivial theorems based on whether we should do it only
            // for the last object of the configuration
            var trivialTheorems = _settings.FindTrivialTheoremsOnlyForLastObject
                // If yes, do so
                ? _producer.InferTrivialTheoremsFromObject(configuration.ConstructedObjects.Last())
                // Otherwise do it for all objects
                : configuration.ConstructedObjects.SelectMany(_producer.InferTrivialTheoremsFromObject)
                    // And enumerate the results
                    .ToList();

            #region Proof builder initialization

            // Prepare a proof builder in case we are supposed to construct proofs
            var proofBuilder = shouldWeConstructProofs ? new TheoremProofBuilder() : null;

            // Mark trivial theorems to the proof builder in case we are supposed to construct proofs
            trivialTheorems.ForEach(theorem => proofBuilder?.AddImplication(TrivialTheorem, theorem, assumptions: Array.Empty<Theorem>()));

            // Mark assumed theorems to the proof builder in case we are supposed to construct proofs
            provenTheorems.AllObjects.ForEach(theorem => proofBuilder?.AddImplication(AssumedProven, theorem, assumptions: Array.Empty<Theorem>()));

            #endregion

            #region Theorems definable simpler

            // Prepare the list of theorems definable simpler
            var theoremsDefinableSimpler = new List<Theorem>();

            // If we are supposed to assuming them proven...
            if (_settings.AssumeThatSimplifiableTheoremsAreTrue)
            {
                // Go through unproven theorems except for trivial ones
                foreach (var theoremToProve in theoremsToProve.AllObjects.Except(trivialTheorems))
                {
                    // Find the redundant objects
                    var redundantObjects = theoremToProve.GetUnnecessaryObjects(configuration);

                    // If there are none, then it cannot be defined simpler
                    if (redundantObjects.IsEmpty())
                        continue;

                    // Otherwise add it to the list
                    theoremsDefinableSimpler.Add(theoremToProve);

                    // And make sure the proof builder knows it
                    proofBuilder?.AddImplication(new DefinableSimplerInferenceData(redundantObjects), theoremToProve, assumptions: Array.Empty<Theorem>());
                }
            }

            #endregion

            #region Theorems inferable from symmetry

            // Prepare the set of theorems inferred from symmetry
            var theoremsInferredFromSymmetry = new List<Theorem>();

            // Go throw the theorems that are already inferred, i.e. assumed proven, trivial and definable simpler ones
            foreach (var provedTheorem in provenTheorems.AllObjects.Concat(trivialTheorems).Concat(theoremsDefinableSimpler))
            {
                // Try to infer new theorems using this one
                foreach (var inferredTheorem in provedTheorem.InferTheoremsFromSymmetry(configuration))
                {
                    // Add it to the list
                    theoremsInferredFromSymmetry.Add(inferredTheorem);

                    // Make sure the proof builds knows it
                    proofBuilder?.AddImplication(InferableFromSymmetry, inferredTheorem, assumptions: new[] { provedTheorem });
                }
            }

            #endregion

            #region Normalization helper initialization

            // Initially we are going to assume that the proved theorems are the passed ones
            var provedTheorems = provenTheorems.AllObjects
                // And trivial ones
                .Concat(trivialTheorems)
                // And ones with redundant objects
                .Concat(theoremsDefinableSimpler)
                // And ones inferred from symmetry
                .Concat(theoremsInferredFromSymmetry)
                // Distinct ones
                .Distinct();

            // The theorems to prove will be the new ones except for the proved ones
            var currentTheoremsToBeProven = theoremsToProve.AllObjects.Except(provedTheorems).ToArray();

            // Prepare the cloned pictures that will be used to numerically verify new theorems
            var clonedPictures = picture.Pictures.Clone();

            // Prepare a normalization helper with all this information
            var normalizationHelper = new NormalizationHelper(_verifier, clonedPictures, provedTheorems, currentTheoremsToBeProven);

            #endregion

            #region Scheduler initialization

            // Prepare a scheduler
            var scheduler = new Scheduler(_manager);

            // Do the initial scheduling
            scheduler.PerformInitialScheduling(currentTheoremsToBeProven, configuration);

            #endregion

            // Prepare the object introduction helper
            var objectIntroductionHelper = new ObjectIntroductionHelper(_introducer, normalizationHelper);

            #region Inference loop

            // Do until break
            while (true)
            {
                // Ask the scheduler for the next inference data to be used
                var data = scheduler.NextScheduledData();

                #region Object introduction

                // If there is no data, we will try to introduce objects
                if (data == null)
                {
                    // Call the introduction helper
                    var (removedObjects, introducedObject) = objectIntroductionHelper.IntroduceObject();

                    // Invalidate removed objects
                    removedObjects.ForEach(scheduler.InvalidateObject);

                    // If there is something to be introduced
                    if (introducedObject != null)
                    {
                        // Call the appropriate method to handle introduced objects
                        HandleNewObject(introducedObject, normalizationHelper, scheduler, proofBuilder);

                        // Ask the scheduler for the next inference data to be used
                        data = scheduler.NextScheduledData();
                    }
                }

                #endregion

                // If all theorems are proven or there is no data even after object introduction, we're done
                if (!normalizationHelper.AnythingLeftToProve || data == null)
                    // If we should construct proofs
                    return shouldWeConstructProofs
                        // Build them for the theorems to be proven
                        ? (dynamic)proofBuilder.BuildProofs(theoremsToProve.AllObjects)
                        // Otherwise just take the theorems to be proven that happen to be proven
                        : theoremsToProve.AllObjects.Where(normalizationHelper.ProvedTheorems.Contains).ToReadOnlyHashSet();

                #region Inference rule applier call

                // Try to apply the current scheduled data
                var applierResults = _applier.InferTheorems(new InferenceRuleApplierInput
                    (
                        // Pass the data provided by the scheduler
                        inferenceRule: data.InferenceRule,
                        premappedAssumption: data.PremappedAssumption,
                        premappedConclusion: data.PremappedConclusion,
                        premappedObject: data.PremappedObject,

                        // Pass the methods that the normalization helper offers
                        mappableTheoremsFactory: normalizationHelper.GetProvedTheoremOfType,
                        mappableObjectsFactory: normalizationHelper.GetObjectsWithConstruction,
                        equalObjectsFactory: normalizationHelper.GetEqualObjects,
                        normalizationFunction: normalizationHelper.GetNormalVersionOfObjectOrNull
                    ))
                    // Enumerate results. This step is needed because the applier could iterate over the 
                    // collections of objects and theorems used by the normalization helper 
                    .ToArray();

                // Before handling results prepare a variable that will indicate whether there has been any change of the
                // normal version of an object. 
                var anyNormalVersionChange = false;

                // Handle every inferred theorems
                foreach (var (theorem, negativeAssumptions, possitiveAssumptions) in applierResults)
                {
                    // If in some previous iteration there has been a change of the normal version of an object, then
                    // it might happen that some other theorems inferred in this loop no longer contain only correct objects,
                    // therefore we need to verify them. The reason why we don't have to worry about incorrect objects in other
                    // cases is that the normalization helper keeps everything normalized and the inference rule applier provides
                    // only normalized objects. However, if there is a change of normal versions and the applier is already called
                    // and the results are enumerated, then we have to check it manually
                    if (anyNormalVersionChange && normalizationHelper.DoesTheoremContainAnyIncorrectObject(theorem))
                        continue;

                    // We need to check negative assumptions. The inference should be accepted only if all of them are false
                    if (negativeAssumptions.Any(negativeAssumption => _verifier.IsTrueInAllPictures(clonedPictures, negativeAssumption)))
                        continue;

                    // Prepare the proof data in case we need to construct proofs
                    var proofData = shouldWeConstructProofs ? new ProofData(proofBuilder, new CustomInferenceData(data.InferenceRule), possitiveAssumptions) : null;

                    // Prepare the variable indicating whether the theorem is geometrically valid
                    bool isValid;

                    // If this is an equality
                    if (theorem.Type == EqualObjects)
                    {
                        // Call the appropriate method to handle it while finding out whether there has been any normal version change
                        HandleEquality(theorem, normalizationHelper, scheduler, proofData, out isValid, out var anyNormalVersionChangeInThisIteration);

                        // If yes, then we set the outer loop variable indicating the same thing for the whole loop
                        if (anyNormalVersionChangeInThisIteration)
                            anyNormalVersionChange = true;
                    }
                    // If this is a non-equality
                    else
                        // Call the appropriate method to handle it
                        HandleNonequality(theorem, normalizationHelper, scheduler, proofData, out isValid);

                    // If the theorem turns out not to be geometrically valid, trace it
                    if (!isValid)
                        _tracer.MarkInvalidInferrence(configuration, theorem, data.InferenceRule, negativeAssumptions, possitiveAssumptions);
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Handles a new object by commuting it with the scheduler and finding its trivial theorems and passing those to be handled
        /// by the normalization helper and scheduler.
        /// </summary>
        /// <param name="newObject">The new object to be handled.</param>
        /// <param name="helper">The normalization helper used later for the trivial theorems of the object.</param>
        /// <param name="scheduler">The scheduler of inference rules used for the new object and later for its trivial theorems.</param>
        /// <param name="builder">Either the builder to build theorem proofs; or null, if we are not constructing proofs.</param>
        private void HandleNewObject(ConstructedConfigurationObject newObject, NormalizationHelper helper, Scheduler scheduler, TheoremProofBuilder builder)
        {
            // Schedule after finding this object
            scheduler.ScheduleAfterDiscoveringObject(newObject);

            // Look for its trivial theorems
            foreach (var trivialTheorem in _producer.InferTrivialTheoremsFromObject(newObject))
            {
                // Prepare the proof data in case we need to construct proofs
                var proofData = builder != null ? new ProofData(builder, new TheoremInferenceData(TrivialTheorem), assumptions: Array.Empty<Theorem>()) : null;

                // Let the other method handle this theorem, while ignoring whether it is geometrically valid (it just should be)
                HandleNonequality(trivialTheorem, helper, scheduler, proofData, out var _);
            }
        }

        /// <summary>
        /// Handles an inferred theorem by communicating it with the normalization helper and scheduler, and also handling proof 
        /// construction is the proof data is provided.
        /// </summary>
        /// <param name="theorem">The theorem to be handled.</param>
        /// <param name="helper">The normalization helper that verifies and normalizes the theorem.</param>
        /// <param name="scheduler">The scheduler of inference rules used for the theorem if it is correct.</param>
        /// <param name="proofData">Either the data needed to mark the theorem's inference in case it's correct; or null, if we are not constructing proofs.</param>
        /// <param name="isValid">Indicates whether the theorem has been found geometrically valid.</param>
        private void HandleNonequality(Theorem theorem, NormalizationHelper helper, Scheduler scheduler, ProofData proofData, out bool isValid)
        {
            // Mark the theorem to the helper
            helper.MarkProvedNonequality(theorem, out var isNew, out isValid, out var normalizedTheorem, out var equalities);

            // If it turns out not new or valid, we're done
            if (!isNew || !isValid)
                return;

            #region Handle proof construction

            // Mark the original theorem
            proofData?.ProofBuilder.AddImplication(proofData.InferenceData, theorem, proofData.Assumptions);

            // If any normalization happened
            if (equalities.Any())
                // Mark the normalized theorem too
                proofData?.ProofBuilder.AddImplication(ReformulatedTheorem, normalizedTheorem, assumptions: equalities.Concat(theorem).ToArray());

            #endregion

            // Let the scheduler know
            scheduler.ScheduleAfterProving(normalizedTheorem);

            #region Inference from symmetry

            // If the theorem use an object that is not part of the original configuration,
            // then we will not do any symmetry inference. If it could prove a new theorem,
            // then this new theorem would be inferable conventionally
            if (!normalizedTheorem.GetInnerConfigurationObjects().All(helper.Configuration.AllObjects.Contains))
                return;

            // Otherwise try to infer new theorems using this one
            foreach (var inferredTheorem in normalizedTheorem.InferTheoremsFromSymmetry(helper.Configuration))
            {
                // If it can be done, make sure the proof builder knows it
                proofData?.ProofBuilder.AddImplication(InferableFromSymmetry, inferredTheorem, assumptions: new[] { normalizedTheorem });

                // Call this method to handle this new inferred theorem, without caring if it is valid (it should be logically)
                HandleNonequality(inferredTheorem, helper, scheduler, proofData, isValid: out var _);
            }

            #endregion
        }

        /// <summary>
        /// Handles an inferred equality theorem by communicating it with the normalization helper and scheduler, and also handling proof 
        /// construction is the proof data is provided. 
        /// </summary>
        /// <param name="equality">The equality theorem to be handled.</param>
        /// <param name="helper">The normalization helper that verifies and normalizes the theorem.</param>
        /// <param name="scheduler">The scheduler of inference rules used for the new objects and theorems this equality might have brought.</param>
        /// <param name="proofData">Either the data needed to mark the theorem's inference in case it's correct; or null, if we are not constructing proofs.</param>
        /// <param name="isValid">Indicates whether the theorem has been found geometrically valid.</param>
        /// <param name="anyChangeOfNormalVersion">Indicates whether this equality caused any change of the normal version of some object.</param>
        private void HandleEquality(Theorem equality, NormalizationHelper helper, Scheduler scheduler, ProofData proofData, out bool isValid, out bool anyChangeOfNormalVersion)
        {
            // Mark the equality to the helper
            helper.MarkProvedEquality(equality, out var isNew, out isValid, out var result);

            // If it turns out not new or valid, we're done
            if (!isNew || !isValid)
            {
                // No removed objects
                anyChangeOfNormalVersion = false;

                // We're done
                return;
            }

            // If we should construct proof, mark the inference to the proof builder
            proofData?.ProofBuilder.AddImplication(proofData.InferenceData, equality, proofData.Assumptions);

            #region Handling new normalized theorems

            // Go through all of them
            foreach (var (originalTheorem, equalities, normalizedTheorem) in result.NormalizedNewTheorems)
            {
                // If we should construct proof, mark the normalized theorem to the proof builder
                proofData?.ProofBuilder.AddImplication(ReformulatedTheorem, normalizedTheorem, assumptions: equalities.Concat(originalTheorem).ToArray());

                // Let the scheduler know about the new normalized theorem
                scheduler.ScheduleAfterProving(normalizedTheorem);
            }

            #endregion

            // Invalidate theorems
            result.DismissedTheorems.ForEach(scheduler.InvalidateTheorem);

            // Invalidate removed objects
            result.RemovedObjects.ForEach(scheduler.InvalidateObject);

            // Handle changed objects
            result.ChangedObjects.ForEach(changedObject =>
            {
                // First we will invalidate them
                scheduler.InvalidateObject(changedObject);

                // And now schedule for them again as if they were knew because now the results of schedules might be different
                scheduler.ScheduleAfterDiscoveringObject(changedObject);
            });

            // Handle entirely new objects 
            result.NewObjects.ForEach(newObject => HandleNewObject(newObject, helper, scheduler, proofData?.ProofBuilder));

            // Set if there has been any change of normal version, which is indicated by removing 
            // an object or changing its normal version
            anyChangeOfNormalVersion = result.RemovedObjects.Any() || result.ChangedObjects.Any();
        }

        #endregion
    }
}