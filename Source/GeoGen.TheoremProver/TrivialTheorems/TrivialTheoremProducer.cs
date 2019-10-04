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
    /// The default implementation of <see cref="ITrivialTheoremProducer"/>. This implementation 
    /// caches trivial theorems that are true for general <see cref="ComposedConstruction"/>s.
    /// These are found by constructing the underlying <see cref="ComposedConstruction.Configuration"/>
    /// using <see cref="IGeometryConstructor"/> and finding the theorems in it via <see cref="ITheoremFinder"/>.
    /// </summary>
    public class TrivialTheoremProducer : ITrivialTheoremProducer
    {
        #region Dependencies

        /// <summary>
        /// The constructor used to determine theorems of composed constructions.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The finder used to determine theorems of composed constructions.
        /// </summary>
        private readonly ITheoremFinder _finder;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping the <see cref="ComposedConstruction"/>s
        /// to the theorems holding true in <see cref="ComposedConstruction.Configuration"/>.
        /// Only theorems that can be then remapped to trivial theorem are stored, i.e. the ones
        /// that can be stated with just the loose objects of the <see cref="ComposedConstruction.Configuration"/>.
        /// </summary>
        private readonly Dictionary<ComposedConstruction, IReadOnlyList<Theorem>> _composedConstructionsTheorems;

        /// <summary>
        /// The cache dictionary mapping the names of composed constructions to the instances
        /// that were used to determine the <see cref="_composedConstructionsTheorems"/>.
        /// </summary>
        private readonly Dictionary<string, ComposedConstruction> _originalInstances;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrivialTheoremProducer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor used to determine theorems of composed constructions.</param>
        /// <param name="finder">The finder used to determine theorems of composed constructions.</param>
        public TrivialTheoremProducer(IGeometryConstructor constructor, ITheoremFinder finder)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
            _composedConstructionsTheorems = new Dictionary<ComposedConstruction, IReadOnlyList<Theorem>>();
            _originalInstances = new Dictionary<string, ComposedConstruction>();
        }

        #endregion

        #region ITrivialTheoremProducer

        /// <summary>
        /// Derive trivial theorems from a given constructed configuration object.
        /// </summary>
        /// <param name="constructedObject">The object from which we should derive theorems.</param>
        /// <returns>The produced theorems.</returns>
        public IReadOnlyList<Theorem> DeriveTrivialTheoremsFromObject(ConstructedConfigurationObject constructedObject)
        {
            // If it has only loose objects, then do anything
            if (constructedObject == null)
                return Array.Empty<Theorem>();

            // Get the passed objects to it for comfort
            var objects = constructedObject.PassedArguments.FlattenedList;

            // Switch on the construction
            switch (constructedObject.Construction)
            {
                // If we have a predefined construction...
                case PredefinedConstruction predefinedConstruction:

                    // Switch based on its type
                    switch (predefinedConstruction.Type)
                    {
                        // An angle bisector makes equal angles and incidence
                        case InternalAngleBisector:
                        {
                            // Create theorem objects
                            var line1 = new LineTheoremObject(objects[0], objects[1]);
                            var line2 = new LineTheoremObject(objects[0], objects[2]);
                            var bisector = new LineTheoremObject(constructedObject);

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(EqualAngles, new []
                                {
                                    new AngleTheoremObject(line1, bisector),
                                    new AngleTheoremObject(line2, bisector)
                                }),
                                new Theorem(Incidence, objects[0], constructedObject)
                            };
                        }

                        // Point reflection makes collinear points and equally distances points
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

                        // Point reflection makes collinear points and equally distances points
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
                                new Theorem(PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(objects[0], constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),
                                new Theorem(Incidence, objects[1], constructedObject)
                            };

                        // Perpendicular line makes (surprisingly) perpendicular lines and incidence
                        case PerpendicularLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),
                                new Theorem(Incidence, objects[0], constructedObject)
                            };

                        // Parallel line makes (surprisingly) parallel lines and incidence
                        case ParallelLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(ParallelLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject),
                                    new LineTheoremObject(objects[1])
                                }),
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
                                new Theorem(CollinearPoints, constructedObject, objects[0], objects[1]),
                                new Theorem(ConcyclicPoints, constructedObject, objects[0], objects[2], objects[3])
                            };


                        // Random point on a line makes collinear points
                        case RandomPointOnLineFromPoints:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(CollinearPoints, constructedObject, objects[0], objects[1])
                            };

                        // Random point on a circles makes concyclic points
                        case RandomPointOnCircleFromPoints:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(ConcyclicPoints, constructedObject, objects[0], objects[1], objects[2])
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

                        // Circle with center through point makes incidences
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

                        // Here we can't say anything useful
                        case CenterOfCircle:
                        case RandomPoint:
                            return Array.Empty<Theorem>();

                        // Default case
                        default:
                            throw new TheoremProverException($"Unhandled type of predefined construction: {predefinedConstruction.Type}");
                    }

                // If we have a composed construction...
                case ComposedConstruction composedConstruction:

                    // If we haven't found the theorems yet...
                    if (!_originalInstances.ContainsKey(composedConstruction.Name))
                    {
                        // Let the helper method do the job
                        var newTheorems = FindTheoremsForComposedConstruction(composedConstruction);

                        // Add them
                        _composedConstructionsTheorems.Add(composedConstruction, newTheorems);

                        // Mark the used instance
                        _originalInstances.Add(composedConstruction.Name, composedConstruction);
                    }

                    // Get the original instance used for the theorems
                    composedConstruction = _originalInstances[composedConstruction.Name];

                    // Get the theorems
                    var theorems = _composedConstructionsTheorems[composedConstruction];

                    // Create mapping of loose objects of the template configuration 
                    var mapping = composedConstruction.Configuration.LooseObjects.Cast<ConfigurationObject>()
                        // and the constructed object
                        .Concat(composedConstruction.ConstructionOutput)
                        // to the actual objects that created the constructed object and the constructed object
                        .ZipToDictionary(objects.Concat(constructedObject));

                    // Remap all the theorems
                    return theorems.Select(theorem => theorem.Remap(mapping)).ToList();

                // There shouldn't be any other case
                default:
                    throw new TheoremProverException($"Unhandled type of construction: {constructedObject.Construction.GetType()}");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Find all theorems that hold true in the configuration of a given composed construction. 
        /// Only theorems that can be stated with only loose objects are returned.
        /// </summary>
        /// <param name="composedConstruction">The composed construction.</param>
        /// <returns>The theorems.</returns>
        private List<Theorem> FindTheoremsForComposedConstruction(ComposedConstruction composedConstruction)
        {
            // Create an array of loose objects of the defining configuration
            var looseObjects = composedConstruction.Configuration.LooseObjects.ToArray();

            // Create a constructed object representing this construction, i.e. the one that gets passed the loose objects
            var constructedObject = new ConstructedConfigurationObject(composedConstruction, looseObjects);

            // Create a configuration holding the same loose objects, and the final constructed one
            var configuration = new Configuration(composedConstruction.Configuration.LooseObjectsHolder, new[] { constructedObject });

            // Local function that throws an exception about the examination failure
            void ThrowIncorrectConstructionException(string message, Exception innerException = null)
                // informing about the examination failure
                => throw new TheoremProverException($"Cannot examine construction {composedConstruction.Name}. {message}", innerException);

            // Safely execute
            var (pictures, data) = GeneralUtilities.TryExecute(
                // Constructing of the new configuration
                () => _constructor.Construct(configuration),
                // While making sure any exception is caught and re-thrown
                (GeometryConstructionException e) => ThrowIncorrectConstructionException("The defining configuration couldn't be drawn.", e));

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
                (InconstructibleContextualPicture e) => ThrowIncorrectConstructionException("The contextual picture for the defining configuration couldn't be drawn.", e));

            // Find the theorems
            return _finder.FindAllTheorems(contextualPicture).AllObjects
                // For each theorem we need to replace the artificially created
                // object with the original one from the defining configuration
                .Select(theorem =>
                {
                    // We will create a mapping that maps loose objects to itself
                    // and the artificial constructed object to the last object of 
                    // the defining configuration. Take the loose objects first...
                    var mapping = configuration.LooseObjects
                        // Cast each to an identity tuple
                        .Select(looseObject => ((ConfigurationObject)looseObject, (ConfigurationObject)looseObject))
                        // Add the tuple of the constructed and last object
                        .Concat((constructedObject, composedConstruction.ConstructionOutput))
                        // Make the mapping
                        .ToDictionary(pair => pair.Item1, pair => pair.Item2);

                    // Delegate the work to the theorem
                    return theorem.Remap(mapping);
                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}