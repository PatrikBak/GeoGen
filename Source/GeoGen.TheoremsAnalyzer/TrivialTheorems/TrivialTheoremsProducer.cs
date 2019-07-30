using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremsFinder;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private Dictionary<string, List<Theorem>> _composedConstructionsTheorems = new Dictionary<string, List<Theorem>>();

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
                        case PredefinedConstructionType.InternalAngleBisector:
                        {
                            // Create theorem objects
                            var line1 = new TheoremObjectWithPoints(ConfigurationObjectType.Line, objects[0], objects[1]);
                            var line2 = new TheoremObjectWithPoints(ConfigurationObjectType.Line, objects[0], objects[2]);
                            var bisector = new TheoremObjectWithPoints(ConfigurationObjectType.Line, constructedObject, objects[0].ToEnumerable());

                            // Create the theorem
                            return new[] { new Theorem(configuration, TheoremType.EqualAngles, line1, bisector, line2, bisector) };
                        }

                        // Point reflection makes collinear points and equally distances points
                        case PredefinedConstructionType.PointReflection:
                        {
                            // Create theorem objects
                            var point1 = new TheoremPointObject(objects[0]);
                            var point2 = new TheoremPointObject(objects[1]);
                            var reflectedPoint = new TheoremPointObject(constructedObject);

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(configuration, TheoremType.CollinearPoints, point1, point2, reflectedPoint),

                                // Equal distances
                                new Theorem(configuration, TheoremType.EqualLineSegments, point2, point1, point2, reflectedPoint)
                            };
                        }

                        // Point reflection makes collinear points and equally distances points
                        case PredefinedConstructionType.Midpoint:
                        {
                            // Create theorem objects
                            var point1 = new TheoremPointObject(objects[0]);
                            var point2 = new TheoremPointObject(objects[1]);
                            var midpoint = new TheoremPointObject(constructedObject);

                            // Create the theorems
                            return new[]
                            {
                                // Collinearity
                                new Theorem(configuration, TheoremType.CollinearPoints, point1, point2, midpoint),

                                // Equal distances
                                new Theorem(configuration, TheoremType.EqualLineSegments, point1, midpoint, point2, midpoint)
                             };
                        }

                        // Perpendicular projection makes perpendicular lines
                        case PredefinedConstructionType.PerpendicularProjection:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.PerpendicularLines, new TheoremObject[]
                                {
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, objects[1]),
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, constructedObject, objects[0])
                                })
                            };

                        // Perpendicular line makes (surprisingly) perpendicular lines
                        case PredefinedConstructionType.PerpendicularLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.PerpendicularLines, new TheoremObject[]
                                {
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, constructedObject),
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, objects[1], objects[0].ToEnumerable())
                                })
                            };

                        // Parallel line makes (surprisingly) parallel lines
                        case PredefinedConstructionType.ParallelLine:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.ParallelLines, new TheoremObject[]
                                {
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, constructedObject),
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, objects[1], objects[0].ToEnumerable())
                                })
                            };

                        // Second intersection of two circumcircles makes concyclic points
                        case PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.ConcyclicPoints, constructedObject, objects[0], objects[1], objects[2]),
                                new Theorem(configuration, TheoremType.ConcyclicPoints, constructedObject, objects[0], objects[3], objects[4])
                            };

                        // Second intersection of a circle and line from point make collinear and concyclic points
                        case PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints:

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.CollinearPoints, objects[0], objects[1], constructedObject),
                                new Theorem(configuration, TheoremType.ConcyclicPoints, objects[0], objects[2], objects[3], constructedObject)
                            };


                        // Second intersection of a circle with center and line from points makes collinear and equally distanced points
                        case PredefinedConstructionType.SecondIntersectionOfCircleWithCenterAndLineFromPoints:

                            // Create theorem objects
                            var A = new TheoremPointObject(objects[0]);
                            var B = new TheoremPointObject(objects[1]);
                            var C = new TheoremPointObject(objects[2]);
                            var D = new TheoremPointObject(constructedObject);

                            // Create the theorems
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.CollinearPoints, A, B, D),
                                new Theorem(configuration, TheoremType.EqualLineSegments, C, A, C, D)
                            };


                        // Random point on a line makes collinear points
                        case PredefinedConstructionType.RandomPointOnLineFromPoints:

                            // Create the theorem
                            return new[]
                            {
                                new Theorem(configuration, TheoremType.CollinearPoints, objects[0], objects[1], constructedObject)
                            };

                        // In all the other cases we can't say anything useful :/
                        default:
                            return new Theorem[0];
                    }

                // If we have a composed construction...
                case ComposedConstruction composedConstruction:

                    // Get the theorems for it 
                    var theorems = _composedConstructionsTheorems.GetOrAdd(composedConstruction.Name, () => FindTheoremsForComposedConstruction(composedConstruction));

                    // Create mapping of loose objects of the template configuration + the constructed object
                    var mapping = composedConstruction.Configuration.LooseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().Concat(composedConstruction.ConstructionOutput)
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
            var looseObjects = composedConstruction.Configuration.LooseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().ToArray();

            // Create a constructed object representing this construction
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
                contextualPicture = _pictureFactory.Create(configuration, geometryData.Manager);
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
                    // Use the remap method. In that case we just need to remap a single theorem object
                    return theorem.Remap(theoremObject =>
                    {
                        // Helper function that replaces an object, if it is the one to be replaced,
                        // and otherwise returns the original object
                        ConfigurationObject Replace(ConfigurationObject o) => o == constructedObject ? composedConstruction.ConstructionOutput : o;

                        // Switch based on the type
                        switch (theoremObject.Type)
                        {
                            // If we have a point, then we replace its object 
                            case ConfigurationObjectType.Point:
                                return new TheoremPointObject(Replace(theoremObject.ConfigurationObject));

                            // If we have a line or circle...
                            case ConfigurationObjectType.Line:
                            case ConfigurationObjectType.Circle:

                                // The object is an object with points
                                var objectWithPoints = (TheoremObjectWithPoints) theoremObject;

                                // We replace its points
                                var points = objectWithPoints.Points.Select(Replace).ToList();

                                // And create it with a replaced object
                                return new TheoremObjectWithPoints(theoremObject.Type, Replace(theoremObject.ConfigurationObject), points);

                            // Otherwise make the developer aware
                            default:
                                throw new GeoGenException($"Unhandled theorem object type: {theoremObject.Type}");
                        }
                    });
                })
                // Take only remaining theorems
                .Where(theorem => theorem != null)
                // Enumerate
                .ToList();
        }

        #endregion
    }
}
