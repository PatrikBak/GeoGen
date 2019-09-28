using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents a simple version of the algorithm where each configuration is tested 
    /// for theorems immediately after it's generated.
    /// </summary>
    public class SequentialAlgorithm : IAlgorithm
    {
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
        /// The factory for creating contextual pictures.
        /// </summary>
        private readonly IContextualPictureFactory _pictureFactory;

        /// <summary>
        /// The finder of theorems in generated configurations.
        /// </summary>
        private readonly ITheoremFinder _finder;

        /// <summary>
        /// The prover of theorems.
        /// </summary>
        private readonly ITheoremProver _prover;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAlgorithm"/> class.
        /// </summary>
        /// <param name="generator">The generator of configurations.</param>
        /// <param name="geometryConstructor">The constructor that perform the actual geometric construction of configurations.</param>
        /// <param name="pictureFactory">The factory for creating contextual pictures.</param>
        /// <param name="finder">The finder of theorems in generated configurations.</param>
        /// <param name="prover">The prover of theorems.</param>
        public SequentialAlgorithm(IGenerator generator,
                                   IGeometryConstructor geometryConstructor,
                                   IContextualPictureFactory pictureFactory,
                                   ITheoremFinder finder,
                                   ITheoremProver prover)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _geometryConstructor = geometryConstructor ?? throw new ArgumentNullException(nameof(geometryConstructor));
            _pictureFactory = pictureFactory ?? throw new ArgumentNullException(nameof(pictureFactory));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _prover = prover ?? throw new ArgumentNullException(nameof(prover));
        }

        #endregion

        #region IAlgorithm implementation

        /// <summary>
        /// Executes the algorithm for a given generator input.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        public (TheoremMap initialTheorems, IEnumerable<AlgorithmOutput> generationOutputs) Run(GeneratorInput input)
        {
            #region Initial configuration

            // Safely execute
            var (initialPictures, initialData) = GeneralUtilities.TryExecute(
                // Constructing the configuration
                () => _geometryConstructor.Construct(input.InitialConfiguration),
                // Make sure a potential exception is caught and re-thrown
                (GeometryConstructionException e) => throw new InitializationException("Drawing of the initial configuration failed.", e));

            // Make sure it is constructible
            if (initialData.InconstructibleObject != default)
                throw new InitializationException($"The initial configuration contains an inconstructible object.");

            // Make sure there are no duplicates...
            if (initialData.Duplicates != default)
                throw new InitializationException($"The initial configuration contains duplicate objects.");

            // Safely execute
            var initialContextualPicture = GeneralUtilities.TryExecute(
                 // Creating of the picture
                 () => _pictureFactory.CreateContextualPicture(initialPictures),
                 // Make sure a potential exception is caught and re-thrown
                 (InconstructibleContextualPicture e) => throw new InitializationException("Drawing of the contextual container for the initial configuration failed.", e));

            // Find the initial theorems for the configuration
            var initialTheorems = _finder.FindAllTheorems(initialContextualPicture);

            #endregion

            #region Preparing variables

            // Prepare the map for contextual pictures
            var contextualPictureMap = new Dictionary<GeneratedConfiguration, ContextualPicture>();

            // Prepare the map for pictures
            var picturesMap = new Dictionary<GeneratedConfiguration, Pictures>();

            // Prepare the map for theorems
            var theoremMap = new Dictionary<GeneratedConfiguration, TheoremMap>();

            // Prepare the set containing inconstructible objects. 
            var inconstructibleObjects = new HashSet<ConfigurationObject>();

            // Prepare the picture where we will draw all the objects to find geometrically equal ones
            var objectTestingPictures = initialPictures.Clone();

            // Prepare the dictionary mapping configuration objects to their integer codes
            var allObjectCodes = new Dictionary<ConfigurationObject, int>(
                // Initialize it with the objects of the initial configuration
                input.InitialConfiguration.AllObjects.ToDictionary((obj, index) => obj, (obj, index) => index));

            // Prepare the set of correctly examined configurations, where the configurations
            // are coded by the codes of their objects wrapped in a read-only set 
            // (which correctly implements hash code and equals)
            var configurationCodes = new HashSet<IReadOnlyHashSet<int>>();

            #endregion

            #region Construction verification function

            // The function that says if we should perform the construction on the configuration
            // TODO: Consider implementing it meaningfully, for example, consider not using 
            //       a construction when it's already used more than a few times, or some other 
            //       heuristic approach
            static bool VerifyThatConstructionMightBeApplied(GeneratedConfiguration configuration, Construction construction) => true;

            #endregion

            #region Object verification function

            // Prepare the function that checks if the generated object is correct
            bool VerifyConstructedObjectCorrectness(GeneratedConfiguration currentConfiguration, ConstructedConfigurationObject constructedObject)
            {
                // If it's marked as an inconstructible one, then it's not fine
                if (inconstructibleObjects.Contains(constructedObject))
                    return false;

                // If the object already has a key, then it's fine
                if (allObjectCodes.ContainsKey(constructedObject))
                    return true;

                // If it doesn't have a code, construct it 
                // TODO: Do it safely
                // TODO: Add tracing
                var data = _geometryConstructor.ExamineObject(objectTestingPictures, constructedObject, addToPictures: true);

                // If it's not constructible...
                if (data.InconstructibleObject != default)
                {
                    // Mark it
                    inconstructibleObjects.Add(data.InconstructibleObject);

                    // The configuration is incorrect
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

                // TODO: Duplicates might create new super-cool theorems! Consider doing something about it

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
                    // Ignoring a possible failure (such configurations will be discarded in the next step)
                    (GeometryConstructionException e) => { });

                // Since the configuration has been confirmed to be correct, there should not
                // be any problem. If there is, then this is very weird, thought theoretically not impossible
                // We're going to evaluate it as an incorrect configuration after all
                // TODO: Add tracing
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
                    // Ignoring a possible failure (such configurations will be discarded in the next step)
                    (InconstructibleContextualPicture _) => { });

                // If the construction cannot be done, we can't use this configuration
                // TODO: Add tracing
                if (newContextualPicture == default)
                    return false;

                // Otherwise add it to the map
                contextualPictureMap.Add(configuration, newContextualPicture);

                #endregion

                // TODO: Further verification, for example whether points aren't too close to each other?

                // If we got here, everything's fine
                return true;
            }

            #endregion

            #region Returning result

            // Prepare the callbacks for the generation algorithm
            var callbacks = new GenerationCallbacks
            {
                ObjectsFilter = VerifyConstructedObjectCorrectness,
                ConfigurationsFilter = VerifyConfigurationCorrectness,
                ConstructionFilter = VerifyThatConstructionMightBeApplied
            };

            // Return the tuple of initial theorems and lazy algorithm enumerable
            return (initialTheorems, _generator.Generate(input, callbacks)
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

                       // Return the final output
                       return new AlgorithmOutput
                       {
                           Configuration = configuration,
                           Theorems = newTheorems,
                           ProverOutput = proverOutput
                       };
                   }));

            #endregion
        }

        #endregion
    }
}