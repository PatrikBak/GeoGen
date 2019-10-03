using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.ConfigurationObjectType;
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
            var derivationHelper = new TheoremDerivationHelper(mainTheorems: input.NewTheorems);

            // Get the analyzed configuration for comfort
            var configuration = input.ContextualPicture.Pictures.Configuration;

            #region Basic derivations

            // Add old theorems as not interesting ones
            foreach (var theorem in input.OldTheorems.AllObjects)
                derivationHelper.AddDerivation(theorem, TrueInPreviousConfiguration, Array.Empty<Theorem>());

            // Add theorems definable simpler as not interesting ones
            foreach (var theorem in input.NewTheorems.AllObjects)
            {
                // For a theorem find unnecessary objects
                var unnecessaryObjects = theorem.GetUnnecessaryObjects();

                // If there are any...
                if (unnecessaryObjects.Any())
                    // Add the derivation
                    derivationHelper.AddDerivation(theorem, new DefinableSimplerDerivationData(unnecessaryObjects), Array.Empty<Theorem>());
            }

            // Add all trivial theorems as not interesting ones
            foreach (var theorem in _trivialTheoremProducer.DeriveTrivialTheoremsFromLastObject(configuration))
                derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>());

            // Use all derivers to make relations between all theorems
            foreach (var deriver in _derivers)
                foreach (var (assumptions, derivedTheorem) in deriver.DeriveTheorems(input.AllTheorems))
                    derivationHelper.AddDerivation(derivedTheorem, deriver.Rule, assumptions);

            #endregion

            #region Subtheorem derivation

            // Prepare a tracker of equalities and incidences
            var equalityAndIncidenceTracker = new EqualityAndIncidenceTracker();

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
                    // Deconstruct
                    var (templateConfiguration, templateTheorems) = template;

                    // If there are no main theorems to prove, we don't need to continue
                    if (!derivationHelper.AnyMainTheoremLeftToProve())
                        break;

                    // If we have used this template, we can skip it
                    if (usedTemplates.Contains(template))
                        continue;

                    // If these doesn't yield special Incidence and EqualObjects theorems
                    if (!templateTheorems.ContainsKey(Incidence) && !templateTheorems.ContainsKey(EqualObjects)
                        // And there is no chance we will prove some theorem
                        && !templateTheorems.Keys.Any(derivationHelper.AnyTheoremLeftToProveOfType))
                        // Then we may skip the template
                        continue;

                    // Otherwise we're going to do the derivation
                    // Mark that we've used the template
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
                            #region Handling equalities

                            // If this theorem is an equal objects theorem, mark it
                            if (derivedTheorem.Type == EqualObjects)
                                equalityAndIncidenceTracker.MarkEquality(derivedTheorem);

                            // Prepare the used equality theorems
                            var usedEqualities = output.UsedEqualities
                                // Convert each equality to a theorem
                                .Select(equality => new Theorem(configuration, EqualObjects, equality.originalObject, equality.equalObject));

                            // Mark all of them
                            usedEqualities.ForEach(equalityAndIncidenceTracker.MarkEquality);

                            #endregion

                            #region Handling incidences

                            // If this theorem is an incidence theorem, mark it
                            if (derivedTheorem.Type == Incidence)
                                equalityAndIncidenceTracker.MarkIncidence(derivedTheorem);

                            // Prepare the used incidences theorems
                            var usedIncidences = output.UsedIncidencies
                                // Convert each incidence to a theorem
                                .Select(incidence => new Theorem(configuration, Incidence, incidence.point, incidence.lineOrCircle));

                            // Mark all of them
                            usedIncidences.ForEach(equalityAndIncidenceTracker.MarkIncidence);

                            #endregion

                            // Prepare the needed assumptions by merging the used acts, equalities and incidences
                            var neededAsumptions = output.UsedFacts.Concat(usedEqualities).Concat(usedIncidences).ToArray();

                            // Add the subtheorem derivation
                            derivationHelper.AddDerivation(derivedTheorem, new SubtheoremDerivationData(templateTheorem), neededAsumptions);
                        }
                    }
                }
            });

            #endregion

            #region Trivial theorems of newly discovered objects

            // Prepare a set of newly found incidences that will be merged later
            var newIncidences = new HashSet<Theorem>();

            // Get all the objects tracker by the tracker
            equalityAndIncidenceTracker.AllObjects
                // Only constructed
                .OfType<ConstructedConfigurationObject>()
                // And only new ones
                .Where(o => !configuration.ConstructedObjectsSet.Contains(o))
                // Create a temporary configuration containing each object as the last one
                .Select(o => new Configuration(configuration.LooseObjectsHolder, configuration.ConstructedObjects.Concat(o).ToList()))
                // Get trivial theorems for it
                .Select(_trivialTheoremProducer.DeriveTrivialTheoremsFromLastObject)
                // Get rid of the temporary configuration from the theorems definition
                .SelectMany(theorems => theorems.Select(theorem => new Theorem(configuration, theorem.Type, theorem.InvolvedObjects)))
                // Take distinct ones
                .Distinct()
                // Handle each theorem
                .ForEach(theorem =>
                {
                    // Add the derivation
                    derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>());

                    // If this theorem is an incidence theorem, remember it
                    if (theorem.Type == Incidence)
                        newIncidences.Add(theorem);
                });

            // Merge the new incidences with the old ones
            newIncidences.ForEach(equalityAndIncidenceTracker.MarkIncidence);

            #endregion

            #region Transitivities of equalities

            // Get all triples of equal objects and for each do the derivation
            equalityAndIncidenceTracker.EqualityGroups.SelectMany(group => group.Subsets(3)).ForEach(triple =>
            {
                // Prepare the equal objects theorems
                var theorem1 = new Theorem(configuration, EqualObjects, triple[0], triple[1]);
                var theorem2 = new Theorem(configuration, EqualObjects, triple[1], triple[2]);
                var theorem3 = new Theorem(configuration, EqualObjects, triple[2], triple[0]);

                // From every two we can derive the other
                derivationHelper.AddDerivation(theorem1, Transitivity, new[] { theorem2, theorem3 });
                derivationHelper.AddDerivation(theorem2, Transitivity, new[] { theorem3, theorem1 });
                derivationHelper.AddDerivation(theorem3, Transitivity, new[] { theorem1, theorem2 });
            });

            #endregion

            #region Redefining incidences using equalities

            // Go through every incidence
            equalityAndIncidenceTracker.Incidences.ForEach(objects =>
            {
                // Deconstruct
                var (point, lineOrCircle) = objects;

                // Get the incidence theorem
                var incidence = new Theorem(configuration, Incidence, point, lineOrCircle);

                // Get the equality groups for the points objects and take its other points
                equalityAndIncidenceTracker.FindEqualityGroup(point).Where(_point => !_point.Equals(point)).ForEach(equalPoint =>
                {
                    // Create the used equality theorem
                    var usedEquality = new Theorem(configuration, EqualObjects, point, equalPoint);

                    // Create the derived theorem
                    var otherIncidence = new Theorem(configuration, Incidence, equalPoint, lineOrCircle);

                    // Add the derivations
                    derivationHelper.AddDerivation(otherIncidence, ReformulatedTheorem, new[] { usedEquality, incidence });
                    derivationHelper.AddDerivation(incidence, ReformulatedTheorem, new[] { usedEquality, otherIncidence });
                });

                // Get the equality groups for the line or circles objects and take its other objects
                equalityAndIncidenceTracker.FindEqualityGroup(lineOrCircle).Where(_lineOrCircle => !_lineOrCircle.Equals(lineOrCircle)).ForEach(equalLineOrCircle =>
                {
                    // Create the used equality theorem
                    var usedEquality = new Theorem(configuration, EqualObjects, lineOrCircle, equalLineOrCircle);

                    // Create the derived theorem
                    var otherIncidence = new Theorem(configuration, Incidence, point, equalLineOrCircle);

                    // Add the derivation
                    derivationHelper.AddDerivation(otherIncidence, ReformulatedTheorem, new[] { usedEquality, incidence });
                    derivationHelper.AddDerivation(incidence, ReformulatedTheorem, new[] { usedEquality, otherIncidence });
                });
            });

            #endregion

            #region Incidences give collinearity and concyclity

            // Take all incidences
            equalityAndIncidenceTracker.Incidences
                // Group these incidences by the line / circle
                .GroupBy(tuple => tuple.lineOrCircle)
                // Each group represents more potential collinearities / concyclities
                // We take every possible triple / quadruple of incidences from this group,
                // based on the fact whether we're dealing with a line or circle
                .SelectMany(grouping => grouping.Subsets(grouping.First().lineOrCircle.ObjectType switch
                {
                    // Line is interesting with 3 points
                    Line => 3,

                    // Circle is interesting with 4 points
                    Circle => 4,

                    // Default case
                    _ => throw new TheoremProverException($"Unhandled type of configuration object: {grouping.First().lineOrCircle.ObjectType}")
                }))
                // Each triple represents a true collinearity (potentially degenerated)
                .ForEach(incidences =>
                {
                    // Get the points of the incidences
                    var points = incidences.Select(pair => pair.point).ToArray();

                    // Make sure none two points are equal
                    if (points.UnorderedPairs().Any(pair => equalityAndIncidenceTracker.AreEqual(pair.Item1, pair.Item2)))
                        return;

                    // Prepare the incidence theorems
                    var incidencesTheorems = incidences.Select(pair => new Theorem(configuration, Incidence, pair.lineOrCircle, pair.point)).ToArray();

                    // Switch on the type of the other object
                    switch (incidences[0].lineOrCircle.ObjectType)
                    {
                        // Line case
                        case Line:

                            // Prepare the collinearity theorem
                            var collinearity = new Theorem(configuration, CollinearPoints, points);

                            // Add the derivation of any of the theorems from the others
                            derivationHelper.AddDerivation(collinearity, IncidencesAndCollinearity, new[] { incidencesTheorems[0], incidencesTheorems[1], incidencesTheorems[2] });
                            derivationHelper.AddDerivation(incidencesTheorems[0], IncidencesAndCollinearity, new[] { collinearity, incidencesTheorems[1], incidencesTheorems[2] });
                            derivationHelper.AddDerivation(incidencesTheorems[1], IncidencesAndCollinearity, new[] { collinearity, incidencesTheorems[0], incidencesTheorems[2] });
                            derivationHelper.AddDerivation(incidencesTheorems[2], IncidencesAndCollinearity, new[] { collinearity, incidencesTheorems[0], incidencesTheorems[1] });

                            break;

                        // Circle case
                        case Circle:

                            // Prepare the concyclity theorem
                            var concyclity = new Theorem(configuration, CollinearPoints, points);

                            // Add the derivation of any of the theorems from the others
                            derivationHelper.AddDerivation(concyclity, IncidencesAndCollinearity, new[] { incidencesTheorems[0], incidencesTheorems[1], incidencesTheorems[2], incidencesTheorems[3] });
                            derivationHelper.AddDerivation(incidencesTheorems[0], IncidencesAndCollinearity, new[] { concyclity, incidencesTheorems[1], incidencesTheorems[2], incidencesTheorems[3] });
                            derivationHelper.AddDerivation(incidencesTheorems[1], IncidencesAndCollinearity, new[] { concyclity, incidencesTheorems[0], incidencesTheorems[2], incidencesTheorems[3] });
                            derivationHelper.AddDerivation(incidencesTheorems[2], IncidencesAndCollinearity, new[] { concyclity, incidencesTheorems[0], incidencesTheorems[1], incidencesTheorems[3] });
                            derivationHelper.AddDerivation(incidencesTheorems[3], IncidencesAndCollinearity, new[] { concyclity, incidencesTheorems[0], incidencesTheorems[1], incidencesTheorems[2] });

                            break;

                        // Default case
                        default:
                            throw new TheoremProverException($"Unhandled type of configuration object: {incidences[0].lineOrCircle.ObjectType}");
                    }
                });

            #endregion

            #region Reformulating theorems to trivial ones

            // Get the new theorems that are not proven
            derivationHelper.UnproveneMainTheorems().ToList().ForEach(theorem =>
            {
                // For a given one get the inner objects of the theorem
                var innerObjects = theorem.GetInnerConfigurationObjects();

                // For each find the equivalence group
                innerObjects.Select(innerObject => (innerObject, group: equalityAndIncidenceTracker.FindEqualityGroup(innerObject)))
                    // Take only objects that have some equal objects
                    .Where(pair => pair.group.Count != 1)
                    // Make all possible pairs of the object and its equal partner
                    .Select(pair => pair.group.Select(equalObject => (pair.innerObject, equalObject)))
                    // Combine these option to a single one
                    .Combine()
                    // Each option represents a reformulation
                    .ForEach(option =>
                    {
                        // Create the dictionary mapping objects to their equal versions
                        var mapping = option.ToDictionary(pair => pair.innerObject, pair => pair.equalObject);

                        // Make sure all the objects are in the mapping
                        innerObjects.Where(o => !mapping.ContainsKey(o)).ForEach(o => mapping.Add(o, o));

                        // Create the remapped theorem
                        var remappedTheorem = theorem.Remap(mapping);

                        // If this theorem is the same as the original one, we don't need to do anything
                        if (theorem.Equals(remappedTheorem))
                            return;

                        // Otherwise prepare the used theorem by taking all non-identity equality pairs
                        var assumptions = mapping.Where(pair => !pair.Key.Equals(pair.Value))
                            // Making each a theorem
                            .Select(pair => new Theorem(configuration, EqualObjects, new[] { pair.Key, pair.Value }))
                            // And appending our reformulated theorem
                            .Concat(remappedTheorem);

                        // Finally add the derivation
                        derivationHelper.AddDerivation(theorem, ReformulatedTheorem, assumptions);
                    });
            });

            #endregion

            #region Reformulating theorem and configuration to get rid of objects

            // Find all possible configuration and theorem reformulations by taking all the objects
            configuration.ConstructedObjects
                // Find the equality groups for them
                .Select(innerObject => (innerObject, group: equalityAndIncidenceTracker.FindEqualityGroup(innerObject)))
                // Take only objects that have some equal objects
                .Where(pair => pair.group.Count != 1)
                // Make all possible pairs of the object and its equal distinct partner
                .Select(pair => pair.group.Where(equalObject => !equalObject.Equals(pair.innerObject)).Select(equalObject => (pair.innerObject, equalObject)))
                // Combine these option to a single one
                .Combine()
                // Exclude the trivial one with no mapping
                .Where(option => option.Length != 0)
                // Each option represents a reformulation
                .ForEach(option =>
                {
                    // Get the mapping dictionary
                    var mapping = option.ToDictionary(pair => pair.innerObject, pair => (ConstructedConfigurationObject)pair.equalObject);

                    #region Function for mapping object

                    // Prepare the set of objects being map
                    // It serves as a prevention from cyclic mapping
                    var objectsBeingMapped = new HashSet<ConstructedConfigurationObject>();

                    // Local function to map an object. If it cannot be done
                    // (because of the cycle), returns null
                    ConfigurationObject Map(ConfigurationObject configurationObject)
                    {
                        // Switch based on the object
                        switch (configurationObject)
                        {
                            // Loose objects are mapped to themselves
                            case LooseConfigurationObject _:
                                return configurationObject;

                            // For constructed objects
                            case ConstructedConfigurationObject constructedObject:

                                // We first check if it's in the mapping, because
                                // if it's not, it should be mapped to itself
                                if (!mapping.ContainsKey(constructedObject))
                                    return constructedObject;

                                // Otherwise the object is in the mapping
                                // If it's being mapped, then we have run into a cycle
                                // and the mapping should not be successful
                                if (objectsBeingMapped.Contains(constructedObject))
                                    return null;

                                // Otherwise we're going to map the object right now
                                objectsBeingMapped.Add(constructedObject);

                                // Get the object to which we're going to map this
                                var newObject = mapping[constructedObject];

                                // Take the inner objects and recursively map them
                                var newArguments = newObject.PassedArguments.FlattenedList.SelectIfNotDefault(Map);

                                // If some of them cannot be mapped, the mapping cannot be done,
                                // and we don't even have to remove the object from the set of 
                                // ones being mapped (it is not mappable anyway)
                                if (newArguments == null)
                                    return null;

                                // If all of them are fine, we can say the mapping is done
                                objectsBeingMapped.Remove(constructedObject);

                                // And we can return the object with remapped arguments
                                return new ConstructedConfigurationObject(newObject.Construction, newArguments.ToArray());

                            // Default case
                            default:
                                throw new TheoremProverException($"Unhandled type of configuration object: {configurationObject.GetType()}");
                        }
                    }

                    #endregion

                    // Create the mapping of all objects
                    var allObjectsMapping = configuration.AllObjects.SelectIfNotDefault(configurationObject =>
                    {
                        // Map the object
                        var mappedObject = Map(configurationObject);

                        // Return either the pair of the original one and this one if it can be done,
                        // otherwise the default value
                        return mappedObject != null ? (originalObject: configurationObject, mappedObject) : default;
                    })
                    // Enumerate to a dictionary
                    ?.ToDictionary(pair => pair.originalObject, pair => pair.mappedObject);

                    // If the mapping cannot be done, we're sad
                    if (allObjectsMapping == null)
                        return;

                    // Otherwise we can create a new redefined configuration
                    var redefinedConfiguration = new Configuration(configuration.LooseObjectsHolder,
                            // Pull the constructed objects from the map
                            // It might probably happen they are in a wrong order, 
                            // but it should not mapped for this particular usage
                            allObjectsMapping.Values.OfType<ConstructedConfigurationObject>().ToArray());

                    // Get the not-proven theorems
                    derivationHelper.UnproveneMainTheorems().ToList().ForEach(theorem =>
                    {
                        // Try to reformulate the current one
                        var remappedTheorem = theorem.Remap(allObjectsMapping, redefinedConfiguration);

                        // I think it should be doable...
                        if (remappedTheorem == null)
                            throw new TheoremProverException("A theorem to be proved got remapped to an incorrect one. How is this possible???");

                        // Finally we can test if the theorem can be stated without some objects
                        var redundantObjects = remappedTheorem.GetUnnecessaryObjects();

                        // If there is none, we can't do more
                        if (redundantObjects.IsEmpty())
                            return;

                        // Otherwise prepare the used equalities by taking all non-identity pairs of the mapping
                        var usedEqualities = allObjectsMapping.Where(pair => !pair.Key.Equals(pair.Value))
                            // Making each a theorem
                            .Select(pair => new Theorem(configuration, EqualObjects, new[] { pair.Key, pair.Value }));

                        // Prepare the list of objects that are redundant in the original configuration
                        // This code doesn't look efficient, but we're dealing with very little quantities
                        var originalRedundantObjects = redundantObjects.Select(redundantObject =>
                                // Get the pair where this object is on the right and take the corresponding key
                                allObjectsMapping.First(pair => pair.Value.Equals(redundantObject)).Key)
                            // Wrap it all in a read-only set
                            .ToReadOnlyHashSet();

                        // Otherwise add the derivation
                        derivationHelper.AddDerivation(theorem, new DefinableSimplerDerivationData(originalRedundantObjects), usedEqualities);
                    });
                });

            #endregion

            // Let the helper construct the result
            return derivationHelper.ConstructResult();
        }

        #endregion
    }
}