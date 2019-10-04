using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// A default implementation of <see cref="ISubtheoremDeriver"/>. 
    /// 
    /// Supported <see cref="LooseObjectsLayout"/> of <see cref="SubtheoremDeriverInput.TemplateTheorems"/>:
    /// 
    ///  - TwoPoints
    ///  - ThreePoints
    ///  - FourPoints
    ///  - FourConcyclicPoints (uses <see cref="ContextualPicture"/> to get the concyclic points)
    ///  - Trapezoid (uses <see cref="ParallelLines"/>)
    ///  - RightTriangle (uses <see cref="PerpendicularLine"/>)
    ///  - CircleAndTangentLine (uses <see cref="LineTangentToCircle"/>)
    ///  - IsoscelesTriangle (uses <see cref="EqualLineSegments"/>)
    ///  
    /// If the theorems needed for a specific layout aren't provided (for example, because there are not being found),
    /// then the deriver will not match the layout.
    /// </summary>
    public class SubtheoremDeriver : ISubtheoremDeriver
    {
        #region MappingData class

        /// <summary>
        /// A helper class holding data about a mapping between objects of the 
        /// template configuration and the examined configuration.
        /// </summary>
        private class MappingData
        {
            #region Public properties

            /// <summary>
            /// The mapping of objects of the template theorem's configuration and objects of the examined theorem's configuration.
            /// </summary>
            public Dictionary<ConfigurationObject, ConfigurationObject> Mapping { get; }

            /// <summary>
            /// The list of used object equalities found while obtaining this mapping.
            /// </summary>
            public List<(ConfigurationObject originalObject, ConfigurationObject equalObject)> EqualObjects { get; }

            /// <summary>
            /// The list of pairs of (point, configurationObject), such that the point
            /// lies on the object, that have been used to come up with the mapping.
            /// </summary>
            public List<(ConfigurationObject point, ConfigurationObject lineOrCircle)> UsedIncidencies { get; }

            /// <summary>
            /// The list of theorems that have been taken from input that have been used to come up with the mapping.
            /// </summary>
            public List<Theorem> UsedFacts { get; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a instance of the <see cref="MappingData"/> class with no equalities,
            /// no incidences, and passed facts.
            /// </summary>
            /// <param name="templateObjects">The loose objects that are mapped.</param>
            /// <param name="mappedObjects">The objects to which the loose objects are mapped.</param>
            /// <param name="facts">The initial facts to be included in the mapping.</param>
            public MappingData(IReadOnlyList<LooseConfigurationObject> templateObjects, IEnumerable<ConfigurationObject> mappedObjects, params Theorem[] facts)
            {
                // Create the mapping by the template and mapped objects one by one
                Mapping = templateObjects.Cast<ConfigurationObject>().ZipToDictionary(mappedObjects);

                // Create empty equal objects and incidences
                EqualObjects = new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>();
                UsedIncidencies = new List<(ConfigurationObject point, ConfigurationObject lineOrCircle)>();

                // Wrap the passed facts
                UsedFacts = new List<Theorem>(facts);
            }

            /// <summary>
            /// Initializes a instance of the <see cref="MappingData"/> class by cloning a given 
            /// mapping, adding a new mapped pair to it, and adding passed facts to it.
            /// </summary>
            /// <param name="mapping">The mapping to be cloned.</param>
            /// <param name="newMappedPair">The new mapped pair to be included in the mapping.</param>
            /// <param name="facts">The new facts to be included in the mapping.</param>
            public MappingData(MappingData mapping, (ConfigurationObject, ConfigurationObject) newMappedPair, params Theorem[] facts)
            {
                // Clone the mapping
                Mapping = mapping.Mapping.ToDictionary(pair => pair.Key, pair => pair.Value);

                // Add the new pair
                Mapping.Add(newMappedPair.Item1, newMappedPair.Item2);

                // Clone equal objects and incidences
                EqualObjects = mapping.EqualObjects.ToList();
                UsedIncidencies = mapping.UsedIncidencies.ToList();

                // Clone the facts and add the passed one
                UsedFacts = mapping.UsedFacts.Concat(facts).ToList();
            }

            /// <summary>
            /// Initializes a instance of the <see cref="MappingData"/> class by cloning a given 
            /// mapping, adding a new mapped pair to it, and adding a passed incidence to it. 
            /// </summary>
            /// <param name="mapping">The mapping to be cloned.</param>
            /// <param name="newMappedPair">The new mapped pair to be included in the mapping.</param>
            /// <param name="incidence">The new incidence to be included in the mapping.</param>
            public MappingData(MappingData mapping, (ConfigurationObject, ConfigurationObject) newMappedPair, (ConfigurationObject point, ConfigurationObject lineOrCircle) incidence)
                : this(mapping, newMappedPair)
            {
                // Add the passed incidence
                UsedIncidencies.Add(incidence);
            }

            #endregion
        }

        #endregion

        #region Dependencies

        /// <summary>
        /// The constructor of geometric objects that allows us to find geometrical properties of the mappings.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremDeriver"/> class.
        /// </summary>
        /// <param name="constructor">The constructor of geometric objects that allows us to find geometrical properties of the mappings.</param>
        public SubtheoremDeriver(IGeometryConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region ISubtheoremDeriver implementation

        /// <summary>
        /// Performs the sub-theorem derivation on a given input.
        /// </summary>
        /// <param name="input">The input for the deriver.</param>
        /// <returns>The derived theorems wrapped in output objects.</returns>
        public IEnumerable<SubtheoremDeriverOutput> DeriveTheorems(SubtheoremDeriverInput input)
        {
            #region Check if we have enough objects to do the mapping

            // Find it out by going through all the (type, objects) pairs of the template configuration
            var doWeHaveEnoughObjects = input.TemplateConfiguration.ObjectMap.All(pair =>
            {
                // Deconstruct
                var (type, templateObjects) = pair;

                // Get the number examined objects of this type
                var numberOfNumbers = input.ExaminedConfigurationPicture.Pictures.Configuration.ObjectMap.GetOrDefault(type)?.Count ?? 0;

                // This type is fine if there at least as many objects as the template ones
                return numberOfNumbers >= templateObjects.Count;
            });

            // If we don't have enough objects
            if (!doWeHaveEnoughObjects)
                // Then there will definitely be no derived theorems
                return Enumerable.Empty<SubtheoremDeriverOutput>();

            #endregion

            // Use the helper method to generate possible mappings between the loose objects of the
            // template configurations and objects of the examined configuration
            return GenerateInitialMappings(input)
                // Each of these mappings can yield more mappings when other objects are considered
                .SelectMany(mapping =>
                {
                    // Take the constructed objects of the template configuration
                    return input.TemplateConfiguration.ConstructedObjects
                        // We're going to gradually map each of them. Since each of 
                        // them can yield more mappings, we will use aggregation. 
                        // At each step we have a list of correct mappings
                        // (initially the list containing only the current one).
                        // Then we use our helper function to generate mappings
                        // that include this object.
                        .Aggregate(

                            // Initially we only have this mapping
                            mapping.ToEnumerable(),

                            // In each step we have our current mappings and the current object
                            (currentMappings, constructedObject) =>
                            {
                                // For each of the current mapping we use our helper method to include the current object
                                return currentMappings.SelectMany(innerMapping => GenerateMappingsIncludingObject(innerMapping, constructedObject, input));
                            }
                        );
                })
                // Now we have correct mappings. We generate theorems from each
                .Select(data => (data, derivedTheorems:
                            // To get theorems take all template ones
                            input.TemplateTheorems.AllObjects
                            // Try to remap each, persevering the info about the template one
                            .Select(theorem => (remppedTheorem: theorem.Remap(data.Mapping), templateTheorem: theorem))
                            // Take those where it could be done
                            .Where(pair => pair.remppedTheorem != null)
                            // Enumerate
                            .ToList()))
                // Take only those remapping that have any theorem
                .Where(pair => pair.derivedTheorems.Any())
                // Those represent a correct result
                .Select(pair => new SubtheoremDeriverOutput
                (
                    derivedTheorems: pair.derivedTheorems,
                    usedEqualities: pair.data.EqualObjects,
                    usedFacts: pair.data.UsedFacts,
                    usedIncidencies: pair.data.UsedIncidencies
                ))
                // Don't take outputs with the same theorems and reasoning
                .Distinct();
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappings(SubtheoremDeriverInput input)
        {
            // Return the answer based on the layout of the template theorem
            return input.TemplateConfiguration.LooseObjectsHolder.Layout switch
            {
                // Trapezoid
                Trapezoid => GenerateInitialMappingsForTrapezoidLayout(input),

                // Triangle
                // TODO: collinearity
                ThreePoints => GenerateInitialMappingsWithPoints(input),

                // Quadrilateral
                // TODO: collinearity
                FourPoints => GenerateInitialMappingsWithPoints(input),

                // Concyclic points
                FourConcyclicPoints => GenerateInitialMappingsForFourConcyclicPoints(input),

                // Line segment
                TwoPoints => GenerateInitialMappingsWithPoints(input),

                // Right triangle
                RightTriangle => GenerateInitialMappingsForRightTriangleLayout(input),

                // Tangent line 
                CircleAndTangentLine => GenerateInitialMappingsForCircleAndTangentLineLayout(input),

                // Isosceles triangle
                // TODO: collinearity
                IsoscelesTriangle => GenerateInitialMappingsForIsoscelesTriangleLayout(input),

                // Default case
                _ => throw new TheoremProverException($"Unhandled type of loose objects layout: {input.TemplateConfiguration.LooseObjectsHolder.Layout}")
            };
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration consists of points only.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsWithPoints(SubtheoremDeriverInput input)
        {
            // Take the points 
            return input.ExaminedConfigurationPicture.AllPoints
                // And their n-tuples, where n is the inferred number of points
                .Variations(input.TemplateConfiguration.LooseObjects.Count)
                // Unwrap configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject))
                // Each represent a mapping
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects the template theorem in case where the layout of the template
        /// configuration is <see cref="FourConcyclicPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForFourConcyclicPoints(SubtheoremDeriverInput input)
        {
            // Take all circles
            return input.ExaminedConfigurationPicture.AllCircles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // Take all 4-elements variations for each of them
                .SelectMany(circle => circle.Points.Variations(4))
                // Unwrap configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each represent a possible mapping
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points,
                        // With the used concyclity theorem
                        new Theorem(ConcyclicPoints, points)));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="Trapezoid"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForTrapezoidLayout(SubtheoremDeriverInput input)
        {
            // Take the parallel lines theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(ParallelLines)
                // Where both the lines objects are defined by points
                ?.Where(theorem => theorem.InvolvedObjects.All(line => ((LineTheoremObject)line).DefinedByPoints))
                // From each theorem object unwrap points
                .Select(theorem => (theorem,
                    // We now know we have two lines defined by points
                    points1: ((LineTheoremObject)theorem.InvolvedObjectsList[0]).Points,
                    points2: ((LineTheoremObject)theorem.InvolvedObjectsList[1]).Points
                ))
                // Flatten them
                .Select(pair => (pair.theorem, points: pair.points1.Concat(pair.points2).ToArray()))
                // Reorder to get every possible valid mapping
                .SelectMany(pair => new[]
                {
                    (pair.theorem, points: new[] { pair.points[0], pair.points[1], pair.points[2], pair.points[3] }),
                    (pair.theorem, points: new[] { pair.points[1], pair.points[0], pair.points[2], pair.points[3] }),
                    (pair.theorem, points: new[] { pair.points[0], pair.points[1], pair.points[3], pair.points[2] }),
                    (pair.theorem, points: new[] { pair.points[1], pair.points[0], pair.points[3], pair.points[2] }),
                    (pair.theorem, points: new[] { pair.points[2], pair.points[3], pair.points[0], pair.points[1] }),
                    (pair.theorem, points: new[] { pair.points[2], pair.points[3], pair.points[1], pair.points[0] }),
                    (pair.theorem, points: new[] { pair.points[3], pair.points[2], pair.points[0], pair.points[1] }),
                    (pair.theorem, points: new[] { pair.points[3], pair.points[2], pair.points[1], pair.points[0] })
                })
                // These points finally represent one of our sought trapezoids
                .Select(pair => new MappingData(input.TemplateConfiguration.LooseObjects, pair.points, pair.theorem))
                // If there are no parallel line theorems, we have no mapping
                ?? Enumerable.Empty<MappingData>();
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="RightTriangle"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForRightTriangleLayout(SubtheoremDeriverInput input)
        {
            // Take the perpendicular lines theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(PerpendicularLines)
                // Where both the lines objects are defined by points
                ?.Where(theorem => theorem.InvolvedObjects.All(line => ((LineTheoremObject)line).DefinedByPoints))
                // From each theorem object unwrap points
                .Select(theorem => (theorem,
                    // We now know we have two lines defined by points
                    points1: ((LineTheoremObject)theorem.InvolvedObjectsList[0]).Points,
                    points2: ((LineTheoremObject)theorem.InvolvedObjectsList[1]).Points
                ))
                // Find their common point (should be at most one)
                .Select(pair => (pair.theorem, pair.points1, pair.points2, commonPoint: pair.points1.Intersect(pair.points2).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(tuple => tuple.commonPoint != null)
                // Get those points on particular lines that are distinct from the common one
                .Select(triple => (triple.theorem, triple.commonPoint,
                    // Each line should have one other point
                    otherPoint1: triple.points1.Where(point => point != triple.commonPoint).First(),
                    otherPoint2: triple.points2.Where(point => point != triple.commonPoint).First()
                ))
                // Reorder to get every possible valid mapping
                .SelectMany(triple => new[]
                {
                    (triple.theorem, points: new[] { triple.commonPoint, triple.otherPoint1, triple.otherPoint2 }),
                    (triple.theorem, points: new[] { triple.commonPoint, triple.otherPoint2, triple.otherPoint1 })
                })
                // These points finally represent one of our sought right triangle
                .Select(pair => new MappingData(input.TemplateConfiguration.LooseObjects, pair.points, pair.theorem))
                // If there are no perpendicular line theorems, we have no mapping
                ?? Enumerable.Empty<MappingData>();
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="CircleAndTangentLine"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForCircleAndTangentLineLayout(SubtheoremDeriverInput input)
        {
            // Take the line tangent to circle theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(LineTangentToCircle)
                // Where both line and circle are defined by points
                ?.Where(theorem => theorem.InvolvedObjects.All(lineOrCircle => ((TheoremObjectWithPoints)lineOrCircle).DefinedByPoints))
                // From particular theorem objects unwrap points
                .Select(theorem => (theorem, lineCirclePoints: new[]
                {
                    theorem.InvolvedObjects.OfType<LineTheoremObject>().First().Points,
                    theorem.InvolvedObjects.OfType<CircleTheoremObject>().First().Points
                }))
                // Find their common points (should be at most one)
                .Select(pair => (pair.theorem, pointArrays: pair.lineCirclePoints, commonPoint: pair.lineCirclePoints[0].Intersect(pair.lineCirclePoints[1]).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(triple => triple.commonPoint != null)
                // Get those points on particular objects that are distinct from the common one
                .Select(triple => (triple.theorem, triple.commonPoint,
                    // The line has only one other point
                    linePoint: triple.pointArrays[0].Where(point => point != triple.commonPoint).First(),
                    // The circle has 2 other points
                    circlePoints: triple.pointArrays[1].Where(point => point != triple.commonPoint).ToArray()
                ))
                // Reorder to get every possible valid mapping
                .SelectMany(tuple => new[]
                {
                    (tuple.theorem, points: new[] { tuple.commonPoint, tuple.circlePoints[0], tuple.circlePoints[1], tuple.linePoint }),
                    (tuple.theorem, points: new[] { tuple.commonPoint, tuple.circlePoints[1], tuple.circlePoints[0], tuple.linePoint })
                })
                // These points finally represent one of our sought situation
                .Select(pair => new MappingData(input.TemplateConfiguration.LooseObjects, pair.points, pair.theorem))
                // If there are no line tangent to circle theorems, we have no mapping
                ?? Enumerable.Empty<MappingData>();
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="IsoscelesTriangle"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForIsoscelesTriangleLayout(SubtheoremDeriverInput input)
        {
            // Take the equal line segments theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(EqualLineSegments)
                // From each theorem object unwrap line segments
                ?.Select(theorem => (theorem, lineSegments: new[]
                {
                    (LineSegmentTheoremObject)theorem.InvolvedObjectsList[0],
                    (LineSegmentTheoremObject)theorem.InvolvedObjectsList[1],
                }))
                // And configuration objects from them
                .Select(pair => (pair.theorem,
                    // Use the helper method
                    points1: pair.lineSegments[0].GetInnerConfigurationObjects().ToArray(),
                    points2: pair.lineSegments[1].GetInnerConfigurationObjects().ToArray()))
                // Find their common points (should be at most one)
                .Select(pair => (pair.theorem, pair.points1, pair.points2, commonPoint: pair.points1.Intersect(pair.points2).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(tuple => tuple.commonPoint != null)
                // Find the other point for 
                .Select(tuple => (tuple.theorem, tuple.commonPoint,
                    // There should be exactly one other point
                    otherPoint1: tuple.points1.Where(point => point != tuple.commonPoint).First(),
                    otherPoint2: tuple.points2.Where(point => point != tuple.commonPoint).First()
                ))
                // Reorder to get every possible valid mapping
                .SelectMany(triple => new[]
                {
                    (triple.theorem, points: new[] {triple.commonPoint, triple.otherPoint1, triple.otherPoint2}),
                    (triple.theorem, points: new[] {triple.commonPoint, triple.otherPoint2, triple.otherPoint1})
                })
                // These points finally represent one of our sought situation
                .Select(pair => new MappingData(input.TemplateConfiguration.LooseObjects, pair.points, pair.theorem))
                // If there are no equal line segments theorems, we have no mapping
                ?? Enumerable.Empty<MappingData>();
        }

        /// <summary>
        /// Extends a given mapping by including a given constructed objects.
        /// </summary>
        /// <param name="data">The current data where we want to add the <paramref name="constructedObject"/>.</param>
        /// <param name="constructedObject">The object that should be currently mapped.</param>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateMappingsIncludingObject(MappingData data, ConstructedConfigurationObject constructedObject, SubtheoremDeriverInput input)
        {
            #region Creating the mapped object

            // We need to remap the arguments of the constructed objects with respect to the provided mapping
            var mappedObjects = constructedObject.PassedArguments.FlattenedList.Select(o => data.Mapping[o]).ToArray();

            // Make sure there are no two equal objects. If yes, the mapping is incorrect
            if (mappedObjects.AnyDuplicates())
                return Enumerable.Empty<MappingData>();

            // Create new arguments for the remapped object.
            var newArguments = constructedObject.Construction.Signature.Match(mappedObjects);

            // Finally create the object
            var mappedObject = new ConstructedConfigurationObject(constructedObject.Construction, newArguments);

            #endregion

            #region Handling mapping if there are more options for mapping this object

            // Get the configuration for comfort 
            var configuration = input.ExaminedConfigurationPicture.Pictures.Configuration;

            // Then we need to have a 'Random' construction:
            switch (mappedObject.Construction)
            {
                // If we can choose our point completely at random...
                case PredefinedConstruction p when p.Type == RandomPoint:

                    // TODO: Wouldn't it be better not to allow duplicates? 
                    // In general. Shouldn't the remapped theorems be always correct
                    // no matter what?

                    // Then we do so by pulling the map and looking at points
                    return configuration.ObjectMap.GetOrDefault(ConfigurationObjectType.Point)?
                        // And using our helper function
                        .Select(point => new MappingData(data, (constructedObject, point)))
                        // In case there in no point (which I doubt that will ever happen), return nothing
                        ?? Enumerable.Empty<MappingData>();

                // If the construction is predefined...
                case PredefinedConstruction predefinedConstruction when
                    // And it's a random point on an explicit line
                    predefinedConstruction.Type == RandomPointOnLine
                    // Or on a circle
                    || predefinedConstruction.Type == RandomPointOnCircle:

                    // Get the input line or line
                    var inputLineOrCircle = mappedObject.PassedArguments.FlattenedList[0];

                    // Get the geometric line or circle
                    return ((DefinableByPoints)input.ExaminedConfigurationPicture.GetGeometricObject(inputLineOrCircle))
                        // Unwrap configuration points
                        .Points.Select(point => point.ConfigurationObject)
                        // Any of its points does the job
                        .Select(point => new MappingData(data, (constructedObject, point),
                            // But we need an incidence theorem to state so
                            incidence: (point, inputLineOrCircle)));

                // If the construction is predefined...
                case PredefinedConstruction predefinedConstruction when
                    // And it's a random point on a line from points
                    predefinedConstruction.Type == RandomPointOnLineFromPoints
                    // Or on a circle
                    || predefinedConstruction.Type == RandomPointOnCircleFromPoints:

                    // Get the input points
                    var inputPoints = mappedObject.PassedArguments.FlattenedList;

                    // Get the geometric objects corresponding to the input points
                    var geometricPoints = inputPoints
                        // For each find the geometric point
                        .Select(input.ExaminedConfigurationPicture.GetGeometricObject)
                        // Cast
                        .Cast<PointObject>()
                        // Enumerate
                        .ToArray();

                    // Prepare the type of the used fact
                    var type = predefinedConstruction.Type switch
                    {
                        // Line means collinearity
                        RandomPointOnLineFromPoints => CollinearPoints,

                        // Circle means concyclity
                        RandomPointOnCircleFromPoints => ConcyclicPoints,

                        // Impossible, we're down to these 2 cases
                        _ => throw new TheoremProverException("Impossible.")
                    };

                    // First find the geometric line/circle corresponding that pass through the found points
                    return (predefinedConstruction.Type switch
                    {
                        // Looking for the line that contains our points
                        RandomPointOnLineFromPoints => geometricPoints[0].Lines.First(line => line.ContainsAll(geometricPoints)) as DefinableByPoints,

                        // Looking for the circle that contains our points
                        // It might happen there is no such circle, if the points are collinear
                        RandomPointOnCircleFromPoints => geometricPoints[0].Circles.FirstOrDefault(circle => circle.ContainsAll(geometricPoints)),

                        // Impossible, we're down to these 2 cases
                        _ => throw new TheoremProverException("Impossible.")
                    })
                   // Now we take every possible point on this line / circle 
                   ?.Points.Select(point => point.ConfigurationObject)
                   // Distinct from the passed ones
                   .Where(point => !mappedObject.PassedArguments.FlattenedList.Contains(point))
                   // Create the final mapping
                   .Select(point => new MappingData(data, (constructedObject, point),
                        // with the right theorem used as a fact
                        new Theorem(type, inputPoints.Concat(point).ToArray())))
                    // If there is no valid object, then there is no mapping
                    ?? Enumerable.Empty<MappingData>();

                // If we have a random point on construction
                case RandomPointOnConstruction randomPointConstruction:

                    // We need to create the object which we're looking for a random point on
                    var innerObject = new ConstructedConfigurationObject(randomPointConstruction.Construction, newArguments);

                    // Now we prepare the constructor function of it for the contextual picture
                    IReadOnlyDictionary<Picture, IAnalyticObject> ConstructorFunction() =>
                        // That calls the constructor 
                        _constructor.Construct(input.ExaminedConfigurationPicture.Pictures, innerObject);

                    // We pass this function to the contextual picture to get the resulting object
                    // TODO: Handle possible failure
                    var foundObject = input.ExaminedConfigurationPicture.GetGeometricObject(ConstructorFunction);

                    // If the found object is null, i.e. the construction is not possible, 
                    // then there is no mapping
                    if (foundObject == null)
                        return Enumerable.Empty<MappingData>();

                    // Otherwise we have something that has points. We take every possible point 
                    return ((DefinableByPoints)foundObject).Points
                        // and create a mapping for each
                        .Select(point => new MappingData(data,
                            // Marking the found mapping
                            (constructedObject, point.ConfigurationObject),
                            // And the found incidence
                            (point.ConfigurationObject, innerObject)));
            }

            #endregion

            #region Handling mapping if there is at most one option

            // First take the configuration
            var equalObject = (ConfigurationObject)configuration
                // And look for an equivalent constructed object
                .ConstructedObjects.FirstOrDefault(constructedObject => constructedObject.Equals(mappedObject));

            // If there is no formally equal object...
            if (equalObject == null)
            {
                // We need to examine this object with respect to the examined configuration
                // TODO: Figure out how we want to handle a possible exception
                var newObjectData = _constructor.ExamineObject(input.ExaminedConfigurationPicture.Pictures, mappedObject, addToPictures: false);

                // If the object is not constructible, then the mapping is incorrect
                if (newObjectData.InconstructibleObject != default)
                    return Enumerable.Empty<MappingData>();

                // If there is NO duplicate, then the mapping is not correct as well
                if (newObjectData.Duplicates == default)
                    return Enumerable.Empty<MappingData>();

                // Otherwise we know some real objects corresponds to the templated one
                equalObject = newObjectData.Duplicates.olderObject;

                // Mark this equality
                data.EqualObjects.Add((equalObject, mappedObject));
            }

            // Make sure the object is added to the mapping
            data.Mapping.Add(constructedObject, equalObject);

            // Return the current data
            return data.ToEnumerable();

            #endregion
        }

        #endregion
    }
}
