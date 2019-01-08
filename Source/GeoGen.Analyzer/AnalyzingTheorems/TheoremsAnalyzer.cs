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

        #endregion

        #region Private fields

        /// <summary>
        /// The configuration for the analyzer.
        /// </summary>
        private readonly ITheoremAnalysisConfiguration _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzer"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for the analyzer.</param>
        /// <param name="theoremsAnalyzers">The array of all the available potential theorems analyzers.</param>
        /// <param name="registrar">The geometry registrar that holds already registered object managers for the configurations.</param>
        /// <param name="factory">The factory for creating contextual containers that are required by the theorem analyzers.</param>
        public TheoremsAnalyzer(ITheoremAnalysisConfiguration configuration, IPotentialTheoremsAnalyzer[] theoremsAnalyzers, IGeometryRegistrar registrar, IContextualContainerFactory factory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
                container = manager.ExecuteAndResolvePossibleIncosistencies(() => _factory.Create(configuration.ObjectsMap.AllObjects, manager));
            }
            catch (UnresolvableInconsistencyException)
            {
                // If there are unresolvable inconsistencies, then we can't do much
                return new TheoremsAnalyzerOutput { TheoremAnalysisSuccessful = false };
            }

            // Map each verifier to all possible theorems it produces
            var potentialTheorems = _theoremsAnalyzers.SelectMany(verifier => verifier.FindPotentialTheorems(container))
                    // Take those potential theorems that turn out not to contain needless objects
                    .Where(potentialTheorem => !potentialTheorem.ContainsNeedlessObjects(expectedMinimalNumberOfNeededObjects: configuration.ObjectsMap.AllObjects.Count))
                    // Now we test each theorem in all the containers. We exclude the ones
                    .Select(potentialTheorem => (potentialTheorem, trueContainers: manager.Count(potentialTheorem.VerificationFunction)))
                    // Exclude the ones that are false in fewer than the given minimal number of containers
                    .Where(tuple => tuple.trueContainers >= _configuration.MinimalNumberOfTrueContainersToRevalidate)
                    // Enumerate them to a list which will perform the actual validations
                    .ToList();

            // Prepare a list of theorems that turns out to be true, possible after revalidation
            var trueTheorems = new List<AnalyzedTheorem>();

            // Prepare a list of theorems that are potentially false negatives, i.e. true in some reasonable number of containers
            var potentialFalseNegatives = new List<AnalyzedTheorem>();

            // Prepare a variable indicated if we attempted to recreate the container yet
            // null means we didn't, false means we did and it wasn't successful, true means it went fine
            bool? wasContainerRecreated = null;

            // Local function that performs the recreation and sets this value
            void RecreateContextualContainer()
            {
                // A variable indicating the number of attempts
                var numberOfAttempts = 0;

                // While we should try...
                while (numberOfAttempts < _configuration.MaximalNumberOfAttemptsToReconstructContextualContainer)
                {
                    // Mark an attempt
                    numberOfAttempts++;

                    try
                    {
                        // Perform the recreation
                        container.Recreate();

                        // If we got here, we're happy
                        break;
                    }
                    catch (AnalyzerException)
                    {
                        // It might happen that it failed. This is a very rare case, 
                        // but due to the imprecision of the analytic geometry it is possible
                        // TODO: Replace with some tracer
                        Console.WriteLine("It has happened");
                    }
                }

                // We did it if and only if we didn't reach the maximal number of attempts
                wasContainerRecreated = numberOfAttempts == _configuration.MaximalNumberOfAttemptsToReconstructContextualContainer;
            }

            // Handle all the theorems we're left with
            foreach (var (potentialTheorem, trueContainers) in potentialTheorems)
            {
                // If the theorem is true in enough containers...
                if (trueContainers >= _configuration.MinimalNumberOfTrueContainers)
                {
                    // Then we add the theorem to the list of certain ones
                    trueTheorems.Add(new AnalyzedTheorem(potentialTheorem, trueContainers, numberOfTrueContainersAfterSecondTest: null));

                    // And continue to the next one
                    continue;
                }

                // Otherwise we have a theorem to retest
                // First we recreate the container, if we haven't it done already
                if (wasContainerRecreated == null)
                    RecreateContextualContainer();

                // If the container couldn't be recreated, then we can't perform the second phase :(
                if (!wasContainerRecreated.Value)
                {
                    // We just add the theorem to potentially false negatives
                    potentialFalseNegatives.Add(new AnalyzedTheorem(potentialTheorem, trueContainers, numberOfTrueContainersAfterSecondTest: null));

                    // And continue to the next one
                    continue;
                }

                // Otherwise we can first the number of true containers in the second phase
                var trueContainersAfterRevalidation = manager.Count(potentialTheorem.VerificationFunction);

                // Construct the resulting theorem
                var theorem = new AnalyzedTheorem(potentialTheorem, trueContainers, trueContainersAfterRevalidation);

                // And decide to which list we're going to add it according to the configuration
                (trueContainersAfterRevalidation >= _configuration.MinimalNumberOfTrueContainers ? trueTheorems : potentialFalseNegatives).Add(theorem);
            }

            // Return the final output
            return new TheoremsAnalyzerOutput
            {
                // Set that we've finished successfully
                TheoremAnalysisSuccessful = true,

                // Set theorems
                Theorems = trueTheorems,

                // Set false negatives
                PotentialFalseNegatives = potentialFalseNegatives
            };
        }

        #endregion
    }
}