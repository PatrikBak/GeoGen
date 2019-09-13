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
    /// <see cref="ITrivialTheoremsProducer"/>, <see cref="ISubtheoremsDeriver"/>, 
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
        private readonly ITrivialTheoremsProducer _trivialTheoremsProducer;

        /// <summary>
        /// The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.
        /// </summary>
        private readonly ISubtheoremsDeriver _subtheoremsDeriver;

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
        /// <param name="trivialTheoremsProducer">The producer of trivial theorems.</param>
        /// <param name="subtheoremsDeriver">The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.</param>
        public TheoremProver(TheoremProverData data,
                             ITheoremDeriver[] derivers,
                             ITrivialTheoremsProducer trivialTheoremsProducer,
                             ISubtheoremsDeriver subtheoremsDeriver)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _derivers = derivers ?? throw new ArgumentNullException(nameof(derivers));
            _trivialTheoremsProducer = trivialTheoremsProducer ?? throw new ArgumentNullException(nameof(trivialTheoremsProducer));
            _subtheoremsDeriver = subtheoremsDeriver ?? throw new ArgumentNullException(nameof(subtheoremsDeriver));
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
                if (theorem.CanBeStatedInSmallerConfiguration())
                    derivationHelper.AddDerivation(theorem, DefinableSimpler, Array.Empty<Theorem>());

            // Add all trivial theorems as not interesting ones
            foreach (var theorem in _trivialTheoremsProducer.DeriveTrivialTheoremsFromLastObject(configuration))
                derivationHelper.AddDerivation(theorem, TrivialTheorem, Array.Empty<Theorem>());

            // Use all derivers to make relations between all theorems
            foreach (var deriver in _derivers)
                foreach (var (assumptions, derivedTheorem) in deriver.DeriveTheorems(input.AllTheorems))
                    derivationHelper.AddDerivation(derivedTheorem, deriver.Rule, assumptions);

            #endregion

            #region Subtheorems derivation

            // Prepare a set of object equalities
            var equalObjects = new HashSet<(ConfigurationObject, ConfigurationObject)>();

            // Prepare a set of found incidences
            var incidences = new HashSet<(ConfigurationObject, ConfigurationObject)>();

            // Prepare a set of used template configurations
            var usedTemplates = new HashSet<(Configuration, TheoremsMap)>();

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

                    // If there is no chance that we will prove some unproved theorem
                    if (!templateTheorems.Keys.Any(derivationHelper.AnyTheoremLeftToProveOfType)
                        // or we have already tried this template 
                        || usedTemplates.Contains(template))
                        // we may skip the derivation (we wouldn't get anything useful)                   
                        continue;

                    // Otherwise we're going to do the derivation
                    // Mark that we've used the template
                    usedTemplates.Add(template);

                    // Call the subtheorem algorithm
                    var outputs = _subtheoremsDeriver.DeriveTheorems(new SubtheoremsDeriverInput
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
                            // Add the found equalities to our set of all equalities
                            equalObjects.UnionWith(output.UsedEqualities);

                            // Add the found incidences to our set of all incidences
                            incidences.UnionWith(output.UsedIncidencies);

                            // Prepare the used equality theorems
                            var usedEqualities = output.UsedEqualities
                                // Convert each equality to a theorem
                                .Select(equality => new Theorem(configuration, EqualObjects, equality.originalObject, equality.equalObject));

                            // Prepare the used incidences theorems
                            var usedIncidences = output.UsedIncidencies
                                // Convert each incidence to a theorem
                                .Select(incidence => new Theorem(configuration, Incidence, incidence.point, incidence.lineOrCircle));

                            // Prepare the needed assumptions by merging the used acts, equalities and incidences
                            var neededAsumptions = output.UsedFacts.Concat(usedEqualities).Concat(usedIncidences).ToArray();

                            // Add the subtheorem derivation
                            derivationHelper.AddSubtheoremDerivation(derivedTheorem, templateTheorem, neededAsumptions);
                        }
                    }
                }
            });

            #endregion

            #region Transitivities based on discovered object equalities

            // From the found object equalities create groups of mutually equal objects
            var objectEqualityGroups = equalObjects.CreateEqualityClasses();

            // Go through each triples of each equality group
            objectEqualityGroups.ForEach(group => group.Subsets(3).ForEach(triple =>
            {
                // Prepare the equal objects theorems
                var theorem1 = new Theorem(configuration, EqualObjects, triple[0], triple[1]);
                var theorem2 = new Theorem(configuration, EqualObjects, triple[1], triple[2]);
                var theorem3 = new Theorem(configuration, EqualObjects, triple[2], triple[0]);

                // From every two we can derive the other
                derivationHelper.AddDerivation(theorem1, Transitivity, new[] { theorem2, theorem3 });
                derivationHelper.AddDerivation(theorem2, Transitivity, new[] { theorem3, theorem1 });
                derivationHelper.AddDerivation(theorem3, Transitivity, new[] { theorem1, theorem2 });
            }));

            // Go through every incidence
            incidences.ForEach(incidence =>
            {
                // Deconstruct
                var (object1, object2) = incidence;

                // Get the equality groups for these objects, or a one-element sets, if there is none
                var group1 = objectEqualityGroups.FirstOrDefault(group => group.Contains(object1)) ?? new HashSet<ConfigurationObject> { object1 };
                var group2 = objectEqualityGroups.FirstOrDefault(group => group.Contains(object2)) ?? new HashSet<ConfigurationObject> { object2 };

                // Combine elements from these groups 
                group1.CombinedWith(group2).ForEach(pair =>
                {
                    // Deconstruct
                    var (group1Object, group2Object) = pair;

                    // To create an incidence theorem
                    var newIncidence = new Theorem(configuration, Incidence, group1Object, group2Object);

                    // Use every equality in the first group, except for the identity
                    group1.Where(o => !o.Equals(group1Object)).ForEach(otherGroup1Object =>
                    {
                        // Create the used equality theorem
                        var usedEquality = new Theorem(configuration, EqualObjects, group1Object, otherGroup1Object);

                        // Create the derived theorem
                        var derivedTheorem = new Theorem(configuration, Incidence, otherGroup1Object, group2Object);

                        // Add the derivation
                        derivationHelper.AddDerivation(derivedTheorem, Transitivity, new[] { usedEquality, newIncidence });
                    });

                    // Use every equality in the second group, except for the identity
                    group2.Where(o => !o.Equals(group2Object)).ForEach(otherGroup2Object =>
                    {
                        // Create the used equality theorem
                        var usedEquality = new Theorem(configuration, EqualObjects, group2Object, otherGroup2Object);

                        // Create the derived theorem
                        var derivedTheorem = new Theorem(configuration, Incidence, otherGroup2Object, group1Object);

                        // Add the derivation
                        derivationHelper.AddDerivation(derivedTheorem, Transitivity, new[] { usedEquality, newIncidence });
                    });
                });
            });

            #endregion

            // Let the helper construct the result
            return derivationHelper.ConstructResult();
        }

        #endregion
    }
}