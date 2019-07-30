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
        public TheoremsAnalyzer(TheoremsAnalyzerData data, ITrivialTheoremsProducer trivialTheoremsProducer)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _trivialTheoremsProducer = trivialTheoremsProducer ?? throw new ArgumentNullException(nameof(trivialTheoremsProducer));
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

            // Return the result
            return result;
        }

        #endregion
    }
}
