using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremsAnalyzer;
using GeoGen.TheoremsFinder.new_stuff;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleLauncher
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
        private readonly ITheoremsFinder _finder;

        /// <summary>
        /// The factory for creating objects containers.
        /// </summary>
        private readonly IConfigurationObjectsContainerFactory _containerFactory;

        /// <summary>
        /// The analyzer of theorem providing feedback whether they are olympiad or not.
        /// </summary>
        private readonly ITheoremsAnalyzer _analyzer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAlgorithm"/> class.
        /// </summary>
        /// <param name="generator">The generator of configurations.</param>
        /// <param name="geometryConstructor">The constructor that perform the actual geometric construction of configurations.</param>
        /// <param name="pictureFactory">The factory for creating contextual pictures.</param>
        /// <param name="finder">The finder of theorems in generated configurations.</param>
        /// <param name="containerFactory">The factory for creating objects containers.</param>
        /// <param name="analyzer">The analyzer of theorem providing feedback whether they are olympiad or not.</param>
        public SequentialAlgorithm(IGenerator generator,
                                   IGeometryConstructor geometryConstructor,
                                   IContextualPictureFactory pictureFactory,
                                   ITheoremsFinder finder,
                                   IConfigurationObjectsContainerFactory containerFactory,
                                   ITheoremsAnalyzer analyzer)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _geometryConstructor = geometryConstructor ?? throw new ArgumentNullException(nameof(geometryConstructor));
            _pictureFactory = pictureFactory ?? throw new ArgumentNullException(nameof(pictureFactory));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _containerFactory = containerFactory ?? throw new ArgumentNullException(nameof(containerFactory));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IAlgorithm implementation

        /// <summary>
        /// Executes the algorithm for a given generator input.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <returns>The theorems in the initial configuration and a lazy enumerable of all the generated output.</returns>
        public (TheoremsMap initialTheorems, IEnumerable<AlgorithmOutput> generationOutputs) Run(GeneratorInput input)
        {
            #region Initial configuration

            // Safely execute
            var (initialPictures, initialData) = GeneralUtilities.TryExecute(
                // Constructing the configuration
                () => _geometryConstructor.Construct(input.InitialConfiguration),
                // Make sure the exception is caught and re-thrown
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
                 // While ignoring potential issues (such a configuration will be discarded anyway)
                 (InconstructibleContextualPicture e) => throw new InitializationException("Drawing of the contextual container for the initial configuration failed.", e));

            // Find the initial theorems for the configuration
            var initialTheorems = _finder.FindAllTheorems(initialContextualPicture);

            #endregion

            #region Preparing variables

            // Prepare the map for contextual pictures
            var contextualPicturesMap = new Dictionary<GeneratedConfiguration, HierarchicalContextualPicture>();

            // Prepare the map for pictures
            var picturesMap = new Dictionary<GeneratedConfiguration, Pictures>();

            // Prepare a set containing inconstructible objects. 
            var inconstructibleObjects = new HashSet<ConfigurationObject>();

            #endregion

            #region Object verification function

            // Prepare the function that checks if the generated object is correct
            bool VerifyConstructedObjectCorrectness(ConstructedConfigurationObject constructedObject)
            {
                // It's correct if it hasn't been previously marked as an inconstructible one
                return !inconstructibleObjects.Contains(constructedObject);
            }

            #endregion

            #region Configuration verification function

            // Prepare a function that does geometric verification of the configurations
            bool VerifyConfigurationCorrectness(GeneratedConfiguration configuration)
            {
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

                // If the construction didn't work out, the configuration is incorrect
                if (pictures == default)
                    return false;

                // Find out if the constructed object is inconstructible 
                var anyInconstructibleObject = constructionData.InconstructibleObject != default;

                // Find out if there are any duplicates
                var anyDuplicates = constructionData.Duplicates != default;

                // If there is an inconstructible object, remember it 
                if (anyInconstructibleObject)
                    inconstructibleObjects.Add(constructionData.InconstructibleObject);

                // If there are any invalid objects or duplicates, then it's incorrect
                if (anyInconstructibleObject || anyDuplicates)
                    return false;

                // If we got here, the configuration is fine. 
                // We have to remember its pictures for further use
                picturesMap.Add(configuration, pictures);

                #endregion

                #region Contextual picture construction

                // Get the previous contextual picture
                // If the previous configuration is the initial one
                var previousContextualPicture = configuration.PreviousConfiguration.IterationIndex == 0
                        // Then the result is the initial picture
                        ? initialContextualPicture
                        // Otherwise we get it from the map
                        : contextualPicturesMap[configuration.PreviousConfiguration];

                // Safely execute
                var newContextualPicture = GeneralUtilities.TryExecute(
                    // Constructing the new contextual picture by cloning
                    () => previousContextualPicture.ConstructByCloning(pictures),
                    // Ignoring a possible failure (such configurations will be discarded in the next step)
                    (InconstructibleContextualPicture _) => { });

                // If the construction cannot be done, we can't use this configuration
                if (newContextualPicture == default)
                    return false;

                // Otherwise add it to the map
                contextualPicturesMap.Add(configuration, newContextualPicture);

                #endregion

                // If we got here, everything's fine
                return true;
            }

            #endregion

            // Perform the generation with our defined verification functions
            return (initialTheorems, _generator.Generate(input, VerifyConstructedObjectCorrectness, VerifyConfigurationCorrectness)
                    // For each correct configuration perform the theorem analysis
                    .Select(configuration =>
                    {
                        // Get the contextual picture
                        var picture = contextualPicturesMap[configuration];

                        // Find new theorems
                        var newTheorems = _finder.FindNewTheorems(picture);

                        // Create a container holding the objects of the configuration
                        var container = _containerFactory.CreateContainer(configuration);

                        // Analyze the theorems
                        var analysisResult = _analyzer.Analyze(new TheoremAnalyzerInput
                        {
                            ContextualPicture = picture,
                            NewTheorems = newTheorems,
                            ConfigurationObjectsContainer = container
                        });

                        // Return the final output
                        return new AlgorithmOutput
                        {
                            Configuration = configuration,
                            Theorems = newTheorems.AllObjects,
                            AnalyzerOutput = analysisResult
                        };
                    }));
        }

        #endregion
    }
}