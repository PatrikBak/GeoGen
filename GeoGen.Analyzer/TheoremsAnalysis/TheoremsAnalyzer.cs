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
        /// The validator of an output of verifiers.
        /// </summary>
        private readonly IOutputValidator _outputValidator;

        /// <summary>
        /// The container of default theorems.
        /// </summary>
        private readonly ITheoremsContainer _container;

        /// <summary>
        /// The constructor of <see cref="Theorem"/> objects.
        /// </summary>
        private readonly ITheoremConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="verifiers">The array of available verifiers to be used.</param>
        /// <param name="factory">The factory for creating contextual containers holding configurations.</param>
        /// <param name="container">The container of default theorems.</param>
        /// <param name="validator">The validator of verifiers output.</param>
        /// <param name="constructor">The constructor of theorems.</param>
        public TheoremsAnalyzer
        (
            ITheoremVerifier[] verifiers,
            IContextualContainerFactory factory,
            IOutputValidator validator,
            ITheoremsContainer container,
            ITheoremConstructor constructor
        )
        {
            _verifiers = verifiers ?? throw new ArgumentNullException(nameof(verifiers));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _outputValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
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
            return _verifiers.SelectMany(verifier => verifier.GetOutput(container))
                    // Take only those that represent a correct theorem
                    .Where(output => _outputValidator.Validate(configuration, output))
                    // Cast all of them to an actual theorem object
                    .Select(output => _constructor.Construct(output.InvoldedObjects, output.Type))
                    // Take only those that are not present in the container of default theorems
                    .Where(theorem => !_container.Contains(theorem))
                    // Enumerate to list
                    .ToList();
        }

        #endregion
    }
}