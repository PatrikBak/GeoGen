using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    internal class TheoremsAnalyzer : ITheoremsAnalyzer
    {
        #region Private fields

        /// <summary>
        /// The array of all available theorem verifiers.
        /// </summary>
        private readonly ITheoremVerifier[] _verifiers;

        /// <summary>
        /// The factory for creating contextual containers
        /// </summary>
        private readonly IContextualContainerFactory _factory;

        /// <summary>
        /// The service to which is delegated the actual theorem creating
        /// </summary>
        private readonly ITheoremsValidator _validator;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="verifiers">The array of available verifiers to be used.</param>
        /// <param name="factory">The factory for creating contextual containers holding configurations.</param>
        /// <param name="validator">The validator for converting verifier output to theorems.</param>
        public TheoremsAnalyzer(ITheoremVerifier[] verifiers, IContextualContainerFactory factory, ITheoremsValidator validator)
        {
            _verifiers = verifiers ?? throw new ArgumentNullException(nameof(verifiers));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        public List<Theorem> Analyze(Configuration configuration)
        {
            // Create contextual container holding the configuration
            var container = _factory.Create(configuration);

            // Merge output of all verifiers
            var verifiersOutput = _verifiers.SelectMany(v => v.GetOutput(container));

            // Let the validator find out which of these outputs is an actual theorem
            return _validator.ValidateTheorems(configuration, verifiersOutput).ToList();
        }

        #endregion
    }
}