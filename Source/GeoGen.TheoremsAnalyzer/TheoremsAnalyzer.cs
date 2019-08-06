using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremsAnalyzer : ITheoremsAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The producer of trivial theorems.
        /// </summary>
        private readonly ITrivialTheoremsProducer _trivialTheoremsProducer;

        /// <summary>
        /// The sub-theorems analyzer. It gets template theorems from the <see cref="_data"/>.
        /// </summary>
        private readonly ISubtheoremAnalyzer _subtheoremAnalyzer;

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
        /// <param name="subtheoremAnalyzer">The sub-theorems analyzer. It gets template theorems from the <see cref="data"/>.</param>
        /// <
        public TheoremsAnalyzer(TheoremsAnalyzerData data, ITrivialTheoremsProducer trivialTheoremsProducer, ISubtheoremAnalyzer subtheoremAnalyzer, ITransitivityDeriver transitivityDeriver)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _trivialTheoremsProducer = trivialTheoremsProducer ?? throw new ArgumentNullException(nameof(trivialTheoremsProducer));
            _subtheoremAnalyzer = subtheoremAnalyzer ?? throw new ArgumentNullException(nameof(subtheoremAnalyzer));
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

            // Get the trivial theorems first
            var trivialTheorems = _trivialTheoremsProducer.DeriveTrivialTheoremsFromLastObject(input.Configuration);

            // Go through all the theorems
            input.Theorems.ForEach(theorem =>
            {
                #region Can be stated in a smaller configuration?

                // If it can be defined in a simpler configuration, 
                if (theorem.CanBeStatedInSmallerConfiguration())
                {
                    // Mark it
                    result.Add(theorem, new DefineableSimplerFeedback());

                    // Break
                    return;
                }

                #endregion

                #region Trivial theorem testing

                // If they're is a trivial theorem equivalent to it
                if (trivialTheorems.Any(trivialTheorem => trivialTheorem.IsEquivalentTo(theorem)))
                {
                    // Mark it
                    result.Add(theorem, new TrivialTheoremFeedback());

                    // Break
                    return;
                }

                #endregion

                #region Sub-theorem algorithm

                // Otherwise try sub-theorem algorithm
                // Find possible theorems that might imply this one
                var match = _data.TemplateTheorems.Where(templateTheorem => theorem.Type == templateTheorem.Type)
                    // Try to match
                    .Select(templateTheorem => (templateTheorem, output: _subtheoremAnalyzer.Analyze(new SubtheoremAnalyzerInput
                    {
                        ExaminedTheorem = theorem,
                        TemplateTheorem = templateTheorem,
                        ExaminedConfigurationObjectsContainer = input.ConfigurationObjectsContainer,
                        ExaminedConfigurationContexualPicture = input.ContextualPicture,
                        ExaminedConfigurationManager = input.Manager
                    })))
                    // Get the first that matches
                    .FirstOrDefault(pair => pair.output.IsSubtheorem);

                // If there is a match,
                if (match != default)
                {
                    // Mark it
                    result.Add(theorem, new SubtheoremFeedback { TemplateTheorem = match.templateTheorem });

                    // Break
                    return;
                }

                #endregion
            });

            // Now we'll try to use the transitivity rule for the rest
            _transitivityDeriver.Derive(input.Configuration, input.Theorems, result.Keys.ToList()).ForEach(triple =>
            {
                // Unwrap the result
                var (fact1, fact2, concludedFact) = triple;

                // Local function that finds an equivalent theorem from the input, 
                // or returns the current one, if there is no such theorem
                Theorem FindEquivalent(Theorem theorem) => input.Theorems.FirstOrDefault(_theorem => _theorem.IsEquivalentTo(theorem)) ?? theorem;

                // Find an equivalent to the concluded thing
                concludedFact = FindEquivalent(concludedFact);

                // If it's been already proven, don't do a thing
                if (result.ContainsKey(concludedFact))
                    return;

                // Otherwise mark that we've concluded the theorem, using potentially theorems from our feedback
                result.Add(concludedFact, new TransitivityFeedback
                {
                    Fact1 = FindEquivalent(fact1),
                    Fact2 = FindEquivalent(fact2)
                });
            });

            // Return the result
            return result;
        }
    }

    #endregion
}
