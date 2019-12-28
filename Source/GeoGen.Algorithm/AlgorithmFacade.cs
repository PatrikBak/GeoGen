using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSimplifier;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// The default implementation of <see cref="IAlgorithmFacade"/>.
    /// </summary>
    public class AlgorithmFacade : IAlgorithmFacade
    {
        #region Private fields

        /// <summary>
        /// The settings for the algorithm.
        /// </summary>
        private readonly AlgorithmFacadeSettings _settings;

        #endregion

        #region Dependencies

        /// <summary>
        /// The generator of configurations.
        /// </summary>
        private readonly IGenerator _generator;

        /// <summary>
        /// The constructor that perform the actual geometric construction of configurations.
        /// </summary>
        private readonly IGeometryConstructor _geometryConstructor;

        /// <summary>
        /// The finder of theorems in generated configurations.
        /// </summary>
        private readonly ITheoremFinder _finder;

        /// <summary>
        /// The prover of theorems.
        /// </summary>
        private readonly ITheoremProver _prover;

        /// <summary>
        /// The ranker of theorems.
        /// </summary>
        private readonly ITheoremRanker _ranker;

        /// <summary>
        /// The simplifier of theorems.
        /// </summary>
        private readonly ITheoremSimplifier _simplifier;

        /// <summary>
        /// The tracer of potential geometry failures.
        /// </summary>
        private readonly IGeometryFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmFacade"/> class.
        /// </summary>
        /// <param name="settings">The settings for the algorithm.</param>
        /// <param name="generator">The generator of configurations.</param>
        /// <param name="geometryConstructor">The constructor that perform the actual geometric construction of configurations.</param>
        /// <param name="finder">The finder of theorems in generated configurations.</param>
        /// <param name="prover">The prover of theorems.</param>
        /// <param name="ranker">The ranker of theorems.</param>
        /// <param name="simplifier">The simplifier of theorems.</param>
        /// <param name="tracer">The tracer of potential geometry failures.</param>
        public AlgorithmFacade(AlgorithmFacadeSettings settings,
                               IGenerator generator,
                               IGeometryConstructor geometryConstructor,
                               ITheoremFinder finder,
                               ITheoremProver prover,
                               ITheoremRanker ranker,
                               ITheoremSimplifier simplifier,
                               IGeometryFailureTracer tracer)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _geometryConstructor = geometryConstructor ?? throw new ArgumentNullException(nameof(geometryConstructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _prover = prover ?? throw new ArgumentNullException(nameof(prover));
            _ranker = ranker ?? throw new ArgumentNullException(nameof(ranker));
            _simplifier = simplifier ?? throw new ArgumentNullException(nameof(simplifier));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        #endregion

        #region IAlgorithm implementation

        /// <summary>
        /// Executes the algorithm for a given algorithm input.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        public (TheoremMap initialTheorems, IEnumerable<AlgorithmOutput> generationOutputs) Run(AlgorithmInput input)
        {
            #region Preparing variables

            // Prepare the stack for pictures of configurations that will be extended
            var picturesCache = new Stack<PicturesOfConfiguration>();

            // Prepare the stack for contextual pictures of configurations that will be extended
            var contextualPictureCache = new Stack<ContextualPicture>();

            // Prepare the stack for theorems of configurations that will be extended
            var theoremMapCache = new Stack<TheoremMap>();

            #endregion

            #region Initial configuration

            // Safely execute
            var (initialPictures, initialData) = GeneralUtilities.TryExecute(
                // Constructing the configuration
                () => _geometryConstructor.Construct(input.InitialConfiguration, _settings.NumberOfPictures),
                // Make sure a potential exception is caught and re-thrown
                (InconsistentPicturesException e) => throw new InitializationException("Drawing of the initial configuration failed.", e));

            // Make sure it is constructible
            if (initialData.InconstructibleObject != default)
                throw new InitializationException($"The initial configuration contains an inconstructible object.");

            // Make sure there are no duplicates...
            if (initialData.Duplicates != default)
                throw new InitializationException($"The initial configuration contains duplicate objects.");

            // Cache the pictures
            picturesCache.Push(initialPictures);

            // Safely execute
            var initialContextualPicture = GeneralUtilities.TryExecute(
                 // Creating the contextual picture
                 () => new ContextualPicture(initialPictures),
                 // Make sure a potential exception is caught and re-thrown
                 (InconsistentPicturesException e) => throw new InitializationException("Drawing of the contextual container for the initial configuration failed.", e));

            // Cache the contextual picture
            contextualPictureCache.Push(initialContextualPicture);

            // Find the initial theorems for the initial configuration
            var initialTheorems = _finder.FindAllTheorems(initialContextualPicture);

            // Cache the initial theorems
            theoremMapCache.Push(initialTheorems);

            #endregion

            #region Configuration verification function

            // Prepare a function that does geometric verification of the configurations
            // While doing so it construct pictures that we can reuse further for finding theorems
            bool VerifyConfigurationCorrectness(GeneratedConfiguration configuration)
            {
                #region Exclusion based on symmetry

                // Find out if we should exclude this configuration because of symmetry
                // That can happen only if we are told to do so...
                var excludeBecauseOfSymmetry = _settings.ExcludeAsymmetricConfigurations &&
                    // And there is no chance for this configuration to yield a symmetric one by 
                    // extending it in the remaining iterations. This covers even the case where
                    // this is the last iteration and the configuration is not symmetric 
                    configuration.IterationIndex + configuration.GetMinimalNumberOfObjectsToMakeThisSymmetric() > input.NumberOfIterations;

                // If we should exclude this configuration because of symmetry, do it
                if (excludeBecauseOfSymmetry)
                    return false;

                #endregion

                #region Handling cache

                // We assume that the generator uses a memory-efficient DFS approach, i.e.
                // when we are done with extending a configuration, we will never need it again
                // and we move to the next one. This way we just need to cache the initial configuration
                // and N-1 subsequent ones, where N is the number of iterations. 

                // We got a configuration. We need to make sure that our cache contains only 
                // objects belonging to the previous configurations of this. There final number
                // should be configuration.IterationIndex, therefore we know how many we should remove
                GeneralUtilities.ExecuteNTimes(picturesCache.Count - configuration.IterationIndex, () =>
                {
                    // Remove the last pictures / contextual picture / theorems map from the cache
                    picturesCache.Pop();
                    contextualPictureCache.Pop();
                    theoremMapCache.Pop();
                });

                #endregion

                #region Pictures construction

                // Get the previous pictures
                var previousPictures = picturesCache.Peek();

                // Safely execute
                var (newPictures, constructionData) = GeneralUtilities.TryExecute(
                    // Constructing the pictures for the configuration
                    () => _geometryConstructor.ConstructByCloning(previousPictures, configuration),
                    // While tracing a possible failure (such configurations will be discarded in the next step)
                    (InconsistentPicturesException e) => _tracer.InconstructiblePicturesByCloning(previousPictures, configuration, e));

                // The configuration is incorrect if it couldn't be carried out...
                var configurationIncorrect = newPictures == default
                    // If it contains an inconstructible object
                    || constructionData.InconstructibleObject != default
                    // Or it contains the same object twice
                    || constructionData.Duplicates != default;

                // Exclude incorrect configuration
                if (configurationIncorrect)
                    return false;

                // We have to remember the pictures for further use
                picturesCache.Push(newPictures);

                #endregion

                #region Contextual picture construction

                // Get the previous contextual picture
                var previousContextualPicture = contextualPictureCache.Peek();

                // Safely execute
                var newContextualPicture = GeneralUtilities.TryExecute(
                    // Constructing the new contextual picture by cloning
                    () => previousContextualPicture.ConstructByCloning(newPictures),
                    // While tracing a possible failure (such configurations will be discarded in the next step)
                    (InconsistentPicturesException e) => _tracer.InconstructibleContextualPictureByCloning(previousContextualPicture, newPictures, e));

                // If the construction of the picture cannot be done...
                if (newContextualPicture == default)
                {
                    // We need to make sure the pictures are discarded
                    picturesCache.Pop();

                    // And we can rule out the whole configuration
                    return false;
                }

                // We have to remember the contextual picture for further use
                contextualPictureCache.Push(newContextualPicture);

                #endregion

                // FEAUTURE: Should we verify here whether the configuration has a normal picture? E.g. no close points.

                // If we got here, everything's fine
                return true;
            }

            #endregion

            #region Returning result

            // Prepare the generator input
            var generatorInput = new GeneratorInput
            (
                numberOfIterations: input.NumberOfIterations,
                constructions: input.Constructions,
                initialConfiguration: input.InitialConfiguration,
                configurationFilter: VerifyConfigurationCorrectness
            );

            // Return the tuple of initial theorems and a lazy algorithm enumerable
            return (initialTheorems, _generator.Generate(generatorInput)
                   // For each correct configuration perform the subsequent algorithms
                   .Select(configuration =>
                   {
                       // Get the contextual picture of the current configuration
                       var picture = contextualPictureCache.Peek();

                       // Get the old theorems
                       var oldTheorems = theoremMapCache.Peek();

                       // Find new theorems
                       var newTheorems = _finder.FindNewTheorems(picture, oldTheorems);

                       // Find all theorems by merging the old and new ones
                       var allTheorems = new TheoremMap(oldTheorems.AllObjects.Concat(newTheorems.AllObjects));

                       // Cache the theorems for this configuration
                       theoremMapCache.Push(allTheorems);

                       // If we should exclude asymmetric configurations and this one is not like that,
                       // then we don't need to try to prove the theorems of this one
                       if (_settings.ExcludeAsymmetricConfigurations && !configuration.IsSymmetric())
                           return null;

                       // Prepare the input for the theorem prover
                       var input = new TheoremProverInput
                       (
                           contextualPicture: picture,
                           oldTheorems: oldTheorems,
                           newTheorems: newTheorems
                       );

                       // Try to prove them
                       var proverOutput = _prover.Analyze(input);

                       // Rank all unproved theorems
                       var rankings = proverOutput.UnprovenTheorems.Keys
                            // Each rank separately
                            .ToDictionary(theorem => theorem, theorem => _ranker.Rank(theorem, configuration, input.AllTheorems, proverOutput));

                       // Simplify unproved theorems
                       var simplifiedTheorems = proverOutput.UnprovenTheorems.Keys
                            // Each is attempted to be simplified
                            .Select(theorem => (theorem, simplification: _simplifier.Simplify(configuration, theorem, input.AllTheorems)))
                            // Take only those where it has been successful
                            .Where(result => result.simplification != null)
                            // Wrap the results to a dictionary
                            .ToDictionary(pair => pair.theorem, pair => pair.simplification.Value);

                       // Return the final output
                       return new AlgorithmOutput
                       (
                           configuration: configuration,
                           oldTheorems: oldTheorems,
                           newTheorems: newTheorems,
                           proverOutput: proverOutput,
                           rankings: rankings,
                           simplifiedTheorems: simplifiedTheorems
                       );
                   })
                   // Ignore excluded ones
                   .Where(output => output != null));

            #endregion
        }

        #endregion
    }
}