using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The default implementation of <see cref="ITrivialTheoremProducer"/>. This implementation caches trivial theorems for 
    /// <see cref="ComposedConstruction"/>s. They are found by constructing the underlying <see cref="ComposedConstruction.Configuration"/>
    /// using <see cref="IGeometryConstructor"/> and finding the theorems in it via <see cref="ITheoremFinder"/>. When it comes to
    /// objects constructed via <see cref="PredefinedConstruction"/>s, these have their trivial theorems listed manually.
    /// </summary>
    public class TrivialTheoremProducer : ITrivialTheoremProducer
    {
        #region Private constants

        /// <summary>
        /// The number of pictures that are used for finding trivial theorems of composed constructions. In practice,
        /// this value is not very important therefore it is not made configurable.
        /// </summary>
        private const int NumberOfPicturesForFindingTrivialTheorems = 5;

        #endregion

        #region Dependencies

        /// <summary>
        /// The constructor used to construct underlying configurations of composed constructions.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The theorem finder used to determine theorems of composed constructions.
        /// </summary>
        private readonly ITheoremFinder _finder;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping <see cref="ComposedConstruction"/>s to the theorems holding true
        /// for the loose objects of the <see cref="ComposedConstruction.Configuration"/> and the construction output.
        /// </summary>
        private readonly Dictionary<ComposedConstruction, IReadOnlyList<Theorem>> _composedConstructionTheorems = new Dictionary<ComposedConstruction, IReadOnlyList<Theorem>>();

        /// <summary>
        /// The cache dictionary mapping the names of composed constructions to the instances  that were used to
        /// determine the <see cref="_composedConstructionTheorems"/>. This is important because in order to do remapping
        /// of inner loose objects we need to use the original ones.
        /// </summary>
        private readonly Dictionary<string, ComposedConstruction> _originalInstances = new Dictionary<string, ComposedConstruction>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrivialTheoremProducer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor used to construct underlying configurations of composed constructions.</param>
        /// <param name="finder">The theorem finder used to determine theorems of composed constructions.</param>
        public TrivialTheoremProducer(IGeometryConstructor constructor, ITheoremFinder finder)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
        }

        #endregion

        #region ITrivialTheoremProducer implementation

        /// <inheritdoc/>
        public IReadOnlyList<Theorem> InferTrivialTheoremsFromObject(ConstructedConfigurationObject constructedObject)
        {
            // Get the flattened arguments of the object for comfort
            var objects = constructedObject.PassedArguments.FlattenedList;

            // Switch on the construction
            switch (constructedObject.Construction)
            {
                // If we have a predefined construction...
                case PredefinedConstruction predefinedConstruction:

                    // Switch on its type
                    switch (predefinedConstruction.Type)
                    {
                        // An angle bisector makes equal angles and an incidence
                        case InternalAngleBisector:
                        {
                            // Create theorem objects
                            var line1 = new LineTheoremObject(objects[0], objects[1]);
                            var line2 = new LineTheoremObject(objects[0], objects[2]);
                            var bisector = new LineTheoremObject(constructedObject);

                            // Create the theorems
                            return new[]
                            {
                                // Equal angles
                                new Theorem(EqualAngles, new []
                                {
                                    new AngleTheoremObject(line1, bisector),
                                    new AngleTheoremObject(line2, bisector)
                                }),

                                // Incidence
                                new Theorem(Incidence, objects[0], constructedObject)
                            };
                        }

                        // Point reflection makes collinear points and equally distanced points
                        case PointReflection:

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(CollinearPoints, objects[0], objects[1], constructedObject),

                                // Equal distances
                                new Theorem(EqualLineSegments, new TheoremObject[]
                                {
                                    new LineSegmentTheoremObject(objects[1], objects[0]),
                                    new LineSegmentTheoremObject(objects[1], constructedObject)
                                })
                            };

                        // Point reflection makes collinear points and equally distanced points
                        case Midpoint:

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(CollinearPoints, objects[0], objects[1], constructedObject),

                                // Equal distances
                                new Theorem(EqualLineSegments, new TheoremObject[]
                                {
                                    new LineSegmentTheoremObject(objects[0], constructedObject),
                                    new LineSegmentTheoremObject(objects[1], constructedObject)
                                })
                             };

                        // Perpendicular projection makes perpendicular lines and incidence
                        case PerpendicularProjection:

                            // Create the theorem
                            return new[]
                            {
                                // Perpendicular lines
                                new Theorem(PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(objects[0], constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),
                                
                                // Incidence
                                new Theorem(Incidence, objects[1], constructedObject)
                            };

                        // Perpendicular line makes (surprisingly) perpendicular lines and incidence
                        case PerpendicularLine:

                            // Create the theorems
                            return new[]
                            {
                                // Perpendicular lines
                                new Theorem(PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),

                                // Incidence
                                new Theorem(Incidence, objects[0], constructedObject)
                            };

                        // Parallel line makes (surprisingly) parallel lines and incidence
                        case ParallelLine:

                            // Create the theorems
                            return new[]
                            {
                                // Parallel lines
                                new Theorem(ParallelLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),

                                // Incidence
                                new Theorem(Incidence, objects[0], constructedObject)
                            };

                        // Second intersection of two circumcircles makes concyclic points
                        case SecondIntersectionOfTwoCircumcircles:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(ConcyclicPoints, constructedObject, objects[0], objects[1], objects[2]),
                                new Theorem(ConcyclicPoints, constructedObject, objects[0], objects[3], objects[4])
                            };

                        // Second intersection of a circle and line from point make collinear and concyclic points
                        case SecondIntersectionOfCircleAndLineFromPoints:

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(CollinearPoints, constructedObject, objects[0], objects[1]),

                                // Concyclity
                                new Theorem(ConcyclicPoints, constructedObject, objects[0], objects[2], objects[3])
                            };

                        // Line makes incidences
                        case LineFromPoints:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(Incidence, constructedObject, objects[0]),
                                new Theorem(Incidence, constructedObject, objects[1])
                            };

                        // Circumcircle makes incidences
                        case Circumcircle:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(Incidence, constructedObject, objects[0]),
                                new Theorem(Incidence, constructedObject, objects[1]),
                                new Theorem(Incidence, constructedObject, objects[2])
                            };

                        // Circle with center through point makes incidence
                        case CircleWithCenterThroughPoint:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(Incidence, constructedObject, objects[1])
                            };

                        // Intersection of lines makes incidences
                        case IntersectionOfLines:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(Incidence, constructedObject, objects[0]),
                                new Theorem(Incidence, constructedObject, objects[1])
                            };

                        // Center of circle brings nothing :/
                        case CenterOfCircle:
                            return Array.Empty<Theorem>();

                        // Unhandled cases
                        default:
                            throw new TheoremProverException($"Unhandled value of {nameof(PredefinedConstructionType)}: {predefinedConstruction.Type}");
                    }

                // If we have a composed construction...
                case ComposedConstruction composedConstruction:

                    // If we haven't found the theorems yet...
                    if (!_originalInstances.ContainsKey(composedConstruction.Name))
                    {
                        // Let the helper method do the job
                        var newTheorems = FindTheoremsForComposedConstruction(composedConstruction);

                        // Add them
                        _composedConstructionTheorems.Add(composedConstruction, newTheorems);

                        // Mark the used instance
                        _originalInstances.Add(composedConstruction.Name, composedConstruction);
                    }

                    // Get the original instance used for the theorems
                    composedConstruction = _originalInstances[composedConstruction.Name];

                    // Get the theorems
                    var theorems = _composedConstructionTheorems[composedConstruction];

                    // Create the mapping of loose objects of the template configuration 
                    var mapping = composedConstruction.Configuration.LooseObjects.Cast<ConfigurationObject>()
                        // and the template construction output
                        .Concat(composedConstruction.ConstructionOutput)
                        // to the actual objects that created the constructed object and the constructed object itself
                        .ZipToDictionary(objects.Concat(constructedObject));

                    // Remap all the theorems
                    return theorems.Select(theorem => theorem.Remap(mapping)).ToList();

                // Unhandled cases
                default:
                    throw new TheoremProverException($"Unhandled type of {nameof(Construction)}: {constructedObject.Construction.GetType()}");
            }
        }

        /// <summary>
        /// Find all theorems that hold true for the loose objects and the construction output of a given composed construction.
        /// </summary>
        /// <param name="composedConstruction">The composed construction to be examined.</param>
        /// <returns>The theorems holding true for the loose objects and the construction output.</returns>
        private IReadOnlyList<Theorem> FindTheoremsForComposedConstruction(ComposedConstruction composedConstruction)
        {
            // Local function that throws an exception
            void ThrowIncorrectConstructionException(string message, Exception innerException = null)
                // informing about the examination failure
                => throw new TheoremProverException($"Cannot examine construction {composedConstruction.Name}. {message}", innerException);

            // Create a helper constructed object that simulates the composed construction
            var helperConstructedObject = new ConstructedConfigurationObject(composedConstruction,
                // Its arguments will be the loose objects of the composed construction's configuration
                composedConstruction.Configuration.LooseObjects.Cast<ConfigurationObject>().ToArray());

            // Prepare the configuration that simulates the composed construction
            var helperConfiguration = new Configuration(composedConstruction.Configuration.LooseObjectsHolder, new[] { helperConstructedObject });

            // Safely execute
            var (pictures, data) = GeneralUtilities.TryExecute(
                // Construction of the new configuration
                () => _constructor.Construct(helperConfiguration, NumberOfPicturesForFindingTrivialTheorems, LooseObjectDrawingStyle.GenerationFriendly),
                // While making sure any exception is caught and re-thrown
                (InconsistentPicturesException e) => ThrowIncorrectConstructionException("The defining configuration couldn't be drawn.", e));

            // Make sure it has no inconstructible objects
            if (data.InconstructibleObject != default)
                ThrowIncorrectConstructionException("The defining configuration contains an inconstructible object.");

            // Make sure it has no duplicates
            if (data.Duplicates != default)
                ThrowIncorrectConstructionException("The defining configuration contains duplicate objects.");

            // Safely execute
            var contextualPicture = GeneralUtilities.TryExecute(
                // Construction of the contextual picture
                () => new ContextualPicture(pictures),
                // While making sure any exception is caught and re-thrown
                (InconsistentPicturesException e) => ThrowIncorrectConstructionException("The contextual picture for the defining configuration couldn't be drawn.", e));

            // Find the theorems
            return _finder.FindAllTheorems(contextualPicture).AllObjects
                // Take those that say something about the last object 
                // (there might be theorems in the layout as well, for example in RightTriangle)
                .Where(theorem => theorem.GetInnerConfigurationObjects().Contains(helperConstructedObject))
                // We need to remap the helper constructed object to the one from the underlying configuration
                .Select(theorem =>
                {
                    // Prepare the mapping dictionary
                    var mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                    { 
                        // That already has the helper object remapped to the construction output
                        { helperConstructedObject, composedConstruction.ConstructionOutput }
                    };

                    // Add the loose objects to the mapping as identities
                    composedConstruction.Configuration.LooseObjects.ForEach(looseObject => mapping.Add(looseObject, looseObject));

                    // Do the mapping
                    return theorem.Remap(mapping);
                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}