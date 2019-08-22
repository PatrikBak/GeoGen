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

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// A default implementation of <see cref="ISubtheoremsDeriver"/>.
    /// </summary>
    public class SubtheoremsDeriver : ISubtheoremsDeriver
    {
        #region MappingData class

        /// <summary>
        /// A helper class holding data about some mapping between objects of the template configuration
        /// and the examined configuration.
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
            public List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)> EqualObjects { get; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a instance of the <see cref="MappingData"/> class
            /// by cloning a given mapping and adding a new mapped pair to it.
            /// </summary>
            /// <param name="mapping">The mapping to be cloned.</param>
            /// <param name="newMappedPair">The new mapped pair to be included in the mapping.</param>
            public MappingData(MappingData mapping, (ConfigurationObject object1, ConfigurationObject object2) newMappedPair)
            {
                Mapping = mapping.Mapping.ToDictionary(pair => pair.Key, pair => pair.Value);
                Mapping.Add(newMappedPair.object1, newMappedPair.object2);
                EqualObjects = mapping.EqualObjects.ToList();
            }

            /// <summary>
            /// Initializes a instance of the <see cref="MappingData"/> class
            /// with no equal objects.
            /// </summary>
            /// <param name="templateObjects">The loose objects that are mapped.</param>
            /// <param name="mappedObjects">The objects to which the loose objects are mapped.</param>
            public MappingData(IReadOnlyList<LooseConfigurationObject> templateObjects, IEnumerable<ConfigurationObject> mappedObjects)
            {
                Mapping = templateObjects.Cast<ConfigurationObject>().ZipToDictionary(mappedObjects);
                EqualObjects = new List<(ConfigurationObject originalObject, ConstructedConfigurationObject equalObject)>();
            }

            #endregion
        }

        #endregion

        #region Dependencies

        /// <summary>
        /// The constructor of geometric objects that allows us to find geometrically equal objects.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremsDeriver"/> class.
        /// </summary>
        /// <param name="constructor">The constructor of geometric objects that allows us to find geometrically equal objects.</param>
        public SubtheoremsDeriver(IGeometryConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region ISubtheoremsDeriver implementation

        /// <summary>
        /// Performs the sub-theorem derivation on a given input.
        /// </summary>
        /// <param name="input">The input for the deriver.</param>
        /// <returns>The derived theorems wrapped in output objects.</returns>
        public IEnumerable<SubtheoremsDeriverOutput> DeriveTheorems(SubtheoremsDeriverInput input)
        {
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
                .Select(pair => new SubtheoremsDeriverOutput
                {
                    // To get theorems take all template ones
                    DerivedTheorems = pair.derivedTheorems,

                    // Set the used equalities
                    UsedEqualities = pair.data.EqualObjects
                });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappings(SubtheoremsDeriverInput input)
        {
            // Return the answer based on the layout of the template theorem
            return input.TemplateConfiguration.LooseObjectsHolder.Layout switch
            {
                // Three cyclic quadruples on six points
                ThreeCyclicQuadrilatersOnSixPoints => GenerateInitialMappingsForThreeCyclicQuadrilatersOnSixPointsLayout(input),

                // Trapezoid
                Trapezoid => GenerateInitialMappingsForTrapezoidLayout(input),

                // Triangle
                ThreePoints => GenerateInitialMappingsWithPoints(input),

                // Quadrilateral
                FourPoints => GenerateInitialMappingsWithPoints(input),

                // Concyclic points
                FourConcyclicPoints => GenerateInitialMappingsForFourConcyclicPoints(input),

                // Line segment
                TwoPoints => GenerateInitialMappingsWithPoints(input),

                // Right triangle
                RightTriangle => GenerateInitialMappingsForRightTriangleLayout(input),

                // Tangent line 
                CircleFromPointAndItsTangentLineAtOnePoint => GenerateInitialMappingsForCircleFromPointAndItsTangentLineAtOnePointLayout(input),

                // Isosceles triangle
                IsoscelesTriangle => GenerateInitialMappingsForIsoscelesTriangleLayout(input),

                // Default case
                _ => throw new TheoremsAnalyzerException($"Unhandled type of loose objects layout: {input.TemplateConfiguration.LooseObjectsHolder.Layout}")
            };
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration consists of points only.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsWithPoints(SubtheoremsDeriverInput input)
        {
            // Take the points 
            return input.ExaminedConfigurationPicture.AllPoints
                // And their n-tuples, where n is the inferred number of points
                .Variations(input.TemplateConfiguration.LooseObjects.Count)
                // Unwrap configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject))
                // Each represents a mapping
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects the template theorem in case where the layout of the template
        /// configuration is <see cref="FourConcyclicPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForFourConcyclicPoints(SubtheoremsDeriverInput input)
        {
            // Take all circles
            return input.ExaminedConfigurationPicture.AllCircles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // Take all 4-elements variations for each of them
                .SelectMany(circle => circle.Points.Variations(4))
                // Unwrap configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject))
                // Each represents a possible mapping
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="ThreeCyclicQuadrilatersOnSixPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForThreeCyclicQuadrilatersOnSixPointsLayout(SubtheoremsDeriverInput input)
        {
            // We start with getting all circles
            return input.ExaminedConfigurationPicture.AllCircles
                // That contain at least three points
                .Where(circle => circle.Points.Count >= 4)
                // We take their unordered triples
                .Subsets(3)
                // For each triple of circles create three possible pairs of them
                .Select(triple => new[] { (triple[0], triple[1]), (triple[1], triple[2]), (triple[2], triple[0]) })
                // For each triple of these pairs find common points
                .Select(tripleOfPairs => tripleOfPairs.Select(pair => pair.Item1.CommonPointsWith(pair.Item2).ToArray()).ToArray())
                // Take only those triples of common points where each of them has exactly 2 points
                .Where(commonPointsInPairs => commonPointsInPairs.All(points => points.Length == 2))
                // Flatten them
                .Select(commonPointsInParks => new[]
                {
                    commonPointsInParks[0][0],
                    commonPointsInParks[0][1],
                    commonPointsInParks[1][0],
                    commonPointsInParks[1][1],
                    commonPointsInParks[2][0],
                    commonPointsInParks[2][1]
                })
                // Unwrap configuration objects
                .Select(points => points.Select(p => p.ConfigurationObject))
                // Now we finally have our 6 points that compose a correct layout
                .Select(commonPoints => new MappingData(input.TemplateConfiguration.LooseObjects, commonPoints));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="Trapezoid"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForTrapezoidLayout(SubtheoremsDeriverInput input)
        {
            // Take the parallel lines theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(ParallelLines)
                // Unwrap and order theorem objects
                ?.Select(theorem => theorem.InvolvedObjects.ToArray())
                // From each theorem object unwrap points
                .Select(objects => new[]
                {
                    ((LineTheoremObject)objects[0]).Points,
                    ((LineTheoremObject)objects[1]).Points
                })
                // For each such a pair of lines we combine pair of points from one line
                // with pairs of points from the other one
                .Select(pairOfPointArrays => new[] { pairOfPointArrays[0].Variations(2), pairOfPointArrays[1].Variations(2) }.Combine())
                // This way we get enumerable of pairs of pairs of points, so we flatten it
                .Flatten()
                // In each we might even exchange points from the line, like (A,B,C,D) --> (C,D,A,B)
                .SelectMany(points => new[]
                {
                    new[] { points[0][0], points[0][1], points[1][0], points[1][1] },
                    new[] { points[1][0], points[1][1], points[0][0], points[0][1] }
                })
                // These points finally represent one of our sought trapezoids
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points))
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
        private static IEnumerable<MappingData> GenerateInitialMappingsForRightTriangleLayout(SubtheoremsDeriverInput input)
        {
            // Take the perpendicular lines theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(PerpendicularLines)
                // Unwrap and order theorem objects
                ?.Select(theorem => theorem.InvolvedObjects.ToArray())
                // From each theorem object unwrap points
                .Select(objects => new[]
                {
                    ((LineTheoremObject)objects[0]).Points,
                    ((LineTheoremObject)objects[1]).Points
                })
                // Find their common points (should be at most one)
                .Select(pairOfPointArrays => (pointArrays: pairOfPointArrays, commonPoint: pairOfPointArrays[0].Intersect(pairOfPointArrays[1]).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(tuple => tuple.commonPoint != null)
                // Get those points on particular lines that are distinct from the common one
                .Select(tuple => (tuple.commonPoint, otherPoints: new[]
                {
                    tuple.pointArrays[0].Where(point => point != tuple.commonPoint),
                    tuple.pointArrays[1].Where(point => point != tuple.commonPoint)
                }))
                // Combine the points from the lines
                .SelectMany(tuple => tuple.otherPoints.Combine().Select(twoPoints => (tuple.commonPoint, twoPoints)))
                // Flatten the tuple to a single array + exchange the points
                .SelectMany(tuple => new[]
                {
                    new[] { tuple.commonPoint, tuple.twoPoints[0], tuple.twoPoints[1] },
                    new[] { tuple.commonPoint, tuple.twoPoints[1], tuple.twoPoints[0] }
                })
                // These points finally represent one of our sought right triangle
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points))
                // If there are no perpendicular line theorems, we have no mapping
                ?? Enumerable.Empty<MappingData>();
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="CircleFromPointAndItsTangentLineAtOnePoint"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private static IEnumerable<MappingData> GenerateInitialMappingsForCircleFromPointAndItsTangentLineAtOnePointLayout(SubtheoremsDeriverInput input)
        {
            // Take the line tangent to circle theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(LineTangentToCircle)
                // Unwrap and order theorem objects
                ?.Select(theorem => theorem.InvolvedObjects)
                // From each theorem object unwrap points
                .Select(objects => new[]
                {
                    objects.OfType<LineTheoremObject>().First().Points,
                    objects.OfType<CircleTheoremObject>().First().Points
                })
                // Find their common points (should be at most one)
                .Select(pairOfPointArrays => (pointArrays: pairOfPointArrays, commonPoint: pairOfPointArrays[0].Intersect(pairOfPointArrays[1]).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(tuple => tuple.commonPoint != null)
                // We need to combine points from the line with ordered pairs of points from the circle
                .Select(tuple => (tuple.commonPoint, otherPoints: new[]
                {
                    tuple.pointArrays[1].Where(point => point != tuple.commonPoint).Variations(2),
                    tuple.pointArrays[0].Where(point => point != tuple.commonPoint).Select(point => point.ToEnumerable())
                }))
                // With this array we can use the Combine method to do the combination
                .SelectMany(tuple => tuple.otherPoints.Combine().Select(twoArrays => (tuple.commonPoint, twoArrays)))
                // Finally each tuple represents points that we need to flatten
                .Select(tuple => tuple.commonPoint.ToEnumerable().Concat(tuple.twoArrays.Flatten()))
                // These points finally represent one of our sought situation
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points))
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
        private static IEnumerable<MappingData> GenerateInitialMappingsForIsoscelesTriangleLayout(SubtheoremsDeriverInput input)
        {
            // Take the equal line segments theorems from the configuration
            return input.ExaminedConfigurationTheorems.GetOrDefault(EqualLineSegments)
                // Unwrap and order theorem objects
                ?.Select(theorem => theorem.InvolvedObjects.ToArray())
                // From each theorem object unwrap line segments
                .Select(objects => new[]
                {
                    (LineSegmentTheoremObject)objects[0],
                    (LineSegmentTheoremObject)objects[1],
                })
                // And configuration objects from them
                .Select(lineSegments => new[]
                {
                    new[]
                    {
                        ((PointTheoremObject)lineSegments[0].Object1).ConfigurationObject,
                        ((PointTheoremObject)lineSegments[0].Object2).ConfigurationObject
                    },
                    new[]
                    {
                        ((PointTheoremObject)lineSegments[1].Object1).ConfigurationObject,
                        ((PointTheoremObject)lineSegments[1].Object2).ConfigurationObject
                    }
                })
                // Find their common points (should be at most one)
                .Select(pairOfPointArrays => (pointArrays: pairOfPointArrays, commonPoint: pairOfPointArrays[0].Intersect(pairOfPointArrays[1]).FirstOrDefault()))
                // Take only those where there is a common point
                .Where(tuple => tuple.commonPoint != null)
                // Find the other point for 
                .Select(tuple => (tuple.commonPoint, otherPoints: new[]
                {
                    tuple.pointArrays[0].Where(point => point != tuple.commonPoint).First(),
                    tuple.pointArrays[1].Where(point => point != tuple.commonPoint).First()
                }))
                // Make a single array with possibly exchanged points
                .SelectMany(tuple => new[]
                {
                    new[] {tuple.commonPoint, tuple.otherPoints[0], tuple.otherPoints[1]},
                    new[] {tuple.commonPoint, tuple.otherPoints[1], tuple.otherPoints[0]}
                })
                // These points finally represent one of our sought situation
                .Select(points => new MappingData(input.TemplateConfiguration.LooseObjects, points))
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
        private IEnumerable<MappingData> GenerateMappingsIncludingObject(MappingData data, ConstructedConfigurationObject constructedObject, SubtheoremsDeriverInput input)
        {
            #region Creating the mapped object

            // First we need to remap the arguments of the constructed objects
            // with respect to the provided mapping
            var mappedObjects = constructedObject.PassedArguments.FlattenedList.Select(o => data.Mapping[o]).ToArray();

            // Make sure there are no two equal objects. If yes, the mapping is incorrect
            if (mappedObjects.Distinct().Count() != mappedObjects.Length)
                return Enumerable.Empty<MappingData>();

            // Create new arguments for the remapped object.
            var newArguments = constructedObject.Construction.Signature.Match(mappedObjects);

            // Finally create the object
            var mappedObject = new ConstructedConfigurationObject(constructedObject.Construction, newArguments);

            #endregion

            #region Handling mapping if there are more options for mapping this object

            // Then we need to have a 'Random' construction:
            switch (mappedObject.Construction)
            {
                // If we can choose our point completely at random...
                case PredefinedConstruction p when p.Type == RandomPoint:

                    // Then we do so by pulling the pictures
                    return input.ExaminedConfigurationPicture.Pictures.Configuration
                        // Looking into its objects map
                        .ObjectsMap.GetOrDefault(ConfigurationObjectType.Point)?
                        // And using our helper function
                        .Select(point => new MappingData(data, (constructedObject, point)))
                        // In case there in no point (which I doubt every will), return nothing
                        ?? Enumerable.Empty<MappingData>();

                // If the construction is predefined...
                case PredefinedConstruction predefinedConstruction when
                    // And it's a random point on line
                    predefinedConstruction.Type == RandomPointOnLineFromPoints
                    // Or a circle
                    || predefinedConstruction.Type == RandomPointOnCircleFromPoints:

                    // Get the geometric objects corresponding to the input points
                    // Take the flattened arguments
                    var geometricPoints = mappedObject.PassedArguments.FlattenedList
                        // For each find the geometric point
                        .Select(input.ExaminedConfigurationPicture.GetGeometricObject)
                        // Cast
                        .Cast<PointObject>()
                        // Enumerate
                        .ToArray();

                    // First find the geometric line/circle corresponding that pass through the found points
                    return (predefinedConstruction.Type switch
                    {
                        // Looking for the line that contains our points
                        RandomPointOnLineFromPoints => geometricPoints[0].Lines.First(line => line.ContainsAll(geometricPoints)) as DefinableByPoints,

                        // Looking for the circle that contains our points
                        RandomPointOnCircleFromPoints => geometricPoints[0].Circles.First(circle => circle.ContainsAll(geometricPoints)),

                        // Impossible, we're down to these 2 cases
                        _ => throw new TheoremsAnalyzerException("Impossible.")
                    })
                   // Now we take every possible point on this line / circle as an option
                   .Points.Select(point => new MappingData(data, (constructedObject, point.ConfigurationObject)));

                // If we have a random point on construction
                case RandomPointOnConstruction randomPointConstruction:

                    // We need to create the object which we're looking for a random point on
                    var innerObject = new ConstructedConfigurationObject(randomPointConstruction.Construction, newArguments);

                    // Now we prepare the constructor function of it for the contextual picture
                    IReadOnlyDictionary<Picture, IAnalyticObject> ConstructorFunction() =>
                        // That calls the constructor 
                        _constructor.Construct(input.ExaminedConfigurationPicture.Pictures, innerObject);

                    // We pass this function to the contextual picture to get the resulting object
                    var foundObject = input.ExaminedConfigurationPicture.GetGeometricObject(ConstructorFunction);

                    // If the found object is null, i.e. the construction 
                    // is not possible, then there is no mapping
                    if (foundObject == null)
                        return Enumerable.Empty<MappingData>();

                    // Otherwise we have something that has points. We take every possible point 
                    return ((DefinableByPoints)foundObject).Points
                        // and create a mapping for each
                        .Select(point => new MappingData(data, (constructedObject, point.ConfigurationObject)));
            }

            #endregion

            #region Handling mapping if there is at most one option

            // First dig down to the configuration
            var equalObject = (ConfigurationObject)input.ExaminedConfigurationPicture.Pictures.Configuration
                // And look for an equivalent constructed object
                .ConstructedObjects.FirstOrDefault(constructedObject => constructedObject.Equals(mappedObject));

            // If there is no equal object...
            if (equalObject == null)
            {
                // We need to examine this object with respect to the examined configuration
                // TODO: Figure out how we want to handle a possible exception
                var newObjectData = _constructor.ExamineObject(input.ExaminedConfigurationPicture.Pictures, mappedObject);

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
