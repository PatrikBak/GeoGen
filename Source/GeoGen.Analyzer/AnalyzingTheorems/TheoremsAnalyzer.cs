using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremsAnalyzer"/>. This implementation assumes 
    /// the configuration already have been registered to the provided <see cref="IGeometryRegistrar"/>.
    /// </summary>
    public class TheoremsAnalyzer : ITheoremsAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The array of all the available potential theorems analyzers.
        /// </summary>
        private readonly IPotentialTheoremsAnalyzer[] _theoremsAnalyzers;

        /// <summary>
        /// The geometry registrar that holds already registered object managers for the configurations.
        /// </summary>
        private readonly IGeometryRegistrar _registrar;

        /// <summary>
        /// The factory for creating contextual containers that are required by the theorem analyzers.
        /// </summary>
        private readonly IContextualContainerFactory _factory;

        /// <summary>
        /// The analyzer whether a theorem essentially involves all the objects from the configuration.
        /// </summary>
        private readonly INeedlessObjectsAnalyzer _needlessObjectsAnalyzer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="theoremsAnalyzers">The array of all the available potential theorems analyzers.</param>
        /// <param name="registrar">The geometry registrar that holds already registered object managers for the configurations.</param>
        /// <param name="factory">The factory for creating contextual containers that are required by the theorem analyzers.</param>
        /// <param name="needlessObjectAnalyzer">The analyzer whether a theorem essentially involves all the objects from the configuration.</param>
        public TheoremsAnalyzer(IPotentialTheoremsAnalyzer[] theoremsAnalyzers, IGeometryRegistrar registrar, IContextualContainerFactory factory, INeedlessObjectsAnalyzer needlessObjectAnalyzer)
        {
            _theoremsAnalyzers = theoremsAnalyzers ?? throw new ArgumentNullException(nameof(theoremsAnalyzers));
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _needlessObjectsAnalyzer = needlessObjectAnalyzer ?? throw new ArgumentNullException(nameof(needlessObjectAnalyzer));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        public List<Theorem> Analyze(Configuration configuration)
        {
            // Find the containers manager that was created for this configuration
            var manager = _registrar.GetContainersManager(configuration);

            // Let the manager safely create an instance of the contextual container
            var container = manager.ExecuteAndResolvePossibleIncosistencies(() => _factory.Create(configuration, manager));

            // For each verifier create many outputs
            return _theoremsAnalyzers.SelectMany(verifier => verifier.FindPotentialTheorems(container))
                    // Take those potential theorems that turn out to be true
                    .Where(potentialTheorem =>
                    {
                        // Test the theorem in all the containers
                        if (!manager.All(potentialTheorem.VerificationFunction))
                            return false;

                        // If the theorem holds true, then it's fine if and only if it doesn't contain needless objects
                        return _needlessObjectsAnalyzer.ContainsNeedlessObjects(configuration, potentialTheorem.InvolvedObjects);
                    })
                    // Cast these potential theorem to actual theorem objects
                    .Select(output => output.ToTheorem())
                    // And enumerate to a list
                    .ToList();
        }

        #endregion
    }
}