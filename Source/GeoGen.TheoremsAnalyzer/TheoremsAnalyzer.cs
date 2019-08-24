using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremsAnalyzer"/>. The main goal of this implementation
    /// is to find problems as close to IMO problems as possible. Hence we consider only the following types:
    /// <para>
    /// 1. Collinear points. <br></br>
    /// 2. Concyclic points. <br></br>
    /// 3. Concurrent lines. <br></br>
    /// 4. Perpendicular lines. <br></br>
    /// 5. Parallel lines. <br></br>
    /// 6. Line tangent to circle at an undefined point. <br></br>
    /// 7. Tangent circles at an undefined point. <br></br><br></br>
    /// </para>
    /// Other theorems, such as EqualAngles or ConcurrentObjects (where we might intersect circles as well)
    /// don't seem very olympiad, therefore all attempts to understand theorems will ignore them.
    /// </summary>
    public class TheoremsAnalyzer : ITheoremsAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The producer of trivial theorems.
        /// </summary>
        private readonly ITrivialTheoremsProducer _trivialTheoremsProducer;

        /// <summary>
        /// The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.
        /// </summary>
        private readonly ISubtheoremsDeriver _subtheoremsDeriver;

        /// <summary>
        /// The deriver of new theorems based on the transitivity rule.
        /// </summary>
        private readonly ITransitivityDeriver _transitivityDeriver;

        #endregion

        #region Private fields

        /// <summary>
        /// The data for the analyzer.
        /// </summary>
        private readonly TheoremsAnalyzerData _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="data">The data for the analyzer.</param>
        /// <param name="trivialTheoremsProducer">The producer of trivial theorems.</param>
        /// <param name="subtheoremsDeriver">The sub-theorems deriver. It gets template theorems from the <see cref="_data"/>.</param>
        /// <param name="transitivityDeriver">The deriver of new theorems based on the transitivity rule.</param>
        public TheoremsAnalyzer(TheoremsAnalyzerData data, ITrivialTheoremsProducer trivialTheoremsProducer, ISubtheoremsDeriver subtheoremsDeriver, ITransitivityDeriver transitivityDeriver)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _trivialTheoremsProducer = trivialTheoremsProducer ?? throw new ArgumentNullException(nameof(trivialTheoremsProducer));
            _subtheoremsDeriver = subtheoremsDeriver ?? throw new ArgumentNullException(nameof(subtheoremsDeriver));
            _transitivityDeriver = transitivityDeriver ?? throw new ArgumentNullException(nameof(transitivityDeriver));
        }

        #endregion

        #region ITheoremAnalyzer implementation

        /// <summary>
        /// Performs the analysis for given input.
        /// </summary>
        /// <param name="input">The input for the analyzer.</param>
        /// <returns>
        /// The dictionary mapping non-olympiad theorems to their feedback. 
        /// The ones that are not presents are hopefully olympiad.
        /// </returns>
        public Dictionary<Theorem, TheoremFeedback> Analyze(TheoremAnalyzerInput input)
        {
            // Prepare the result
            var result = new Dictionary<Theorem, TheoremFeedback>();

            // Prepare a dictionary of resolved theorems
            var unresolvedTheorems = new Dictionary<TheoremType, List<Theorem>>();

            // Prepare a dictionary of unresolved theorems
            var resolvedTheorems = new Dictionary<TheoremType, List<Theorem>>();

            // Get the configuration for comfort
            var configuration = input.ContextualPicture.Pictures.Configuration;

            #region Trivial and simplyfiable theorems

            // Find trivial theorems ahead
            var trivialTheorems = new TheoremsMap(_trivialTheoremsProducer.DeriveTrivialTheoremsFromLastObject(configuration));

            // Go through all the theorems
            input.NewTheorems.ForEach(pair =>
            {
                // Get the current type
                var type = pair.Key;

                // Go through the theorems of this type
                pair.Value.ForEach(theorem =>
                {
                    #region Can be stated in a smaller configuration?

                    // If it can be defined in a simpler configuration, 
                    if (theorem.CanBeStatedInSmallerConfiguration())
                    {
                        // Mark it in the result
                        result.Add(theorem, new DefineableSimplerFeedback());

                        // Mark it in the dictionary
                        resolvedTheorems.GetOrAdd(type, () => new List<Theorem>()).Add(theorem);

                        // Break
                        return;
                    }

                    #endregion

                    #region Trivial theorem testing

                    // If they're is a trivial theorem equivalent to it
                    if (trivialTheorems.GetOrDefault(type)?.Any(trivialTheorem => trivialTheorem.Equals(theorem)) ?? false)
                    {
                        // Mark it in the result
                        result.Add(theorem, new TrivialTheoremFeedback());

                        // Mark it in the dictionary
                        resolvedTheorems.GetOrAdd(type, () => new List<Theorem>()).Add(theorem);

                        // Break
                        return;
                    }

                    #endregion

                    // Add this point the theorem is not resolved
                    // Make sure the it's added to the dictionary
                    unresolvedTheorems.GetOrAdd(type, () => new List<Theorem>()).Add(theorem);
                });
            });

            #endregion

            #region Sub-theorems deriver

            // Go through the template theorems
            _data.TemplateTheorems
                // Take only those that contain any theorem of type that has an unresolved theorem
                .Where(pair => pair.Item2.Keys.Any(unresolvedTheorems.ContainsKey))
                // From each derive new theorems
                .ForEach(pair =>
                {
                    // Deconstruct
                    var (configuration, templateTheorems) = pair;

                    // Call the service
                    _subtheoremsDeriver.DeriveTheorems(new SubtheoremsDeriverInput
                    {
                        ExaminedConfigurationPicture = input.ContextualPicture,
                        ExaminedConfigurationTheorems = input.AllTheorems,
                        TemplateConfiguration = configuration,
                        TemplateTheorems = templateTheorems
                    })
                    // Process derived theorems from each output
                    // (ignoring equal objects at the time)
                    .ForEach(output => output.DerivedTheorems.ForEach(pair =>
                    {
                        // Deconstruct
                        var (derivedTheorem, templateTheorem) = pair;

                        // Get an equivalent version of our found theorem
                        var foundTheorem = input.NewTheorems.FindEquivalentTheorem(derivedTheorem);

                        // If this theorem is not new, or has been proven, don't do anything
                        if (foundTheorem == null || result.ContainsKey(foundTheorem))
                            return;

                        // Otherwise we have derived a valid theorem. Mark it in the result
                        result.Add(foundTheorem, new SubtheoremFeedback
                        {
                            TemplateTheorem = templateTheorem
                        });

                        // Get the theorem type for comfort
                        var type = foundTheorem.Type;

                        // Mark it in the dictionary
                        resolvedTheorems.GetOrAdd(type, () => new List<Theorem>()).Add(foundTheorem);

                        // Remove it from the unresolved theorems
                        unresolvedTheorems[type].Remove(foundTheorem);

                        // Remove the type if there is no theorem left
                        if (unresolvedTheorems[type].IsEmpty())
                            unresolvedTheorems.Remove(type);
                    }));
                });

            #endregion

            #region Transitivity deriver

            // Get the type of theorems that are left to resolved
            // Take all key-value pairs where there is any theorem
            var unresolvedTheoremTypes = unresolvedTheorems.Select(pair => pair.Key).ToList();

            // Get the corresponding new relevant theorems from the new theorems dictionary
            var newRelevantTheorems = unresolvedTheoremTypes.SelectMany(type => input.NewTheorems[type]);

            // Get the relevant theorems that have been resolved
            var resolvedRelevantTheorems = unresolvedTheoremTypes.SelectMany(type => resolvedTheorems.GetOrDefault(type) ?? new List<Theorem>());

            // Call the deriver and process its results
            _transitivityDeriver.Derive(configuration, newRelevantTheorems, resolvedRelevantTheorems).ForEach(triple =>
            {
                // Unwrap the result
                var (fact1, fact2, concludedFact) = triple;

                // Find an equivalent to the concluded thing
                concludedFact = input.NewTheorems.FindEquivalentTheorem(concludedFact);

                // If such theorem isn't new or has already proven, don't do anything
                if (concludedFact == null || result.ContainsKey(concludedFact))
                    return;

                // Otherwise mark that we've concluded the theorem, using potentially theorems from our feedback
                // We try to provided an equivalent theorems from the input, but it may not possible
                // (they might be used in the conclusion, yet may not be new)
                result.Add(concludedFact, new TransitivityFeedback
                {
                    Fact1 = input.NewTheorems.FindEquivalentTheorem(fact1) ?? fact1,
                    Fact2 = input.NewTheorems.FindEquivalentTheorem(fact2) ?? fact2
                });
            });

            #endregion

            // Return the result
            return result;
        }
    }

    #endregion
}
