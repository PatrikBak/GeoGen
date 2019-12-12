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
                               IGeometryFailureTracer tracer = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _geometryConstructor = geometryConstructor ?? throw new ArgumentNullException(nameof(geometryConstructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _prover = prover ?? throw new ArgumentNullException(nameof(prover));
            _ranker = ranker ?? throw new ArgumentNullException(nameof(ranker));
            _simplifier = simplifier ?? throw new ArgumentNullException(nameof(simplifier));
            _tracer = tracer;
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

            // Safely execute
            var initialContextualPicture = GeneralUtilities.TryExecute(
                 // Creating of the picture
                 () => new ContextualPicture(initialPictures),
                 // Make sure a potential exception is caught and re-thrown
                 (InconsistentPicturesException e) => throw new InitializationException("Drawing of the contextual container for the initial configuration failed.", e));

            // Find the initial theorems for the configuration
            var initialTheorems = _finder.FindAllTheorems(initialContextualPicture);

            #endregion

            #region Preparing variables

            // Prepare the map for contextual pictures
            var contextualPictureMap = new Dictionary<GeneratedConfiguration, ContextualPicture>();

            // Prepare the map for pictures
            var picturesMap = new Dictionary<GeneratedConfiguration, PicturesOfConfiguration>();

            // Prepare the map for theorems
            var theoremMap = new Dictionary<GeneratedConfiguration, TheoremMap>();

            // Prepare the set containing excluded objects. 
            var excludedObjects = new HashSet<ConfigurationObject>();

            // Prepare the pictures where we will draw all the objects to find geometrically equal ones
            // These pictures won't represent a specific configuration
            var objectTestingPictures = initialPictures.CloneAsRegularPictures();

            // Prepare the dictionary mapping configuration objects to their integer codes
            var allObjectCodes = new Dictionary<ConfigurationObject, int>(
                // Initialize it with the objects of the initial configuration
                input.InitialConfiguration.AllObjects.ToDictionary((obj, index) => obj, (obj, index) => index));

            // Prepare the set of correctly examined configurations, where the configurations
            // are coded by the codes of their objects wrapped in a read-only set 
            // (which correctly implements hash code and equals)
            var configurationCodes = new HashSet<IReadOnlyHashSet<int>>();

            #endregion

            #region Object verification function

            // Prepare the function that checks if the generated object is correct
            bool VerifyConstructedObjectCorrectness(ConstructedConfigurationObject constructedObject)
            {
                // If it's been excluded, then it's not fine
                if (excludedObjects.Contains(constructedObject))
                    return false;

                // If the object already has a code, then it's fine
                if (allObjectCodes.ContainsKey(constructedObject))
                    return true;

                // If it doesn't have a code, then safely execute
                var data = GeneralUtilities.TryExecute(
                    // Constructing of the object with adding it to the pictures
                    () => _geometryConstructor.Construct(objectTestingPictures, constructedObject, addToPictures: true),
                    // Tracing a possible failure, which will be handled in the next step
                    (InconsistentPicturesException e) => _tracer?.UndrawableObjectInBigPicture(constructedObject, input.InitialConfiguration.LooseObjectsHolder, objectTestingPictures, e));

                // If it couldn't be drawn
                if (data == null)
                {
                    // Mark it
                    excludedObjects.Add(constructedObject);

                    // The object is incorrect
                    return false;
                }

                // If it's inconstructible...
                if (data.InconstructibleObject != default)
                {
                    // Mark it
                    excludedObjects.Add(data.InconstructibleObject);

                    // The object is incorrect
                    return false;
                }

                // Get the code for the object
                // If there are no duplicates
                var newCode = data.Duplicates == default ?
                    // Then the object gets a very new one
                    allObjectCodes.Count :
                    // Otherwise it inherits the code of its original version
                    allObjectCodes[data.Duplicates.olderObject];

                // Assign the new code
                allObjectCodes.Add(constructedObject, newCode);

                // TODO: Trace discovered equality

                // At this stage the object is fine
                return true;
            }

            #endregion

            #region Configuration verification function

            // Prepare a function that does geometric verification of the configurations
            // While doing so it construct pictures that we can reuse further for finding theorems
            bool VerifyConfigurationCorrectness(GeneratedConfiguration configuration)
            {
                #region Check if there is a geometrically equal configuration

                // Get the code of the configuration
                var currentObjectCodes = configuration.AllObjects.Select(o => allObjectCodes[o]).ToReadOnlyHashSet();

                // If the number doesn't match, then there are duplicates
                if (currentObjectCodes.Count != configuration.AllObjects.Count)
                    return false;

                // If these codes already exist, then this configuration has been examined
                if (configurationCodes.Contains(currentObjectCodes))
                    return false;

                // Otherwise the configuration is new and we can remember their object codes
                configurationCodes.Add(currentObjectCodes);

                #endregion

                #region Pictures construction

                // Get the previous pictures
                // If the previous configuration is the initial one
                var previousPictures = configuration.PreviousConfiguration.IterationIndex == 0
                        // Then the result is the initial picture
                        ? initialPictures
                        // Otherwise we get it from the map
                        : picturesMap[configuration.PreviousConfiguration];

                // Safely execute
                var (pictures, constructionData) = GeneralUtilities.TryExecute(
                    // Constructing of the pictures for the configuration
                    () => _geometryConstructor.ConstructByCloning(previousPictures, configuration),
                    // While ignoring a possible failure for now (this is practically not possible, see the next comment)
                    (InconsistentPicturesException e) => { });

                // Since the configuration has been confirmed to be correct, there should not
                // be any problem regarding the construction of this object. This seem to be next to impossible.
                // Even after reducing the default precision of rounding doubles I couldn't make this happen
                // In any case, it doesn't hurt us to have this check here
                if (pictures == default || constructionData.InconstructibleObject != default || constructionData.Duplicates != default)
                    return false;

                // We have to remember the pictures for further use
                picturesMap.Add(configuration, pictures);

                #endregion

                #region Contextual picture construction

                // Get the previous contextual picture
                // If the previous configuration is the initial one
                var previousContextualPicture = configuration.PreviousConfiguration.IterationIndex == 0
                        // Then the result is the initial picture
                        ? initialContextualPicture
                        // Otherwise we get it from the map
                        : contextualPictureMap[configuration.PreviousConfiguration];

                // Safely execute
                var newContextualPicture = GeneralUtilities.TryExecute(
                    // Constructing the new contextual picture by cloning
                    () => previousContextualPicture.ConstructByCloning(pictures),
                    // While tracing a possible failure (such configurations will be discarded in the next step)
                    (InconsistentPicturesException e) => _tracer?.InconstructibleContextualPictureByCloning(previousContextualPicture, pictures, e));

                // If the construction cannot be done, we can't use this configuration
                if (newContextualPicture == default)
                    return false;

                // Otherwise add it to the map
                contextualPictureMap.Add(configuration, newContextualPicture);

                #endregion

                // FEATURE: Should we verify here whether the configuration has a normal picture? E.g. no close points.

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
                objectFilter: VerifyConstructedObjectCorrectness,
                configurationFilter: VerifyConfigurationCorrectness
            );

            // Return the tuple of initial theorems and lazy algorithm enumerable
            return (initialTheorems, _generator.Generate(generatorInput)
                   // For each correct configuration perform the theorem analysis
                   .Select(configuration =>
                   {
                       // Get the contextual picture
                       var picture = contextualPictureMap[configuration];

                       // Get the old theorems
                       // If the previous configuration is the initial one
                       var oldTheorems = configuration.PreviousConfiguration.IterationIndex == 0 ?
                          // Then the old theorems are the initial ones
                          initialTheorems
                          // Otherwise we take them from the map
                          : theoremMap[configuration.PreviousConfiguration];

                       // Find new theorems
                       var newTheorems = _finder.FindNewTheorems(picture, oldTheorems);

                       // Prepare the input for the theorem prover
                       var input = new TheoremProverInput
                       (
                           contextualPicture: picture,
                           oldTheorems: oldTheorems,
                           newTheorems: newTheorems
                       );

                       // Cache all the theorems (that the prover conveniently merged for us)
                       theoremMap.Add(configuration, input.AllTheorems);

                       // Analyze them
                       var proverOutput = _prover.Analyze(input);

                       // Rank unproved theorems
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
                   }));

            #endregion
        }

        #endregion
    }
}