using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremsAnalyzer : ITheoremsAnalyzer
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
        /// The validator of an output of verifiers.
        /// </summary>
        private readonly IPotentialTheoremValidator _outputValidator;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="verifiers">The array of available verifiers to be used.</param>
        /// <param name="factory">The factory for creating contextual containers holding configurations.</param>
        /// <param name="validator">The validator of verifiers output.</param>
        public TheoremsAnalyzer
        (
            ITheoremVerifier[] verifiers,
            IContextualContainerFactory factory,
            IPotentialTheoremValidator validator
        )
        {
            _verifiers = verifiers ?? throw new ArgumentNullException(nameof(verifiers));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _outputValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        public List<Theorem> Analyze(Configuration configuration)
        {
            // Create contextual container holding the configuration
            var container = _factory.Create(configuration);

            // For each verifier create many outputs
            return _verifiers.SelectMany(verifier => verifier.FindPotencialTheorems(container))
                    // Take only those that represent a correct theorem
                    .Where(output => _outputValidator.Validate(configuration, output))
                    // Cast them to actual theorems
                    .Select(output => output.ToTheorem())
                    // Enumerate to list
                    .ToList();
        }        

        #endregion
    }
}