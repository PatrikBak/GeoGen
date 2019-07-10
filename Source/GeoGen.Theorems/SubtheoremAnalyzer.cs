using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Theorems
{
    public class SubtheoremAnalyzer : ISubtheoremAnalyzer
    {
        #region MappingData class

        private class MappingData
        {
            public Dictionary<ConfigurationObject, ConfigurationObject> Mapping { get; set; }

            public List<(ConfigurationObject newerObject, ConfigurationObject olderObject)> EqualObjects { get; set; }

            public List<Theorem> UsedFacts { get; set; }
        }

        #endregion

        #region Dependencies

        /// <summary>
        /// The constructor of geometric objects that allows us to find geometrically equal objects.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The factory for creating objects containers that allows us to find syntactically equal objects.
        /// </summary>
        private readonly IConfigurationObjectsContainerFactory _objectContainersFactory;
        private readonly IContextualContainerFactory _contextualContainerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremAnalyzer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor of geometric objects.</param>
        /// <param name="objectContainersFactory"></param>
        /// <param name="contextualContainerFactory"></param>
        public SubtheoremAnalyzer(IGeometryConstructor constructor, IConfigurationObjectsContainerFactory objectContainersFactory, IContextualContainerFactory contextualContainerFactory)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _objectContainersFactory = objectContainersFactory ?? throw new ArgumentNullException(nameof(objectContainersFactory));
            _contextualContainerFactory = contextualContainerFactory ?? throw new ArgumentNullException(nameof(contextualContainerFactory));
        }

        #endregion

        #region ISubtheoremAnalyzer implementation

        /// <summary>
        /// Finds out of a given original theorem is a consequence of another given theorem.
        /// </summary>
        /// <param name="originalTheorem">The original theorem that serves like a template.</param>
        /// <param name="potentialConsequence">The theorem that might be a consequence of the original theorem.</param>
        /// <returns>The result of the sub-theorem analysis.</returns>
        public SubtheoremData Analyze(Theorem originalTheorem, Theorem potentialConsequence)
        {
            // Make sure the theorems have the same type
            if (originalTheorem.Type != potentialConsequence.Type)
            {
                return new SubtheoremData
                {
                    SuccessfullyAnalyzed = true,
                    IsSubtheorem = false
                };
            }

            // We need to have the tested configuration drawn
            var testedConfigurationData = _constructor.Construct(potentialConsequence.Configuration);

            // Make sure it's been drawn correctly
            if (!testedConfigurationData.SuccessfullyExamined)
            {
                return new SubtheoremData
                {
                    SuccessfullyAnalyzed = false
                };
            }

            // Make sure the configuration is correct itself
            if (testedConfigurationData.InconstructibleObject != null || testedConfigurationData.Duplicates != (null, null))
                // TODO: Create a specific exception for this project
                throw new GeoGenException();

            #region Create containers from the original configuration

            // Create an empty container
            var objectsContainer = _objectContainersFactory.CreateContainer();

            // Add all the objects
            potentialConsequence.Configuration.ObjectsMap.AllObjects.ForEach(objectsContainer.Add);

            // Create a lazy initializer of the contextual container
            var contextualContainerInitializer = new Lazy<IContextualContainer>(() => _contextualContainerFactory.Create(potentialConsequence.Configuration, testedConfigurationData.Manager));

            // Local function to access the contextual container
            IContextualContainer ContextualContainer() => contextualContainerInitializer.Value;

            #endregion

            IEnumerable<MappingData> GenerateMappingsIncludingObject(MappingData data, ConstructedConfigurationObject constructedObject)
            {
                // First we need to remap its passed objects according to our mapping
                var mappedObjects = constructedObject.PassedArguments.FlattenedList.Select(o => data.Mapping[o]).ToArray();

                // Make sure there are no two equal objects
                // If yes, the mapping is incorrect
                if (mappedObjects.Distinct().Count() != mappedObjects.Length)
                    return Enumerable.Empty<MappingData>();

                // Create the new object 
                var newPassedObjects = new ConfigurationObjectsMap(mappedObjects);

                // Create the new arguments for them
                var newArguments = constructedObject.Construction.Signature.Match(newPassedObjects);

                // Finally create its remapped version
                var newObject = new ConstructedConfigurationObject(constructedObject.Construction, newArguments);

                #region Handling mapping if the object is a random point

                // Prepare a line/circle where the point lies
                var lineOrCircle = default(DefinableByPoints);

                // There are only predefined random constructions...
                if (newObject.Construction is PredefinedConstruction predefinedConstruction)
                {
                    if (predefinedConstruction.Type == PredefinedConstructionType.RandomPoint)
                    {
                        return potentialConsequence.Configuration.ObjectsMap[ConfigurationObjectType.Point]
                            .Select(point =>
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
                            });
                    }

                    // Consider all the cases
                    switch (predefinedConstruction.Type)
                    {
                        // Point on an explicit circle/line
                        case PredefinedConstructionType.RandomPointOnCircle:
                        case PredefinedConstructionType.RandomPointOnLine:

                            // Get the single line / circle corresponding to the one in the argument
                            lineOrCircle = ContextualContainer().GetGeometricalObject<DefinableByPoints>(newObject.PassedArguments.FlattenedList.Single());

                            break;

                        // Point on an implicit line                             
                        case PredefinedConstructionType.RandomPointOnLineFromPoints:

                            // Get the single line containing passed points
                            lineOrCircle = ContextualContainer().GetGeometricalObjects<LineObject>(new ContextualContainerQuery
                            {
                                IncludeLines = true,
                                Type = ContextualContainerQuery.ObjectsType.All,
                                ContainingPoints = newObject.PassedArguments.FlattenedList
                            }).Single();

                            break;
                    }
                }

                // If we've found a line or a circle...
                if (lineOrCircle != null)
                {
                    // Then we need to generate all possible options of mapping
                    // Take all the points that lie on our line/circle...
                    return lineOrCircle.Points.Select(point =>
                    {
                        // Copy the current mapping
                        var currentMappingCopy = new MappingData
                        {
                            Mapping = data.Mapping.ToDictionary(pair => pair.Key, pair => pair.Value),
                            EqualObjects = data.EqualObjects.ToList(),
                            UsedFacts = data.UsedFacts.ToList()
                        };

                        // Add the new point mapping to it
                        currentMappingCopy.Mapping.Add(constructedObject, point.ConfigurationObject);

                        // Return it
                        return currentMappingCopy;
                    });
                }

                #endregion

                // First we check if it's not directly equal to some object
                var equalObject = objectsContainer.FindEqualItem(newObject);

                // If there is no equal real object...
                if (equalObject == null)
                {
                    // We need to examine this object with respect to the tested configuration
                    var newObjectData = _constructor.Examine(originalTheorem.Configuration, testedConfigurationData.Manager, newObject);

                    // Make sure the examination went fine. If not, we can't do much
                    if (!newObjectData.SuccessfullyExamined)
                        return Enumerable.Empty<MappingData>();

                    // If the object is not constructible, then the mapping is incorrect
                    if (newObjectData.InconstructibleObject != null)
                        return Enumerable.Empty<MappingData>();

                    // If there is NO duplicate, then the mapping is not correct as well
                    if (newObjectData.Duplicates == (null, null))
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

            IEnumerable<MappingData> GenerateInitialMappings()
            {
                var templateObjects = originalTheorem.Configuration.LooseObjectsHolder;
                var realObjects = potentialConsequence.Configuration.ObjectsMap;

                // Handle the case where there is no layout in the template, i.e. objects can be mapped without rules
                if (templateObjects.Layout == LooseObjectsLayout.None)
                {
                    // Make sure all the types are there
                    if (!templateObjects.ObjectsMap.Keys.All(realObjects.ContainsKey))
                        return Enumerable.Empty<MappingData>();


                    return templateObjects.ObjectsMap.Select(keyValue => realObjects[keyValue.Key]
                                                                    ?.Variations(keyValue.Value.Count)
                                                                    .Select(variation => variation.Zip(keyValue.Value, (real, original) => (real, original))))
                                              .Combine()
                                              .Select(mapping => mapping.Flatten().ToDictionary(pair => pair.original, pair => pair.real))
                                              .Select(mapping => new MappingData
                                              {
                                                  Mapping = mapping,
                                                  EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>(),
                                                  UsedFacts = new List<Theorem>()
                                              });
                }

                if (templateObjects.Layout == LooseObjectsLayout.CircleAndItsTangentLine)
                {
                    return testedConfigurationData.Manager.Aggregate(

                            // Seed
                            new IEnumerable<DefinableByPoints>[]
                            {
                                // Circles
                                ContextualContainer().GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
                                {
                                    IncludeCirces = true,
                                    Type = ContextualContainerQuery.ObjectsType.All
                                })
                                .Where(circle => circle.ConfigurationObject != null),

                                // Lines
                                ContextualContainer().GetGeometricalObjects<LineObject>(new ContextualContainerQuery
                                {
                                    IncludeLines = true,
                                    Type = ContextualContainerQuery.ObjectsType.All
                                })
                                .Where(line => line.Points.Count >= 2)
                            }
                            .Combine(),

                            // Current groups
                            (currentPairs, manager) =>
                            {
                                return currentPairs.Where(pair => ContextualContainer()
                                    .GetAnalyticObject<Circle>(pair[0], manager)
                                    .IsTangentTo(ContextualContainer().GetAnalyticObject<Line>(pair[1], manager)));
                            })
                            .SelectMany(pair => pair[1].Points.Subsets(2).Select(p => p.ToArray()).Select(pairOfPoints =>
                            {
                                return new MappingData
                                {
                                    Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                                    {
                                        { templateObjects.LooseObjects[0], pair[0].ConfigurationObject },
                                        { templateObjects.LooseObjects[1], pairOfPoints[0].ConfigurationObject },
                                        { templateObjects.LooseObjects[2], pairOfPoints[1].ConfigurationObject }
                                    },
                                    UsedFacts = new List<Theorem>
                                    {
                                        new Theorem(potentialConsequence.Configuration, TheoremType.LineTangentToCircle, new[]
                                        {
                                            new TheoremObjectWithPoints(ConfigurationObjectType.Line, pairOfPoints[0].ConfigurationObject, pairOfPoints[1].ConfigurationObject),
                                            new TheoremObjectWithPoints(ConfigurationObjectType.Circle, pair[0].ConfigurationObject)
                                        })
                                    },
                                    EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                                };
                            }));
                }

                if (templateObjects.Layout == LooseObjectsLayout.ThreeCyclicQuadrilatersOnSixPoints)
                {
                    return ContextualContainer().GetGeometricalObjects<CircleObject>(new ContextualContainerQuery
                    {
                        IncludeCirces = true,
                        Type = ContextualContainerQuery.ObjectsType.All
                    })
                    .Where(circle => circle.Points.Count >= 4)
                    .ToArray()
                    .UnorderedTriples()
                    .Select(triple => new[] { (triple.Item1, triple.Item2), (triple.Item2, triple.Item3), (triple.Item3, triple.Item1) }
                                        .Select(pair => pair.Item1.CommonPointsWith(pair.Item2).ToArray()).ToArray())
                    .Where(commonPoints => commonPoints.All(points => points.Length == 2))
                    .Select(commonPoints => new MappingData
                    {
                        Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                        {
                            { templateObjects.LooseObjects[0], commonPoints[0][0].ConfigurationObject },
                            { templateObjects.LooseObjects[1], commonPoints[0][1].ConfigurationObject },
                            { templateObjects.LooseObjects[2], commonPoints[1][0].ConfigurationObject },
                            { templateObjects.LooseObjects[3], commonPoints[1][1].ConfigurationObject },
                            { templateObjects.LooseObjects[4], commonPoints[2][0].ConfigurationObject },
                            { templateObjects.LooseObjects[5], commonPoints[2][1].ConfigurationObject }
                        },
                        UsedFacts = new List<Theorem>
                        {
                            new Theorem(originalTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                            {
                                new TheoremPointObject(commonPoints[0][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[0][1].ConfigurationObject),
                                new TheoremPointObject(commonPoints[1][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[1][1].ConfigurationObject)
                            }),
                            new Theorem(originalTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                            {
                                new TheoremPointObject(commonPoints[0][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[0][1].ConfigurationObject),
                                new TheoremPointObject(commonPoints[2][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[2][1].ConfigurationObject)
                            }),
                            new Theorem(originalTheorem.Configuration, TheoremType.ConcyclicPoints, new[]
                            {
                                new TheoremPointObject(commonPoints[1][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[1][1].ConfigurationObject),
                                new TheoremPointObject(commonPoints[2][0].ConfigurationObject),
                                new TheoremPointObject(commonPoints[2][1].ConfigurationObject)
                            })
                        },
                        EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                    });
                }

                if (templateObjects.Layout == LooseObjectsLayout.Trapezoid)
                {

                    return testedConfigurationData.Manager.Aggregate(

                        // Seed
                        ContextualContainer().GetGeometricalObjects<LineObject>(new ContextualContainerQuery
                        {
                            IncludeLines = true,
                            Type = ContextualContainerQuery.ObjectsType.All
                        })
                        .Where(line => line.Points.Count >= 2).ToEnumerable(),

                        // Aggregation function
                        (currentGroups, container) =>
                        {
                            return currentGroups.SelectMany(current =>
                            {
                                return current.GroupBy(line => ContextualContainer().GetAnalyticObject<Line>(line, container).Angle)
                                              .Select(innerGroup => innerGroup.ToArray())
                                              .Where(innerGroup => innerGroup.Length >= 2);
                            });
                        })
                        .SelectMany(group => group.ToArray().UnorderedPairs())
                        .Select(pairOfLines => new[] { pairOfLines.Item1.Points.Subsets(2), pairOfLines.Item2.Points.Subsets(2) }.Combine())
                        .Select(pairOfPairsOfPoints => pairOfPairsOfPoints.Flatten().Flatten().ToArray())
                        .Select(points => new MappingData
                        {
                            Mapping = new Dictionary<ConfigurationObject, ConfigurationObject>
                            {
                                { templateObjects.LooseObjects[0], points[0].ConfigurationObject },
                                { templateObjects.LooseObjects[1], points[1].ConfigurationObject },
                                { templateObjects.LooseObjects[2], points[2].ConfigurationObject },
                                { templateObjects.LooseObjects[3], points[3].ConfigurationObject }
                            },
                            UsedFacts = new List<Theorem>
                            {
                                new Theorem(originalTheorem.Configuration, TheoremType.ParallelLines, new[]
                                {
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, points[0].ConfigurationObject, points[1].ConfigurationObject),
                                    new TheoremObjectWithPoints(ConfigurationObjectType.Line, points[2].ConfigurationObject, points[3].ConfigurationObject)
                                })
                            },
                            EqualObjects = new List<(ConfigurationObject newerObject, ConfigurationObject olderObject)>()
                        });
                }

                // TODO: Handle other layouts
                throw new NotImplementedException();

                // TODO: variation.Zip(keyValue.Value, (real, original) => (real, original)) can be extracted to an extension method
            }

            // Use the helper method to generate possible mappings between the objects of the
            // tested configuration and the loose objects of the original theorem
            return GenerateInitialMappings()
                // We assign an index to the current mapping representing the 
                // current constructed object that hasn't been mapped
                //.Select(mapping => (mapping, objectIndexToConstruct: -1))
                // We try to construct one constructed object given by the index
                .SelectMany(data => originalTheorem.Configuration.ConstructedObjects
                    .Aggregate(data.ToEnumerable(), (current, constructedObject) => current.SelectMany(mapping => GenerateMappingsIncludingObject(mapping, constructedObject))))
                // Create the data for successful ones
                .Select(data =>
                {
                    #region Create remapped theorem

                    // Helper function to remapped a TheoremObject
                    TheoremObject Remap(TheoremObject theoremObject)
                    {
                        // If we have a point, then we just remap the internal object
                        if (theoremObject.Type == ConfigurationObjectType.Point)
                            return new TheoremPointObject(data.Mapping[theoremObject.ConfigurationObject]);

                        // Otherwise we have a theorem object with points
                        var objectWithPoints = (TheoremObjectWithPoints) theoremObject;

                        // We need to remap them
                        var points = objectWithPoints.Points.Select(p => data.Mapping[p]).ToSet();

                        // If there are is not physical object and not enough points, the mapping couldn't be done
                        if (theoremObject.ConfigurationObject == null && points.Count < objectWithPoints.NumberOfNeededPoints)
                            return null;

                        // We need to remap the internal object as well, if it's present...
                        var internalObject = theoremObject.ConfigurationObject == null ? null : data.Mapping[theoremObject.ConfigurationObject];

                        // And we're done
                        return new TheoremObjectWithPoints(theoremObject.Type, internalObject, points);
                    }

                    // Remap the involved objects
                    var remappedInvolvedObjects = originalTheorem.InvolvedObjects.Select(Remap).Distinct().ToArray();

                    // If there is an object that could be mapped, for example
                    // because of the fact that two distinct points making a line are
                    // mapped to the same object, then we won't have a correct theorem
                    if (remappedInvolvedObjects.Any(o => o == null))
                        return null;

                    // Reconstruct the original theorem with respect to the mapping
                    var remappedTheorem = new Theorem(potentialConsequence.Configuration, originalTheorem.Type, remappedInvolvedObjects);

                    #endregion

                    // Find out if the theorems express the same fact (that is assumed to be true)
                    if (!Theorem.AreTheoremsEquivalent(remappedTheorem, potentialConsequence))
                        return null;

                    // Otherwise we can create a correct result
                    return new SubtheoremData
                    {
                        // Set success
                        SuccessfullyAnalyzed = true,

                        // Set sub-theorem
                        IsSubtheorem = true,

                        // Set found equalities
                        UsedEqualities = data.EqualObjects,

                        // Set found used facts
                        UsedFacts = data.UsedFacts
                    };

                })
                // Take the first correct ones
                .FirstOrDefault(data => data != null)
                // If there is none, return failure
                ?? new SubtheoremData
                {
                    SuccessfullyAnalyzed = true,
                    IsSubtheorem = false
                };
        }

        #endregion

    }
}