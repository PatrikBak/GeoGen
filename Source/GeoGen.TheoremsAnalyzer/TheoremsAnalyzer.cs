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
        public TheoremsAnalyzer(TheoremsAnalyzerData data, ITrivialTheoremsProducer trivialTheoremsProducer, ISubtheoremAnalyzer subtheoremAnalyzer)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _trivialTheoremsProducer = trivialTheoremsProducer ?? throw new ArgumentNullException(nameof(trivialTheoremsProducer));
            _subtheoremAnalyzer = subtheoremAnalyzer ?? throw new ArgumentNullException(nameof(subtheoremAnalyzer));
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

            // All theorems equivalent to them 
            input.Theorems.Where(theorem => trivialTheorems.Any(trivialTheorem => Theorem.AreTheoremsEquivalent(trivialTheorem, theorem)))
                // can be marked as trivial
                .ForEach(theorem => result.Add(theorem, new TrivialTheoremFeedback()));

            // Try to match remaining theorems...
            input.Theorems.Where(theorem => !result.ContainsKey(theorem)).ForEach(theorem =>
            {
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

                // If there is a match, add feedback
                if (match != default)
                    result.Add(theorem, new SubtheoremFeedback { TemplateTheorem = match.templateTheorem });
                
            });

            // Return the result
            return result;
        }

        #endregion
    }
}
