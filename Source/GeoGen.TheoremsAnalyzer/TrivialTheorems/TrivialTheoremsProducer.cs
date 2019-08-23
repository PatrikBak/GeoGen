using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremsFinder;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// The default implementation of <see cref="ITrivialTheoremsProducer"/>.
    /// This implementation caches trivial theorems that are true for general
    /// <see cref="ComposedConstruction"/>s.
    /// </summary>
    public class TrivialTheoremsProducer : ITrivialTheoremsProducer
    {
        #region Dependencies

        /// <summary>
        /// The constructor used to determine theorems of composed constructions.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The finders used to determine theorems of composed constructions.
        /// </summary>
        private readonly ITheoremsFinder[] _finders;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping the names of <see cref="ComposedConstruction"/>
        /// to the theorems holding true in <see cref="ComposedConstruction.Configuration"/>.
        /// Only theorems that can be then remapped to trivial theorem are stored, i.e. the ones
        /// that can be stated with just the loose objects of the <see cref="ComposedConstruction.Configuration"/>.
        /// </summary>
        private readonly Dictionary<ComposedConstruction, List<Theorem>> _composedConstructionsTheorems;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrivialTheoremsProducer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor used to determine theorems of composed constructions.</param>
        /// <param name="finders">The finders used to determine theorems of composed constructions.</param>
        public TrivialTheoremsProducer(IGeometryConstructor constructor, ITheoremsFinder[] finders)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _finders = finders ?? throw new ArgumentNullException(nameof(finders));
            _composedConstructionsTheorems = new Dictionary<ComposedConstruction, List<Theorem>>();
        }

        #endregion

        #region ITrivialTheoremsProducer

        /// <summary>
        /// Derive trivial theorems from the last object of a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem should hold.</param>
        /// <returns>The trivial theorems.</returns>
        public IReadOnlyList<Theorem> DeriveTrivialTheoremsFromLastObject(Configuration configuration)
        {
            // Get the last object of the configuration
            var constructedObject = configuration.ConstructedObjects.LastOrDefault();

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
                        // An angle bisector makes equal angles
                        case InternalAngleBisector:
                        {
                            // Create theorem objects
                            var line1 = new LineTheoremObject(objects[0], objects[1]);
                            var line2 = new LineTheoremObject(objects[0], objects[2]);
                            var bisector = new LineTheoremObject(constructedObject, points: new[] { objects[0] });

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, EqualAngles, new []
                                {
                                    new AngleTheoremObject(line1, bisector),
                                    new AngleTheoremObject(line2, bisector)
                                })
                            };
                        }

                        // Point reflection makes collinear points and equally distances points
                        case PointReflection:

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(configuration, CollinearPoints, objects[0], objects[1], constructedObject),

                                // Equal distances
                                new Theorem(configuration, EqualLineSegments, new TheoremObject[]
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
                                new Theorem(configuration, CollinearPoints, objects[0], objects[1], constructedObject),

                                // Equal distances
                                new Theorem(configuration, EqualLineSegments, new TheoremObject[]
                                {
                                    new LineSegmentTheoremObject(objects[0], constructedObject),
                                    new LineSegmentTheoremObject(objects[1], constructedObject)
                                })
                             };

                        // Perpendicular projection makes perpendicular lines
                        case PerpendicularProjection:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(objects[0], constructedObject),
                                    new LineTheoremObject(objects[1], points: new[] {constructedObject })
                                })
                            };

                        // Perpendicular line makes (surprisingly) perpendicular lines
                        case PerpendicularLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, PerpendicularLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject, points: new[] {objects[0] }),
                                    new LineTheoremObject(objects[1])
                                })
                            };

                        // Parallel line makes (surprisingly) parallel lines
                        case ParallelLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, ParallelLines, new TheoremObject[]
                                {
                                    new LineTheoremObject(constructedObject, points: new[] { objects[0] }),
                                    new LineTheoremObject(objects[1])
                                })
                            };

                        // Second intersection of two circumcircles makes concyclic points
                        case SecondIntersectionOfTwoCircumcircles:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, ConcyclicPoints, constructedObject, objects[0], objects[1], objects[2]),
                                new Theorem(configuration, ConcyclicPoints, constructedObject, objects[0], objects[3], objects[4])
                            };

                        // Second intersection of a circle and line from point make collinear and concyclic points
                        case SecondIntersectionOfCircleAndLineFromPoints:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, CollinearPoints, constructedObject, objects[0], objects[1]),
                                new Theorem(configuration, ConcyclicPoints, constructedObject, objects[0], objects[2], objects[3])
                            };


                        // Second intersection of a circle with center and line from points makes collinear and equally distanced points
                        case SecondIntersectionOfCircleWithCenterAndLineFromPoints:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, CollinearPoints, constructedObject, objects[0], objects[1]),
                                new Theorem(configuration, EqualLineSegments, new[]
                                {
                                    new LineSegmentTheoremObject(objects[2], objects[0]),
                                    new LineSegmentTheoremObject(objects[2], constructedObject)
                                })
                            };


                        // Random point on a line makes collinear points
                        case RandomPointOnLineFromPoints:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, CollinearPoints, constructedObject, objects[0], objects[1])
                            };

                        // In all the other cases we can't say anything useful :/
                        default:
                            return Array.Empty<Theorem>();
                    }

                // If we have a composed construction...
                case ComposedConstruction composedConstruction:

                    // Get the theorems for it 
                    var theorems = _composedConstructionsTheorems.GetOrAdd(composedConstruction, () => FindTheoremsForComposedConstruction(composedConstruction));

                    // Create mapping of loose objects of the template configuration + the constructed object
                    var mapping = composedConstruction.Configuration.LooseObjects.Cast<ConfigurationObject>().Concat(composedConstruction.ConstructionOutput)
                        // to the actual objects that created the constructed object + the constructed object
                        .ZipToDictionary(objects.Concat(constructedObject));

                    // Remap all the theorems
                    return theorems.Select(theorem => theorem.Remap(mapping)).ToList();

                // There shouldn't be any other case
                default:
                    throw new TheoremsAnalyzerException($"Unhandled type of construction: {constructedObject.Construction.GetType()}");
            }
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Find all theorems that hold true in the configuration of a given composed construction. 
        /// Only theorems that can be stated with only loose objects are returned.
        /// </summary>
        /// <param name="composedConstruction">The composed construction.</param>
        /// <returns>The theorems.</returns>
        private List<Theorem> FindTheoremsForComposedConstruction(ComposedConstruction composedConstruction)
        {
            // Create an array of loose objects of the defining configuration
            var looseObjects = composedConstruction.Configuration.LooseObjects.Cast<ConfigurationObject>().ToArray();

            // Create a constructed object representing this construction, i.e. the one that gets passed the loose objects
            var constructedObject = new ConstructedConfigurationObject(composedConstruction, looseObjects);

            // Create a configuration holding the same loose objects, and the final constructed one
            var configuration = new Configuration(composedConstruction.Configuration.LooseObjectsHolder, new[] { constructedObject });

            // Local function that throws an exception
            void ThrowIncorrectConstructionException(string message, Exception e = null)
                // informing about the examination failure
                => throw new TheoremsAnalyzerException($"Cannot examine construction {composedConstruction.Name}. {message}", e);

            // Safely execute
            var (pictures, data) = GeneralUtilities.TryExecute(
                // Constructing of the new object
                () => _constructor.Construct(configuration),
                // Make sure the exception is caught and re-thrown
                (GeometryConstructionException e) => ThrowIncorrectConstructionException("The defining configuration couldn't be drawn.", e));

            // Make sure it has no inconstructible objects
            if (data.InconstructibleObject != default)
                ThrowIncorrectConstructionException("The defining configuration contains an inconstructible object.");

            // Make sure it has no duplicates
            if (data.Duplicates != default)
                ThrowIncorrectConstructionException("The defining configuration contains duplicate objects.");

            // Safely execute
            var contextualPicture = GeneralUtilities.TryExecute(
                // Construct of the contextual picture
                () => new ContextualPicture(pictures),
                // Make sure the exception is caught and re-thrown
                (InconstructibleContextualPicture e) => ThrowIncorrectConstructionException("The contextual picture for the defining configuration couldn't be drawn.", e));

            // Find the theorems
            return _finders.SelectMany(finder => finder.FindAllTheorems(contextualPicture))
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
                        // Make a mapping
                        .ToDictionary(pair => pair.Item1, pair => pair.Item2);

                    // Use theorems method to do the job
                    return theorem.Remap(mapping);
                })
                // Enumerate
                .ToList();
        }

        #endregion
    }
}
