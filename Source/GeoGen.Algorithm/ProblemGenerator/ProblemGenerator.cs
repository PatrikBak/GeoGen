using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// The default implementation of <see cref="IProblemGenerator"/> that uses <see cref="IGenerator"/>
    /// to generate configurations, <see cref="IGeometryConstructor"/> to construct them analytically,
    /// and <see cref="ITheoremFinder"/> to find their theorems.
    /// <para>
    /// It is able to trace configurations where geometric construction failed via <see cref="IGeometryConstructor"/>.
    /// </para>
    /// <para>
    /// It is also able to perform efficient exclusion of asymmetric configurations, which can be specified
    /// via <see cref="ProblemGeneratorSettings"/>.
    /// </para>
    /// </summary>
    public class ProblemGenerator : IProblemGenerator
    {
        #region Private fields

        /// <summary>
        /// The settings for the problem generator.
        /// </summary>
        private readonly ProblemGeneratorSettings _settings;

        #endregion

        #region Dependencies

        /// <summary>
        /// The generator of configurations.
        /// </summary>
        private readonly IGenerator _generator;

        /// <summary>
        /// The constructor that perform geometric construction of configurations.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The finder of theorems in generated configurations.
        /// </summary>
        private readonly ITheoremFinder _finder;

        /// <summary>
        /// The tracer of potential geometry failures.
        /// </summary>
        private readonly IGeometryFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGenerator"/> class.
        /// </summary>
        /// <param name="settings">The settings for the problem generator.</param>
        /// <param name="generator">The generator of configurations.</param>
        /// <param name="constructor">The constructor that perform geometric construction of configurations.</param>
        /// <param name="finder">The finder of theorems in generated configurations.</param>
        /// <param name="tracer">The tracer of potential geometry failures.</param>
        public ProblemGenerator(ProblemGeneratorSettings settings,
                                IGenerator generator,
                                IGeometryConstructor constructor,
                                ITheoremFinder finder,
                                IGeometryFailureTracer tracer)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        #endregion

        #region IProblemGenerator implementation

        /// <inheritdoc/>
        public (TheoremMap initialTheorems, IEnumerable<ProblemGeneratorOutput> generationOutputs) Generate(ProblemGeneratorInput input)
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
                // Constructing the configuration suitable for generation
                () => _constructor.Construct(input.InitialConfiguration, _settings.NumberOfPictures, LooseObjectDrawingStyle.GenerationFriendly),
                // Make sure a potential exception is caught and re-thrown
                (InconsistentPicturesException e) => throw new InitializationException("Drawing the initial configuration failed.", e));

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
                 (InconsistentPicturesException e) => throw new InitializationException("Drawing the contextual container for the initial configuration failed.", e));

            // Cache the contextual picture
            contextualPictureCache.Push(initialContextualPicture);

            // Find the initial theorems for the initial configuration
            var initialTheorems = _finder.FindAllTheorems(initialContextualPicture);

            // Cache the initial theorems
            theoremMapCache.Push(initialTheorems);

            #endregion

            #region Configuration verification function

            // Prepare a function that does geometric verification of a generated configuration
            // While doing so it construct pictures that we can reuse further for finding theorems
            bool VerifyConfigurationCorrectness(GeneratedConfiguration configuration)
            {
                #region Handling cache

                // We assume that the generator uses a memory-efficient DFS approach, i.e.
                // we need to remember only the last N-1 configurations, where N is the number 
                // of the current iteration. 

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

                #region Exclusion based on symmetry

                // Find out if we should exclude this configuration because of symmetry
                // That can happen only if we are told to do so...
                var excludeBecauseOfSymmetry = _settings.ExcludeAsymmetricConfigurations &&
                    // And it is not true that we can generate some objects that would make this configuration symmetric
                    !configuration.GetObjectsThatWouldMakeThisConfigurationSymmetric()
                        // Find out if given objects to be added could really be added
                        .Any(objectsToBeAdded =>
                        {
                            // If we don't have enough iterations to add that many objects, then we're done
                            if (configuration.IterationIndex + objectsToBeAdded.Count() > input.NumberOfIterations)
                                return false;

                            // Otherwise find out if we have the needed constructions by taking the objects
                            var doWeHaveNeededConstructions = objectsToBeAdded
                                // Their constructions
                                .Select(objectToBeAdded => objectToBeAdded.Construction)
                                // All of them must be among the ones we're using to extend objects
                                .All(input.Constructions.Contains);

                            // If we don't have the needed constructions, these objects can't be generated
                            if (!doWeHaveNeededConstructions)
                                return false;

                            // Otherwise categorize objects to be added by taking them
                            return objectsToBeAdded
                                // Grouping by their type
                                .GroupBy(objectToBeAdded => objectToBeAdded.ObjectType)
                                // It needs to be true that we can add that many objects from every type
                                .All(group =>
                                {
                                    // Get the type
                                    var type = group.Key;

                                    // Get the needed count 
                                    var neededObjectsOfThisTypeCount = group.Count();

                                    // Get the number of already added objects of this type by taking all objects of this type
                                    var addedObjectsOfThisTypeCount = configuration.ObjectMap.GetObjectsForKeys(type).Count()
                                        // And subtracting objects of this type from the initial configuration
                                        - input.InitialConfiguration.ObjectMap.GetObjectsForKeys(type).Count();

                                    // We can add these objects if and only if we will not exceed the maximal number of objects to add
                                    return neededObjectsOfThisTypeCount + addedObjectsOfThisTypeCount <= input.MaximalNumbersOfObjectsToAdd[type];
                                });
                        });

                // If we should exclude this configuration because of symmetry, do it
                if (excludeBecauseOfSymmetry)
                    return false;

                #endregion

                #region Pictures construction

                // Get the previous pictures
                var previousPictures = picturesCache.Peek();

                // Safely execute
                var (newPictures, constructionData) = GeneralUtilities.TryExecute(
                    // Constructing the pictures for the configuration
                    () => _constructor.ConstructByCloning(previousPictures, configuration),
                    // While tracing a possible failure (such configurations will be discarded in the next step)
                    (InconsistentPicturesException e) => _tracer.InconstructiblePicturesByCloning(previousPictures, configuration, e));

                // The configuration is incorrect if it couldn't be carried out...
                var isConfigurationIncorrect = newPictures == default
                    // Or if it contains an inconstructible object
                    || constructionData.InconstructibleObject != default
                    // Or if it contains the same object twice
                    || constructionData.Duplicates != default;

                // Exclude incorrect configuration
                if (isConfigurationIncorrect)
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
                    // We need to make sure the already constructed pictures are discarded
                    picturesCache.Pop();

                    // And we say the configuration is incorrect
                    return false;
                }

                // We have to remember the contextual picture for further use
                contextualPictureCache.Push(newContextualPicture);

                #endregion

                // If we got here, everything's fine
                return true;
            }

            #endregion

            #region Returning result

            // Prepare the generator input
            var generatorInput = new GeneratorInput
            (
                // Pass the data from the algorithm input
                numberOfIterations: input.NumberOfIterations,
                constructions: input.Constructions,
                initialConfiguration: input.InitialConfiguration,
                maximalNumbersOfObjectsToAdd: input.MaximalNumbersOfObjectsToAdd,

                // As a filter pass our verification that draws the configuration geometrically
                configurationFilter: VerifyConfigurationCorrectness
            );

            // Return the tuple of initial theorems and a lazy algorithm enumerable
            return (initialTheorems, _generator.Generate(generatorInput)
                   // For each configuration perform the subsequent algorithms
                   .Select(configuration =>
                   {
                       // Get the contextual picture of the current configuration
                       var contextualPicture = contextualPictureCache.Peek();

                       // Get the previous of the previous configuration
                       var oldTheorems = theoremMapCache.Peek();

                       // Find new theorems
                       var newTheorems = _finder.FindNewTheorems(contextualPicture, oldTheorems);

                       // Find all theorems by merging the old and new ones
                       var allTheorems = new TheoremMap(oldTheorems.AllObjects.Concat(newTheorems.AllObjects));

                       // Cache the theorems for this configuration
                       theoremMapCache.Push(allTheorems);

                       // If we should exclude asymmetric configurations and this one is like that,
                       // then we don't need to try to return it as a correct one
                       // We can't do this before finding theorems, because this asymmetric configuration
                       // might still be extensible to get a symmetric one and we would need its theorems
                       if (_settings.ExcludeAsymmetricConfigurations && !configuration.IsSymmetric())
                           return null;

                       // Return the final output
                       return new ProblemGeneratorOutput
                       (
                           configuration: configuration,
                           contextualPicture: contextualPicture,
                           oldTheorems: oldTheorems,
                           newTheorems: newTheorems
                       );
                   })
                   // Ignore excluded ones
                   .Where(output => output != null));

            #endregion
        }

        #endregion
    }
}