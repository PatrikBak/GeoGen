using GeoGen.Core;
using System;
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="theoremsAnalyzers">The array of all the available potential theorems analyzers.</param>
        /// <param name="registrar">The geometry registrar that holds already registered object managers for the configurations.</param>
        /// <param name="factory">The factory for creating contextual containers that are required by the theorem analyzers.</param>
        public TheoremsAnalyzer(IPotentialTheoremsAnalyzer[] theoremsAnalyzers, IGeometryRegistrar registrar, IContextualContainerFactory factory)
        {
            _theoremsAnalyzers = theoremsAnalyzers ?? throw new ArgumentNullException(nameof(theoremsAnalyzers));
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration where we're looking for theorems.</param>
        /// <returns>The output of the analyzer holding the theorems.</returns>
        public TheoremsAnalyzerOutput Analyze(Configuration configuration)
        {
            // Find the containers manager that was created for this configuration
            var manager = _registrar.GetContainersManager(configuration);

            // Prepare a variable holding the contextual container to be used by the analyzers
            IContextualContainer container;

            try
            {
                // Let the manager create an instance of the contextual container
                container = manager.ExecuteAndResolvePossibleIncosistencies(() => _factory.Create(configuration, manager));
            }
            // If there are unresolvable inconsistencies, then we can't do much
            catch (UnresolvableInconsistencyException)
            {
                return new TheoremsAnalyzerOutput { TheoremAnalysisSuccessful = false };
            }

            // Otherwise we have a correct contextual container
            // We can create the theorems list
            // For each verifier create many outputs...
            var theorems = _theoremsAnalyzers.SelectMany(verifier => verifier.FindPotentialTheorems(container))
                    // Take those potential theorems that turn out to be true
                    .Where(potentialTheorem =>
                    {
                        // Test the theorem in all the containers
                        if (!manager.All(potentialTheorem.VerificationFunction))
                            return false;

                        // If the theorem holds true, then it's fine if and only if it doesn't contain needless 
                        // objects. We'll figure this out by asking if the minimal number of objects that are 
                        // needed to state this theorem is equal to the number of objects of the configuration
                        return !potentialTheorem.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: configuration.ObjectsMap.AllObjects.Count);
                    })
                    // Cast these potential theorem to actual theorem objects
                    .Select(potentialTheorem => potentialTheorem.ToTheorem())
                    // And enumerate to a list
                    .ToList();

            // Return the final output
            return new TheoremsAnalyzerOutput
            {
                // Set the theorems
                Theorems = theorems,

                // Set that we've finished successfully
                TheoremAnalysisSuccessful = true
            };
        }

        #endregion
    }
}