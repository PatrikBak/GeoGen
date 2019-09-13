using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremProver;
using GeoGen.TheoremsFinder;
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
        /// The finders of theorems in generated configurations.
        /// </summary>
        private readonly ITheoremsFinder[] _finders;

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
        /// <param name="finders">The finders of theorems in generated configurations.</param>
        /// <param name="prover">The prover of theorems.</param>
        public SequentialAlgorithm(IGenerator generator,
                                   IGeometryConstructor geometryConstructor,
                                   IContextualPictureFactory pictureFactory,
                                   ITheoremsFinder[] finders,
                                   ITheoremProver prover)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _geometryConstructor = geometryConstructor ?? throw new ArgumentNullException(nameof(geometryConstructor));
            _pictureFactory = pictureFactory ?? throw new ArgumentNullException(nameof(pictureFactory));
            _finders = finders ?? throw new ArgumentNullException(nameof(finders));
            _prover = prover ?? throw new ArgumentNullException(nameof(prover));
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
            var initialTheorems = new TheoremsMap(_finders.SelectMany(finder => finder.FindAllTheorems(initialContextualPicture)));

            #endregion

            #region Preparing variables

            // Prepare the map for contextual pictures
            var contextualPicturesMap = new Dictionary<GeneratedConfiguration, ContextualPicture>();

            // Prepare the map for pictures
            var picturesMap = new Dictionary<GeneratedConfiguration, Pictures>();

            // Prepare the map for theorems
            var theoremsMap = new Dictionary<GeneratedConfiguration, TheoremsMap>();

            // Prepare the set containing inconstructible objects. 
            var inconstructibleObjects = new HashSet<ConfigurationObject>();

            #endregion

            #region Construction verification function

            // The function that says if we should perform the construction on the configuration
            // TODO: Implement it meaningfully, for example, consider not using construction when
            //       it's already used more than a few times.
            static bool VerifyThatConstructionMightBeApplied(GeneratedConfiguration configuration, Construction construction) => true;

            #endregion

            #region Object verification function

            // Prepare the function that checks if the generated object is correct
            bool VerifyConstructedObjectCorrectness(GeneratedConfiguration currentConfiguration, ConstructedConfigurationObject constructedObject)
            {
                // It's correct if it hasn't been previously marked as an inconstructible one
                return !inconstructibleObjects.Contains(constructedObject);
            }

            #endregion

            #region Configuration verification function

            // Prepare a function that does geometric verification of the configurations
            // While doing so it construct pictures that we can reuse further for finding theorems
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

                // TODO: Duplicates = EqualObjects theorem. The configuration already been examined,
                //       but, these theorems might be interesting. I should probably implement 
                //       an algorithm that finds the minimal configuration for this (going backwards?
                //       and this class can then do something interesting about the found theorem
                //       it would technically have an access to the picture where the striped 
                //       configuration is drawn, i.e. it can call theorems analyzer. For now it's not worth it 

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
                       var picture = contextualPicturesMap[configuration];

                       // Find new theorems
                       var newTheorems = new TheoremsMap(_finders.SelectMany(finder => finder.FindNewTheorems(picture)));

                       // Get the old theorems
                       // If the previous configuration is the initial one
                       var oldTheorems = configuration.PreviousConfiguration.IterationIndex == 0 ?
                          // Then the old theorems are the initial ones
                          initialTheorems
                          // Otherwise we take them from the map
                          : theoremsMap[configuration.PreviousConfiguration];

                       // We need to correct them so that theorem objects depict the most recent situation
                       oldTheorems = new TheoremsMap(oldTheorems.AllObjects.Select(theorem => Recreate(picture, theorem)));

                       // Prepare the input for the theorem prover
                       var input = new TheoremProverInput
                       (
                           contextualPicture: picture,
                           oldTheorems: oldTheorems,
                           newTheorems: newTheorems
                       );

                       // Cache all the theorems (that the prover conveniently merged for us)
                       theoremsMap.Add(configuration, input.AllTheorems);

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

        /// <summary>
        /// Makes sure a given theorem contains the most current theorem objects. For example,
        /// if theorem is about line A, B, and there already is another point C on this line,
        /// then we want to have this piece of information included in the theorem.
        /// </summary>
        /// <param name="picture">The contextual picture with data about lines, circles and points.</param>
        /// <param name="theorem">To theorem to be (potentially) recreated.</param>
        /// <returns>A recreated theorem with adjusted objects.</returns>
        private Theorem Recreate(ContextualPicture picture, Theorem theorem)
        {
            // Local function that recreates an object
            TheoremObject Recreate(TheoremObject theoremObject)
            {
                // Switch on the type
                switch (theoremObject)
                {
                    // Base objects might have an object part
                    case BaseTheoremObject baseObject:

                        // Switch on type of base object
                        switch (baseObject)
                        {
                            // Don't do anything with point
                            case PointTheoremObject point:
                                return point;

                            // If we have an object with points...
                            case TheoremObjectWithPoints objectWithPoints:

                                #region Find the corresponding object in the contextual picture

                                // Prepare the variable holding the result
                                var geometricObject = default(DefinableByPoints);

                                // It's easy if there is a configuration object specified
                                if (objectWithPoints.ConfigurationObject != null)
                                {
                                    // Then we can get it directly
                                    geometricObject = (DefinableByPoints)picture.GetGeometricObject(objectWithPoints.ConfigurationObject);
                                }
                                // Otherwise we need find it by points
                                else
                                {
                                    // Take the needed number of representing the points of this theorem object
                                    var points = objectWithPoints.Points.Take(objectWithPoints.NumberOfNeededPoints)
                                        // Find their point objects
                                        .Select(picture.GetGeometricObject).Cast<PointObject>()
                                        // Enumerate
                                        .ToList();

                                    // Get possible candidates based on the type of this object
                                    geometricObject = (objectWithPoints switch
                                    {
                                        // Line
                                        LineTheoremObject _ => points[0].Lines as IEnumerable<DefinableByPoints>,

                                        // Circle
                                        CircleTheoremObject _ => points[0].Circles,

                                        // Unhandled case
                                        _ => throw new GeoGenException($"Unhandled type of object with points: {objectWithPoints.GetType()}"),
                                    })
                                    // Take the first that contains all the points
                                    .First(lineOrCircle => lineOrCircle.ContainsAll(points));
                                }

                                #endregion

                                // Create new points
                                var newPoints = geometricObject.Points.Select(point => point.ConfigurationObject);

                                // Create the final result based on type
                                return objectWithPoints switch
                                {
                                    // Line
                                    LineTheoremObject _ => new LineTheoremObject(geometricObject.ConfigurationObject, newPoints) as TheoremObject,

                                    // Circle
                                    CircleTheoremObject _ => new CircleTheoremObject(geometricObject.ConfigurationObject, newPoints),

                                    // Unhandled case
                                    _ => throw new GeoGenException($"Unhandled type of object with points: {objectWithPoints.GetType()}"),
                                };

                            // If something else
                            default:
                                throw new GeoGenException($"Unhandled type of base theorem object: {baseObject.GetType()}");
                        }

                    // Objects with two theorem objects
                    case PairTheoremObject pairObject:

                        // Switch based on type
                        return pairObject switch
                        {
                            // Don't do anything with line segments
                            LineSegmentTheoremObject lineSegment => lineSegment as TheoremObject,

                            // With angle convert individual lines
                            AngleTheoremObject angle => new AngleTheoremObject((LineTheoremObject)Recreate(angle.Object1), (LineTheoremObject)Recreate(angle.Object2)),

                            // Default case
                            _ => throw new GeoGenException($"Unhandled type of pair theorem object: {pairObject.GetType()}")
                        };

                    // If something else
                    default:
                        throw new GeoGenException($"Unhandled type of theorem object: {theoremObject.GetType()}");
                }
            }

            // Return theorem with created objects
            return new Theorem(theorem.Configuration, theorem.Type, theorem.InvolvedObjects.Select(Recreate).ToArray());
        }

        #endregion
    }
}