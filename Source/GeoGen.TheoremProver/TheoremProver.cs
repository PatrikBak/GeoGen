using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;
using static GeoGen.TheoremProver.DerivationRule;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremProver"/>. This implementation combines
    /// <see cref="ITrivialTheoremProducer"/>, <see cref="ISubtheoremDeriver"/>, 
    /// and various <see cref="ITheoremDeriver"/>s. Most of the complicated algorithmic work
    /// is delegated to a helper class <see cref="TheoremDerivationHelper"/>.
    /// </summary>
    public class TheoremProver : ITheoremProver
    {
        #region Dependencies

        /// <summary>
        /// The theorem derivers based on logical rules applied on the existing theorems.
        /// </summary>
        private readonly ITheoremDeriver[] _derivers;

        /// <summary>
        /// The producer of trivial theorems.
        /// </summary>
        private readonly ITrivialTheoremProducer _trivialTheoremProducer;

        /// <summary>
        /// The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.
        /// </summary>
        private readonly ISubtheoremDeriver _subtheoremDeriver;

        #endregion

        #region Private fields

        /// <summary>
        /// The data for the prover.
        /// </summary>
        private readonly TheoremProverData _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProver"/> class.
        /// </summary>
        /// <param name="data">The data for the prover.</param>
        /// <param name="derivers">The theorem derivers based on logical rules applied on the existing theorems.</param>
        /// <param name="trivialTheoremProducer">The producer of trivial theorems.</param>
        /// <param name="subtheoremDeriver">The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.</param>
        public TheoremProver(TheoremProverData data,
                             ITheoremDeriver[] derivers,
                             ITrivialTheoremProducer trivialTheoremProducer,
                             ISubtheoremDeriver subtheoremDeriver)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _derivers = derivers ?? throw new ArgumentNullException(nameof(derivers));
            _trivialTheoremProducer = trivialTheoremProducer ?? throw new ArgumentNullException(nameof(trivialTheoremProducer));
            _subtheoremDeriver = subtheoremDeriver ?? throw new ArgumentNullException(nameof(subtheoremDeriver));
        }

        #endregion

        #region ITheoremProver implementation

        /// <summary>
        /// Performs the analysis for a given input.
        /// </summary>
        /// <param name="input">The input for the analyzer.</param>
        /// <returns>The output of the analysis.</returns>
        public TheoremProverOutput Analyze(TheoremProverInput input)
        {
            // Initialize a theorem derivation helper that will do the proving. 
            // We want it to prove the new theorems
            var derivationHelper = new TheoremDerivationHelper(theorems: input.NewTheorems);

            // Prepare a tracker of equalities and incidences
            var equalityAndIncidenceTracker = new EqualityAndIncidenceTracker();

            // Get the analyzed configuration for comfort
            var configuration = input.ContextualPicture.Pictures.Configuration;

            // Add the listener for proved theorems
            derivationHelper.TheoremProven += theorem =>
            {
                // Switch based on the theorem type
                switch (theorem.Type)
                {
                    // If we have new equal objects...
                    case EqualObjects:
                    {
                        #region Trivial theorems

                        // Get the objects
                        var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
                        var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

                        // Enumerate them for comfort
                        object1.ToEnumerable().Concat(object2)
                            // Only constructed
                            .OfType<ConstructedConfigurationObject>()
                            // For each of them
                            .ForEach(constructedObject =>
                            {
                                // Get the trivial theorems
                                var trivialTheorems = _trivialTheoremProducer.DeriveTrivialTheoremsFromObject(constructedObject);

                                // Mark them
                                trivialTheorems.ForEach(theorem => derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>()));

                                // Get the other object
                                var otherObject = object1 == constructedObject ? object2 : object1;

                                // Go through the trivial theorems
                                trivialTheorems.ForEach(trivialTheorem =>
                                {
                                    // We're going use this other object to remap the theorem 
                                    // We need to get all the inner objects first mapped to itself
                                    var mapping = trivialTheorem.GetInnerConfigurationObjects().ToDictionary(o => o, o => o);

                                    // Replace our object with the other equal object
                                    mapping[constructedObject] = otherObject;

                                    // Do the remapping
                                    var remappedTheorem = trivialTheorem.Remap(mapping);

                                    // In case we have something degenerated, do nothing
                                    // TODO: Log it. I wanna see...
                                    if (remappedTheorem == null)
                                        return;

                                    // Add the derivation
                                    derivationHelper.AddDerivation(remappedTheorem, ReformulatedTheorem, new[] { trivialTheorem, theorem });
                                });
                            });

                        #endregion

                        // Mark the proven equality to the tracker
                        equalityAndIncidenceTracker.MarkEquality(theorem);

                        break;
                    }

                    case Incidence:
                    {
                        #region Mark sure trivial theorems of both objects are registered

                        // Get the objects
                        var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
                        var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

                        // Enumerate them for comfort
                        object1.ToEnumerable().Concat(object2)
                            // Only constructed
                            .OfType<ConstructedConfigurationObject>()
                            // Take them
                            .SelectMany(_trivialTheoremProducer.DeriveTrivialTheoremsFromObject)
                            // Mark them
                            .ForEach(theorem => derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>()));

                        #endregion

                        #region Using incidences to prove collinearity / concyclity

                        // Go through the unproven ones
                        derivationHelper.UnprovenTheoremsOfTypes(CollinearPoints, ConcyclicPoints)
                            // Enumerate because they might change by proving
                            .ToList()
                            // Try to prove each of them
                            .ForEach(theoremToProve =>
                            {
                                // Get the points
                                var points = theoremToProve.GetInnerConfigurationObjects();

                                // For each point
                                var lineOrCircle = points
                                    // Try to find the line or circle where each point lies
                                    .Select(equalityAndIncidenceTracker.FindLinesOrCircles)
                                    // Find the intersection of all these sets
                                    .Aggregate((intersection, current) => intersection.Intersect(current).ToSet())
                                    // Take the first common element or null
                                    .FirstOrDefault();

                                // If there is no common line or circle, we can't do much
                                if (lineOrCircle == null)
                                    return;

                                // Otherwise we have it! Prepare the incidences
                                var incidences = points.Select(point => new Theorem(Incidence, point, lineOrCircle)).ToArray();

                                // If we're proving collinearity...
                                if (theoremToProve.Type == CollinearPoints)
                                    derivationHelper.AddDerivation(theoremToProve, IncidencesAndCollinearity, incidences);
                                // Otherwise we're proving concyclities
                                else
                                    derivationHelper.AddDerivation(theoremToProve, IncidencesAndConcyclity, incidences);
                            });

                        #endregion

                        // Mark the proven incidence to the tracker
                        equalityAndIncidenceTracker.MarkIncidence(theorem);

                        break;
                    }
                }
            };

            // Add the listener for newly derived equalities
            equalityAndIncidenceTracker.NewEqualities += derivations =>
            {
                // Add the derivation for all of them
                derivations.ForEach(pair => derivationHelper.AddDerivation(pair.derivedEquality, Transitivity, pair.usedEqualities));
            };

            // Add the listener for newly derived incidences
            equalityAndIncidenceTracker.NewIncidences += derivations =>
            {
                // Add the derivation for all of them
                derivations.ForEach(triple => derivationHelper.AddDerivation(triple.newIncidence, ReformulatedTheorem,
                    // The assumptions are the equalities and the old incidence
                    triple.usedEqualities.Concat(triple.oldIncidence)));
            };

            #region Basic derivations

            // Add old theorems as not interesting ones
            foreach (var theorem in input.OldTheorems.AllObjects)
                derivationHelper.AddDerivation(theorem, TrueInPreviousConfiguration, Array.Empty<Theorem>());

            // Add all trivial theorems as not interesting ones
            foreach (var theorem in _trivialTheoremProducer.DeriveTrivialTheoremsFromObject(configuration.LastConstructedObject))
                derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>());

            // Add theorems definable simpler as not interesting ones
            foreach (var theorem in input.NewTheorems.AllObjects)
            {
                // For a theorem find unnecessary objects
                var unnecessaryObjects = theorem.GetUnnecessaryObjects(configuration);

                // If there are any...
                if (unnecessaryObjects.Any())
                    // Add the derivation
                    derivationHelper.AddDerivation(theorem, new DefinableSimplerDerivationData(unnecessaryObjects), Array.Empty<Theorem>());
            }

            // Use all derivers to make relations between all theorems
            foreach (var deriver in _derivers)
                foreach (var (assumptions, derivedTheorem) in deriver.DeriveTheorems(input.AllTheorems))
                    derivationHelper.AddDerivation(derivedTheorem, deriver.Rule, assumptions);

            #endregion

            #region Subtheorem derivation

            // Prepare a set of used template configurations
            var usedTemplates = new HashSet<(Configuration, TheoremMap)>();

            // We're going to execute the subtheorems algorithm in two phases
            // In the first one we try to derive our theorems that we're interested
            // in, and potentially some equalities / incidences. In the second run
            // we then only look for deriving equalities / incidences, because 
            // potential template theorems might have been skipped before we even
            // knew about them.
            GeneralUtilities.ExecuteNTimes(numberOfTimes: 2, action: () =>
            {
                // Go through the template theorems
                foreach (var template in _data.TemplateTheorems)
                {
                    // If we have used this template, we can skip it
                    if (usedTemplates.Contains(template))
                        continue;

                    // Deconstruct
                    var (templateConfiguration, templateTheorems) = template;

                    // If there are no main theorems to prove, we don't need to continue
                    if (!derivationHelper.AnyTheoremLeftToProve())
                        break;

                    // If these doesn't yield special Incidence and EqualObjects theorems
                    if (!templateTheorems.ContainsKey(Incidence) && !templateTheorems.ContainsKey(EqualObjects)
                        // And there is no chance we will prove some theorem
                        && !templateTheorems.Keys.Any(derivationHelper.AnyTheoremLeftToProveOfType))
                        // Then we may skip the template
                        continue;

                    // Otherwise we're going to do the derivation
                    usedTemplates.Add(template);

                    // Call the subtheorem algorithm
                    var outputs = _subtheoremDeriver.DeriveTheorems(new SubtheoremDeriverInput
                    (
                        examinedConfigurationPicture: input.ContextualPicture,
                        examinedConfigurationTheorems: input.AllTheorems,
                        templateConfiguration: templateConfiguration,
                        templateTheorems: templateTheorems
                    ));

                    // Go through its outputs
                    foreach (var output in outputs)
                    {
                        // Go through the theorems it derived
                        foreach (var (derivedTheorem, templateTheorem) in output.DerivedTheorems)
                        {
                            // Prepare the used equality theorems
                            var usedEqualities = output.UsedEqualities
                                // Convert each equality to a theorem
                                .Select(equality => new Theorem(EqualObjects, equality.originalObject, equality.equalObject));

                            // Prepare the used incidences theorems
                            var usedIncidences = output.UsedIncidencies
                                // Convert each incidence to a theorem
                                .Select(incidence => new Theorem(Incidence, incidence.point, incidence.lineOrCircle));

                            // Prepare the needed assumptions by merging the used acts, equalities and incidences
                            var neededAsumptions = output.UsedFacts.Concat(usedEqualities).Concat(usedIncidences).ToArray();

                            // Add the subtheorem derivation
                            derivationHelper.AddDerivation(derivedTheorem, new SubtheoremDerivationData(templateTheorem), neededAsumptions);
                        }
                    }
                }
            });

            #endregion

            // Let the helper construct the result
            return derivationHelper.ConstructResult();
        }

        #endregion
    }
}