using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;

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
            /// <summary>
            /// The mapping of objects of the template theorem's configuration and objects of the examined theorem's configuration.
            /// </summary>
            public Dictionary<ConfigurationObject, ConfigurationObject> Mapping { get; set; }

            /// <summary>
            /// The list of used object equalities found while obtaining this mapping.
            /// </summary>
            public List<(ConfigurationObject newerObject, ConfigurationObject olderObject)> EqualObjects { get; set; }
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
                .SelectMany(data =>
                {
                    // Take the template theorems
                    return input.TemplateTheorems.AllObjects
                        // Try to remap each, persevering the info about the template one
                        .Select(theorem => (templateTheorem: theorem, remppedTheorem: theorem.Remap(data.Mapping)))
                        // Take those where it could be done
                        .Where(pair => pair.remppedTheorem != null)
                        // Each represents a correct output
                        .Select(pair => new SubtheoremsDeriverOutput
                        {
                            TemplateTheorem = pair.templateTheorem,
                            DerivedTheorem = pair.remppedTheorem,
                            UsedEqualities = data.EqualObjects
                        });
                });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappings(SubtheoremsDeriverInput input)
        {
            // Return the answer based on the layout of the template theorem
            return input.TemplateConfiguration.LooseObjectsHolder.Layout switch
            {
                // Circle and its tangent line
                CircleAndItsTangentLineFromPoints => GenerateInitialMappingsForCircleAndItsTangentLineLayout(input),

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

                // Default case
                _ => throw new GeoGenException($"Unhandled case")
            };
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration consists of points only.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsWithPoints(SubtheoremsDeriverInput input)
        {
            // Get the template loose objects for a shorter access
            var templateLooseObjects = input.TemplateConfiguration.LooseObjects;

            // Take the points 
            return input.ContextualPicture.AllPoints
                // And their n-tuples, where n is the inferred number of points
                .Variations(templateLooseObjects.Count)
                // Unwrap configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject))
                // Each represents a mapping
                .Select(points => new MappingData
                {
                    // That we can get by zipping the loose template objects and these points
                    Mapping = templateLooseObjects.Cast<ConfigurationObject>().ZipToDictionary(points.Cast<ConfigurationObject>()),

                    // No equal objects
                    EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects the template theorem in case where the layout of the template
        /// configuration is <see cref="FourConcyclicPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForFourConcyclicPoints(SubtheoremsDeriverInput input)
        {
            // Get the template loose objects for a shorter access
            var templateLooseObjects = input.TemplateConfiguration.LooseObjects;

            // Take all circles
            return input.ContextualPicture.AllCircles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // Take all 4-elements variations for each of them
                .SelectMany(circle => circle.Points.Variations(4))
                // Get plain configuration objects from these quadruples
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each represents a possible mapping
                .Select(points => new MappingData
                {
                    // That we can get by zipping the loose template objects and these points
                    Mapping = templateLooseObjects.Cast<ConfigurationObject>().ZipToDictionary(points.Cast<ConfigurationObject>()),

                    // No equal objects
                    EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="CircleAndItsTangentLineFromPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForCircleAndItsTangentLineLayout(SubtheoremsDeriverInput input)
        {
            // Get the template loose objects for a shorter access
            var templateLooseObjects = input.TemplateConfiguration.LooseObjects;

            // We need to make sure the result are the same across the pictures.
            // We're going to use aggregation. Each picture gets a list of (circle,line)
            // pairs that were determined tangent to each other in the previous pictures, 
            // or all these pairs at the beginning. For every such a list each picture
            // takes only those pairs that are tangent with respect to it. This way we are sure that 
            // at the end we have only those that are tangent to each other in every picture
            return input.ContextualPicture.Pictures.Aggregate(

               // Initially we take all circles and lines
               new IEnumerable<DefinableByPoints>[]
               {
                    // Get the circles from the contextual picture
                    input.ContextualPicture.AllCircles
                    // That have their configuration object set 
                    // (since it's one of the loose objects)
                    .Where(circle => circle.ConfigurationObject != null),

                    // Get the lines from the contextual picture
                    input.ContextualPicture.AllLines
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
                       return ((Circle)input.ContextualPicture.GetAnalyticObject(pair[0], picture))
                            // Is tangent to the line
                            .IsTangentTo((Line)input.ContextualPicture.GetAnalyticObject(pair[1], picture));
                   });
               })
               // Now we have correct pairs that are tangent to each other in every picture
               // For each such a pair we take every possible pair of points on the line...
               .SelectMany(circleLine => circleLine[1].Points.Subsets(2).Select(points =>
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

                       // No equal objects have been used
                       EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                   };
               }));
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="ThreeCyclicQuadrilatersOnSixPoints"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForThreeCyclicQuadrilatersOnSixPointsLayout(SubtheoremsDeriverInput input)
        {
            // Get the template loose objects for a shorter access
            var templateLooseObjects = input.TemplateConfiguration.LooseObjects;

            // We start with getting all circles
            return input.ContextualPicture.AllCircles
                // That contain at least three points
                .Where(circle => circle.Points.Count >= 4)
                // We take their unordered triples
                .Subsets(3)
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

                    // No equal objects have been used
                    EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                });
        }

        /// <summary>
        /// Generates the initial <see cref="MappingData"/> of the objects of the examined theorem
        /// and the loose objects of the template theorem in case where the layout of the template
        /// configuration is <see cref="Trapezoid"/>.
        /// </summary>
        /// <param name="input">The algorithm input.</param>
        /// <returns>The mappings.</returns>
        private IEnumerable<MappingData> GenerateInitialMappingsForTrapezoidLayout(SubtheoremsDeriverInput input)
        {
            // Get the template loose objects for a shorter access
            var templateLooseObjects = input.TemplateConfiguration.LooseObjects;

            // We need to make sure the result are the same across the pictures.
            // We're going to use aggregation. Each picture gets a list of lists of lines
            // that were determined parallel in the previous pictures, or one list containing
            // all lines at the beginning. For every such a list it groups its elements into 
            // sublists such that lines in each sublist are parallel. This way we are sure that 
            // at the end all our lists contains lines that are parallel to each other in every picture
            return input.ContextualPicture.Pictures.Aggregate(

                // Initially we work with one group 
                (IEnumerable<IEnumerable<LineObject>>)new[]
                {
                    // This group contains all lines
                    input.ContextualPicture.AllLines
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
                        return currentGroup.GroupBy(line => ((Line)input.ContextualPicture.GetAnalyticObject(line, picture)).Angle.Rounded())
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
                .Select(pairOfParellelLines => new[] { pairOfParellelLines.Item1.Points.Variations(2), pairOfParellelLines.Item2.Points.Variations(2) }.Combine())
                // This way we get enumerable of pairs of pairs of points, so we flatten it
                .Flatten()
                // In each we might even exchange points from the line, like (A,B,C,D) --> (C,D,A,B)
                .SelectMany(points => new[]
                {
                    new[] { points[0][0], points[0][1], points[1][0], points[1][1] },
                    new[] { points[1][0], points[1][1], points[0][0], points[0][1] }
                })
                // And now we work with pairs of pairs of points that can be directly flattened to an array
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // These points finally represent one of our sought trapezoids
                .Select(points => new MappingData
                {
                    // Set the mapping of the template points to the found points
                    Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                    {
                        { templateLooseObjects[0], points[0] },
                        { templateLooseObjects[1], points[1] },
                        { templateLooseObjects[2], points[2] },
                        { templateLooseObjects[3], points[3] }
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
                        EqualObjects = data.EqualObjects.ToList()
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
                    case RandomPoint:

                        // Then we do so by pulling the pictures
                        return input.ContextualPicture.Pictures.Configuration
                            // Looking into its objects map
                            .ObjectsMap.GetOrDefault(ConfigurationObjectType.Point)?
                            // And using our helper function
                            .Select(Map)
                            // In case there in no point (which I doubt every will), return nothing
                            ?? Enumerable.Empty<MappingData>();

                    // If our points lines on a line / circle...
                    case RandomPointOnCircle:
                    case RandomPointOnLine:
                    case RandomPointOnLineFromPoints:

                        // Prepare a line/circle where the point lies
                        var lineOrCircle = default(DefinableByPoints);

                        // We need to case-work further
                        switch (predefinedConstruction.Type)
                        {
                            // If we have a random point on a physical line/circle...
                            case RandomPointOnCircle:
                            case RandomPointOnLine:

                                // Then we have passed line or circle
                                var configurationLineOfCircle = mappedObject.PassedArguments.FlattenedList.First();

                                // We get the geometric object
                                lineOrCircle = (DefinableByPoints)input.ContextualPicture.GetGeometricObject(configurationLineOfCircle);

                                break;

                            // If we have a random point on a line defined by points...
                            case RandomPointOnLineFromPoints:

                                // In order to find the result take all lines
                                lineOrCircle = input.ContextualPicture.AllLines
                                    // That contain the two points that should make the arguments of the line
                                    .Where(line => line.Points.Select(p => p.ConfigurationObject).ToSet().IsSupersetOf(mappedObject.PassedArguments.FlattenedList))
                                    // There should be exactly one line like this
                                    .Single();

                                break;
                        }

                        // In order to get every possible mapping we take all the points
                        // on our line / circle. Each of them represents a potential mapping...
                        return lineOrCircle.Points.Select(p => p.ConfigurationObject).Select(Map);
                }
            }

            #endregion

            // At this point we are sure we have at most one option to map the object
            // First we check if it's not 
            var equalObject = (ConfigurationObject)input.ContextualPicture.Pictures.Configuration.ConstructedObjects
                // directly equivalent to some object
                .FirstOrDefault(o => o.IsEquivalentTo(mappedObject));

            // If there is no equal real object...
            if (equalObject == null)
            {
                // We need to examine this object with respect to the examined configuration
                // TODO: Handle exception
                var newObjectData = _constructor.ExamineObject(input.ContextualPicture.Pictures, mappedObject);

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
    }
}
