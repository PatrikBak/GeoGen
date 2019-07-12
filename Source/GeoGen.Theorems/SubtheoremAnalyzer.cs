using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Theorems
{
    /// <summary>
    /// The default implementation of <see cref="ISubtheoremAnalyzer"/>.
    /// </summary>
    public class SubtheoremAnalyzer : ISubtheoremAnalyzer
    {
        #region MappingData class

        /// <summary>
        /// A helper class holding data about some mapping between the template and the 
        /// examined theorem.
        /// </summary>
        private class MappingData
        {
            /// <summary>
            /// The mapping of loose objects of the template theorem's configuration
            /// and objects of the examined theorem's configuration.
            /// </summary>
            public Dictionary<ConfigurationObject, ConfigurationObject> Mapping { get; set; }

            /// <summary>
            /// The list of used object equalities found while obtaining this mapping.
            /// </summary>
            public List<(ConfigurationObject newerObject, ConfigurationObject olderObject)> EqualObjects { get; set; }

            /// <summary>
            /// The list of used facts found while obtaining this mapping.
            /// </summary>
            public List<Theorem> UsedFacts { get; set; }
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
        /// Initializes a new instance of the <see cref="SubtheoremAnalyzer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor of geometric objects that allows us to find geometrically equal objects.</param>
        public SubtheoremAnalyzer(IGeometryConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region ISubtheoremAnalyzer implementation

        /// <summary>
        /// Performs the sub-theorem analysis on a given input.
        /// </summary>
        /// <returns>The result of the sub-theorem analysis.</returns>
        public SubtheoremAnalyzerOutput Analyze(SubtheoremAnalyzerInput input)
        {
            // Make sure the theorems have the same type
            if (input.TemplateTheorem.Type != input.ExaminedTheorem.Type)
                throw new GeoGenException("The algorithm cannot be used for theorems of different types.");

            // Use the helper method to generate possible mappings between the loose objects of the
            // template configurations and objects of the examined configuration
            return GenerateInitialMappings(input)
                // Each of these mappings can yield more mappings when other objects are considered
                .SelectMany(mapping =>
                {
                    // Take the constructed objects of the examined configuration
                    return input.TemplateTheorem.Configuration.ConstructedObjects
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
                // Now we have only correct mappings. We try to match the theorems
                .Select(data =>
                {
                    #region Create remapped theorem

                    // Helper function to remap a TheoremObject
                    TheoremObject Remap(TheoremObject theoremObject)
                    {
                        // If we have a point, then we just remap the internal object
                        if (theoremObject.Type == ConfigurationObjectType.Point)
                            return new TheoremPointObject(data.Mapping[theoremObject.ConfigurationObject]);

                        // Otherwise we have a theorem object with points
                        var objectWithPoints = (TheoremObjectWithPoints) theoremObject;

                        // We need to remap them
                        var points = objectWithPoints.Points.Select(p => data.Mapping[p]).ToSet();

                        // If there is no internal object and not enough points, the mapping couldn't be done
                        if (theoremObject.ConfigurationObject == null && points.Count < objectWithPoints.NumberOfNeededPoints)
                            return null;

                        // We need to remap the internal object as well, if it's present
                        var internalObject = theoremObject.ConfigurationObject == null ? null : data.Mapping[theoremObject.ConfigurationObject];

                        // And we're done
                        return new TheoremObjectWithPoints(theoremObject.Type, internalObject, points);
                    }

                    // Remap the involved objects
                    var remappedInvolvedObjects = input.TemplateTheorem.InvolvedObjects.Select(Remap).ToArray();

                    // If there is an object that could not be mapped, for example
                    // because of the fact that two distinct points making a line are
                    // mapped to the same object, then we won't have a correct theorem
                    if (remappedInvolvedObjects.Any(o => o == null))
                        return null;

                    // Reconstruct the original theorem with respect to the mapping
                    var remappedTheorem = new Theorem(input.ExaminedTheorem.Configuration, input.TemplateTheorem.Type, remappedInvolvedObjects);

                    #endregion

                    // Find out if the theorems express the same fact (that is assumed to be true)
                    return Theorem.AreTheoremsEquivalent(remappedTheorem, input.ExaminedTheorem)
                            // If yes, return a correct result
                            ? new SubtheoremAnalyzerOutput
                            {
                                // Set that we've proved the sub-theorem
                                IsSubtheorem = true,

                                // Set found equalities
                                UsedEqualities = data.EqualObjects,

                                // Set found used facts
                                UsedFacts = data.UsedFacts
                            }
                            // If not, return null
                            : null;
                })
                // Take the first correct output
                .FirstOrDefault(data => data != null)
                // If there is none, return failure
                ?? new SubtheoremAnalyzerOutput
                {
                    IsSubtheorem = false
                };
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappings(SubtheoremAnalyzerInput input)
        {
            // Return the answer based on the layout of the template theorem
            switch (input.TemplateTheorem.Configuration.LooseObjectsHolder.Layout)
            {
                case LooseObjectsLayout.None:
                    return GenerateInitialMappingsForNoneLayout(input);

                case LooseObjectsLayout.CircleAndItsTangentLine:
                    return GenerateInitialMappingsForCircleAndItsTangentLineLayout(input);

                case LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints:
                    return GenerateInitialMappingsForThreeCyclicQuadrilatersOnSixPointsLayout(input);

                case LooseObjectsLayout.Trapezoid:
                    return GenerateInitialMappingsForTrapezoidLayout(input);

                default:
                    throw new GeoGenException("Unhandled layout");
            }
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="LooseObjectsLayout.None"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForNoneLayout(SubtheoremAnalyzerInput input)
        {
            // Get the template configuration's objects map for shorter access
            var templateLooseObjectsMap = input.TemplateTheorem.Configuration.LooseObjectsHolder.ObjectsMap;

            // Get the examined configuration's objects map for shorter access
            var examinedObjectsMap = input.ExaminedTheorem.Configuration.ObjectsMap;

            // Make sure all the needed object types are there
            if (!templateLooseObjectsMap.Keys.All(examinedObjectsMap.ContainsKey))
                return Enumerable.Empty<MappingData>();

            // We take every pair of type / objects
            return templateLooseObjectsMap.Select(typeObjectsPair =>
            {
                // For every pair we look at the examined configuration's objects
                return examinedObjectsMap[typeObjectsPair.Key]
                            // Take the needed number of objects of this type
                            .Variations(typeObjectsPair.Value.Count)
                            // Create pairs using one-to-one mapping (zipping)
                            .Select(variation => variation.Zip(typeObjectsPair.Value, (examinedObject, templateObject) => (examinedObject, templateObject)));
            })
            // These mapping of objects of particular types are then combined
            .Combine()
            // And each such a combination represents a correct result
            .Select(mapping => new MappingData
            {
                // Set the final mapping by flattening the combined mappings for particular types
                Mapping = mapping.Flatten().ToDictionary(pair => pair.templateObject, pair => pair.examinedObject),

                // No equal objects have been used
                EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>(),

                // No facts have been used
                UsedFacts = new List<Theorem>()
            });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="LooseObjectsLayout.CircleAndItsTangentLine"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForCircleAndItsTangentLineLayout(SubtheoremAnalyzerInput input)
        {
            // Get the template loose objects for shorter access
            var templateLooseObjects = input.TemplateTheorem.Configuration.LooseObjectsHolder.LooseObjects;

            // We need to make sure the result are the same across the pictures.
            // We're going to use aggregation. Each picture gets a list of (circle,line)
            // pairs that were determined tangent to each other in the previous pictures, 
            // or all these pairs at the beginning. For every such a list each picture
            // takes only those pairs that are tangent with respect to it. This way we are sure that 
            // at the end we have only those that are tangent to each other in every picture
            return input.ExaminedConfigurationManager.Aggregate(

               // Initially we take all circles and lines
               new IEnumerable<DefinableByPoints>[]
               {
                    // Get the circles from the contextual picture
                    input.ExaminedConfigurationContexualPicture.GetGeometricObjects<CircleObject>(new ContextualPictureQuery
                    {
                        IncludeCirces = true,
                        Type = ContextualPictureQuery.ObjectsType.All
                    })
                    // That have their configuration object set 
                    // (since it's one of the loose objects)
                    .Where(circle => circle.ConfigurationObject != null),

                    // Get the lines from the contextual picture
                    input.ExaminedConfigurationContexualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
                    {
                        IncludeLines = true,
                        Type = ContextualPictureQuery.ObjectsType.All
                    })
                    // That contain at least two points
                    .Where(line => line.Points.Count >= 2)
               }
               // Combine them into pair in every possible way
               .Combine(),

               // Each pictures gets an enumeration of circle/point pairs...
               (currentPairs, picture) =>
               {
                   // And take those pairs...
                   return currentPairs.Where(pair =>
                   {
                       // Where the circle...
                       return input.ExaminedConfigurationContexualPicture.GetAnalyticObject<Circle>(pair[0], picture)
                            // Is tangent to the line
                            .IsTangentTo(input.ExaminedConfigurationContexualPicture.GetAnalyticObject<Line>(pair[1], picture));
                   });
               })
               // Now we have correct pairs that are tangent to each other in every picture
               // For each such a pair we take every possible pair of points on the line...
               .SelectMany(circleLine => circleLine[1].Points.Subsets(2).Select(points => points.ToArray()).Select(points =>
               {
                   // For a given pair of points we finally have out mapping to return
                   return new MappingData
                   {
                       // Set the found circle and the points
                       Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                       {
                            { templateLooseObjects[0], circleLine[0].ConfigurationObject },
                            { templateLooseObjects[1], points[0].ConfigurationObject },
                            { templateLooseObjects[2], points[1].ConfigurationObject }
                       },

                       // Set that we used the fact that the line from our points is tangent to our circle
                       UsedFacts = new List<Theorem>
                       {
                            new Theorem(input.ExaminedTheorem.Configuration, TheoremType.LineTangentToCircle, new[]
                            {
                                new TheoremObjectWithPoints(ConfigurationObjectType.Line, points[0].ConfigurationObject, points[1].ConfigurationObject),
                                new TheoremObjectWithPoints(ConfigurationObjectType.Circle, circleLine[0].ConfigurationObject)
                            })
                       },

                       // No equal objects have been used
                       EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                   };
               }));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForThreeCyclicQuadrilatersOnSixPointsLayout(SubtheoremAnalyzerInput input)
        {
            // Get the template loose objects for shorter access
            var templateLooseObjects = input.TemplateTheorem.Configuration.LooseObjectsHolder.LooseObjects;

            // We start with getting all circles
            return input.ExaminedConfigurationContexualPicture.GetGeometricObjects<CircleObject>(new ContextualPictureQuery
            {
                IncludeCirces = true,
                Type = ContextualPictureQuery.ObjectsType.All
            })
            // That contain at least three points
            .Where(circle => circle.Points.Count >= 4)
            // We take their unordered triples
            .Subsets(3)
            // Cast each to an array so we can work with it better
            .Select(subset => subset.ToArray())
            // For each triple of circles create three possible pairs of them
            .Select(triple => new[] { (triple[0], triple[1]), (triple[1], triple[2]), (triple[2], triple[0]) })
            // For each triple of these pairs find common points
            .Select(tripleOfPairs => tripleOfPairs.Select(pair => pair.Item1.CommonPointsWith(pair.Item2).Select(p => p.ConfigurationObject).ToArray()).ToArray())
            // Take only those triples of common points where every one of them has exactly 2 points
            .Where(commonPointsInPairs => commonPointsInPairs.All(points => points.Length == 2))
            // Now we finally have our 6 points that compose a correct layout
            .Select(commonPoints => new MappingData
            {
                // Set the mapping of the template points to the found points
                Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                {
                     { templateLooseObjects[0], commonPoints[0][0] },
                     { templateLooseObjects[1], commonPoints[0][1] },
                     { templateLooseObjects[2], commonPoints[1][0] },
                     { templateLooseObjects[3], commonPoints[1][1] },
                     { templateLooseObjects[4], commonPoints[2][0] },
                     { templateLooseObjects[5], commonPoints[2][1] }
                },

                // Set that we used the fact that three quadruples of points are concyclic
                UsedFacts = new List<Theorem>
                {
                     new Theorem(input.ExaminedTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                     {
                         new TheoremPointObject(commonPoints[0][0]),
                         new TheoremPointObject(commonPoints[0][1]),
                         new TheoremPointObject(commonPoints[1][0]),
                         new TheoremPointObject(commonPoints[1][1])
                     }),
                     new Theorem(input.ExaminedTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                     {
                         new TheoremPointObject(commonPoints[0][0]),
                         new TheoremPointObject(commonPoints[0][1]),
                         new TheoremPointObject(commonPoints[2][0]),
                         new TheoremPointObject(commonPoints[2][1])
                     }),
                     new Theorem(input.ExaminedTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                     {
                         new TheoremPointObject(commonPoints[1][0]),
                         new TheoremPointObject(commonPoints[1][1]),
                         new TheoremPointObject(commonPoints[2][0]),
                         new TheoremPointObject(commonPoints[2][1])
                     })
                },

                // No equal objects have been used
                EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
            });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="LooseObjectsLayout.Trapezoid"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForTrapezoidLayout(SubtheoremAnalyzerInput input)
        {
            // Get the template loose objects for shorter access
            var templateLooseObjects = input.TemplateTheorem.Configuration.LooseObjectsHolder.LooseObjects;

            // We need to make sure the result are the same across the pictures.
            // We're going to use aggregation. Each picture gets a list of lists of lines
            // that were determined parallel in the previous pictures, or one list containing
            // all lines at the beginning. For every such a list it groups its elements into 
            // sublists such that lines in each sublist are parallel. This way we are sure that 
            // at the end all our lists contains lines that are parallel to each other in every picture
            return input.ExaminedConfigurationManager.Aggregate(

                // Initially we work with one group 
                (IEnumerable<IEnumerable<LineObject>>) new[]
                {
                    // This group contains all lines
                    input.ExaminedConfigurationContexualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
                    {
                        IncludeLines = true,
                        Type = ContextualPictureQuery.ObjectsType.All
                    })
                    // That have at least two points
                    .Where(line => line.Points.Count >= 2)
                },

                // Each picture gets a list of current groups of lines...
                (currentGroups, picture) =>
                {
                    // Every of these groups is then projected into one or more groups...
                    return currentGroups.SelectMany(currentGroup =>
                    {
                        // Within a single group we first categorize these lines be their angle
                        // with respect to the current picture (two lines are parallel if and
                        // only if their angles are the same). 
                        return currentGroup.GroupBy(line => input.ExaminedConfigurationContexualPicture.GetAnalyticObject<Line>(line, picture).Angle)
                                      // We clearly only those groups where there are at least two lines
                                      // (that are parallel to each other)
                                      .Where(innerGroup => innerGroup.Count() >= 2);
                    });
                })
                // Each found groups contains only lines parallel to each other in every picture
                // For each we consider every pair of the lines from the group
                .SelectMany(parallelLines => parallelLines.ToArray().UnorderedPairs())
                // For each such a pair of lines we combine pair of points from one line
                // with pairs of points from the other one
                .Select(pairOfParellelLines => new[] { pairOfParellelLines.Item1.Points.Subsets(2), pairOfParellelLines.Item2.Points.Subsets(2) }.Combine())
                // This way we get pairs of pairs of points, i.e. four points, which we flatten 
                // into a single array
                .Select(pairOfPairOfPoints => pairOfPairOfPoints.Flatten().Flatten().Select(point => point.ConfigurationObject).ToArray())
                // These points finally represent one of our sought trapezoids
                .Select(trapezoidPoints => new MappingData
                {
                    // Set the mapping of the template points to the found points
                    Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                    {
                        { templateLooseObjects[0], trapezoidPoints[0] },
                        { templateLooseObjects[1], trapezoidPoints[1] },
                        { templateLooseObjects[2], trapezoidPoints[2] },
                        { templateLooseObjects[3], trapezoidPoints[3] }
                    },

                    // Set that we used the fact that these four points make parallel lines
                    UsedFacts = new List<Theorem>
                    {
                         new Theorem(input.ExaminedTheorem.Configuration, TheoremType.ParallelLines, new[]
                         {
                             new TheoremObjectWithPoints(ConfigurationObjectType.Line, trapezoidPoints[0], trapezoidPoints[1]),
                             new TheoremObjectWithPoints(ConfigurationObjectType.Line, trapezoidPoints[2], trapezoidPoints[3])
                         })
                    },

                    // No equal objects have been used
                    EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                });
        }

        /// <summary>
        /// Extends a given mapping by including a given constructed objects.
        /// </summary>
        /// <param name="data">The current data where we want to add the <paramref name="constructedObject"/>.</param>
        /// <param name="constructedObject">The object that should be currently mapped.</param>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateMappingsIncludingObject(MappingData data, ConstructedConfigurationObject constructedObject, SubtheoremAnalyzerInput input)
        {
            #region Creating the mapped object

            // First we need to remap the arguments of the constructed objects
            // with respect to the provided mapping
            var mappedObjects = constructedObject.PassedArguments.FlattenedList.Select(o => data.Mapping[o]).ToArray();

            // Make sure there are no two equal objects. If yes, the mapping is incorrect
            if (mappedObjects.Distinct().Count() != mappedObjects.Length)
                return Enumerable.Empty<MappingData>();

            // Create new arguments for the remapped object.
            var newArguments = constructedObject.Construction.Signature.Match(new ConfigurationObjectsMap(mappedObjects));

            // Finally create it
            var mappedObject = new ConstructedConfigurationObject(constructedObject.Construction, newArguments);

            #endregion

            #region Handling mapping if there are more options for mapping this object

            // We need to have a predefined construction
            if (mappedObject.Construction is PredefinedConstruction predefinedConstruction)
            {
                // Helper function that creates a mapping including a given point
                MappingData Map(ConfigurationObject point)
                {
                    // Copy the current mapping
                    var currentMappingCopy = new MappingData
                    {
                        Mapping = data.Mapping.ToDictionary(pair => pair.Key, pair => pair.Value),
                        EqualObjects = data.EqualObjects.ToList(),
                        UsedFacts = data.UsedFacts.ToList()
                    };

                    // Add the new point mapping to it
                    currentMappingCopy.Mapping.Add(constructedObject, point);

                    // Return it
                    return currentMappingCopy;
                }

                // Then we need to have a 'Random' construction:
                switch (predefinedConstruction.Type)
                {
                    // If we can choose our point completely at random...
                    case PredefinedConstructionType.RandomPoint:

                        // Then we do so and create mapping using our helper function
                        return input.ExaminedTheorem.Configuration.ObjectsMap[ConfigurationObjectType.Point].Select(Map);

                    // If our points lines on a line / circle...
                    case PredefinedConstructionType.RandomPointOnCircle:
                    case PredefinedConstructionType.RandomPointOnLine:
                    case PredefinedConstructionType.RandomPointOnLineFromPoints:

                        // Prepare a line/circle where the point lies
                        var lineOrCircle = default(DefinableByPoints);

                        // We need to case-work further
                        switch (predefinedConstruction.Type)
                        {
                            // If we have a random point on a physical line/circle...
                            case PredefinedConstructionType.RandomPointOnCircle:
                            case PredefinedConstructionType.RandomPointOnLine:

                                // Then we get the geometric object corresponding to it 
                                lineOrCircle = input.ExaminedConfigurationContexualPicture.GetGeometricObject<DefinableByPoints>(mappedObject.PassedArguments.FlattenedList.Single());

                                break;

                            // If we have a random point on a line defined by points...
                            case PredefinedConstructionType.RandomPointOnLineFromPoints:

                                // Then the result is the single line containing the examined points
                                lineOrCircle = input.ExaminedConfigurationContexualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
                                {
                                    IncludeLines = true,
                                    Type = ContextualPictureQuery.ObjectsType.All,
                                    ContainingPoints = mappedObject.PassedArguments.FlattenedList
                                }).Single();

                                break;
                        }

                        // In order to get every possible mapping we take all the points
                        // on our line / circle. Each of them represents a potential mapping...
                        return lineOrCircle.Points.Select(p => p.ConfigurationObject).Select(Map);
                }
            }

            #endregion

            // At this point we are sure we have at most one option to map the object
            // First we check if it's not directly equal to some object
            var equalObject = input.ExaminedConfigurationObjectsContainer.FindEqualItem(mappedObject);

            // If there is no equal real object...
            if (equalObject == null)
            {
                // We need to examine this object with respect to the examined configuration
                var newObjectData = _constructor.Examine(input.ExaminedTheorem.Configuration, input.ExaminedConfigurationManager, mappedObject);

                // Make sure the examination went fine. If not, we can't do much
                if (!newObjectData.SuccessfullyExamined)
                    return Enumerable.Empty<MappingData>();

                // If the object is not constructible, then the mapping is incorrect
                if (newObjectData.InconstructibleObject != default)
                    return Enumerable.Empty<MappingData>();

                // If there is NO duplicate, then the mapping is not correct as well
                if (newObjectData.Duplicates == default)
                    return Enumerable.Empty<MappingData>();

                // Otherwise we know some real objects corresponds to the templated one
                equalObject = newObjectData.Duplicates.olderObject;

                // Mark this equality
                data.EqualObjects.Add(newObjectData.Duplicates);
            }

            // Make sure the object is added to the mapping
            data.Mapping.Add(constructedObject, equalObject);

            // Return the current data
            return data.ToEnumerable();
        }

        #endregion
    }
}