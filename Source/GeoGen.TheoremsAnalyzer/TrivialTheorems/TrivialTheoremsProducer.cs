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
        /// The factory for creating contextual pictures used to determine theorems of composed constructions.
        /// </summary>
        private readonly IContextualPictureFactory _pictureFactory;

        /// <summary>
        /// The constructor used to determine theorems of composed constructions.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The finder used to determine theorems of composed constructions.
        /// </summary>
        private readonly IRelevantTheoremsAnalyzer _finder;

        #endregion

        #region Private fields

        /// <summary>
        /// The cache dictionary mapping the names of <see cref="ComposedConstruction"/>
        /// to the theorems holding true in <see cref="ComposedConstruction.Configuration"/>.
        /// Only theorems that can be then remapped to trivial theorem are stored, i.e. the ones
        /// that can be stated with just the loose objects of the <see cref="ComposedConstruction.Configuration"/>.
        /// </summary>
        private Dictionary<ComposedConstruction, List<Theorem>> _composedConstructionsTheorems;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrivialTheoremsProducer"/> class.
        /// </summary>
        /// <param name="pictureFactory">The factory for creating contextual pictures used to determine theorems of composed constructions.</param>
        /// <param name="constructor">The constructor used to determine theorems of composed constructions.</param>
        /// <param name="finder">The finder used to determine theorems of composed constructions.</param>
        public TrivialTheoremsProducer(IContextualPictureFactory pictureFactory, IGeometryConstructor constructor, IRelevantTheoremsAnalyzer finder)
        {
            _pictureFactory = pictureFactory ?? throw new ArgumentNullException(nameof(pictureFactory));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));

            // Create the dictionary 
            _composedConstructionsTheorems = new Dictionary<ComposedConstruction, List<Theorem>>(
                // That compares composed constructions by their names
                new SimpleEqualityComparer<ComposedConstruction>((c1, c2) => c1.Name == c2.Name, c => c.GetHashCode()));
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
                return new Theorem[0];

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
                            return new Theorem[0];
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
                    return theorems.Select(t => t.Remap(mapping)).ToList();

                // There shouldn't be any other case
                default:
                    throw new GeoGenException($"Unhandled type of construction: {constructedObject.Construction.GetType()}");
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

            // Construct the defining configuration
            var geometryData = _constructor.Construct(configuration);

            // Make sure it is correct
            if (!geometryData.SuccessfullyExamined || geometryData.InconstructibleObject != null || geometryData.Duplicates != (null, null))
                throw new GeoGenException($"Cannot examine construction {composedConstruction.Name}.");

            // Prepare its contextual picture
            var contextualPicture = default(IContextualPicture);

            try
            {
                // Try to construct it
                contextualPicture = _pictureFactory.Create(configuration.AllObjects, geometryData.Manager);
            }
            catch (InconstructibleContextualPicture)
            {
                // Make the developer aware
                throw new GeoGenException($"Cannot examine construction {composedConstruction.Name}.");
            }

            // Find the theorems
            return _finder.Analyze(configuration, geometryData.Manager, contextualPicture)
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
