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
        #region Dependencies

        /// <summary>
        /// The constructor of geometric objects that allows us to find geometrically equal objects.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        /// <summary>
        /// The factory for creating objects containers that allows us to find syntactically equal objects.
        /// </summary>
        private readonly IConfigurationObjectsContainerFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremAnalyzer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor of geometric objects.</param>
        public SubtheoremAnalyzer(IGeometryConstructor constructor, IConfigurationObjectsContainerFactory factory)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
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

            #region Create objects container from the original configuration

            // Create an empty container
            var container = _factory.CreateContainer();

            // Add all the objects
            potentialConsequence.Configuration.ObjectsMap.AllObjects.ForEach(container.Add);

            #endregion

            // Use the helper method to generate possible mappings between the objects of the
            // tested configuration and the loose objects of the original theorem
            return GenerateMappings(originalTheorem.Configuration.LooseObjectsHolder, potentialConsequence.Configuration.ObjectsMap)
                // Try all of them
                .Select(mapping =>
                {
                    // Prepare the list of all non-trivial equalities
                    var equalities = new List<(ConfigurationObject originalObject, ConfigurationObject equalObject)>();

                    // With mapping of the template loose objects we need to construct the constructed objects as well
                    foreach (var constructedObject in originalTheorem.Configuration.ConstructedObjects)
                    {
                        // First we need to remap its passed objects according to our mapping
                        var newPassedObjects = new ConfigurationObjectsMap(constructedObject.PassedArguments.FlattenedList.Select(o => mapping[o]));

                        // Create the new arguments for them
                        var newArguments = constructedObject.Construction.Signature.Match(newPassedObjects);

                        // Finally create its remapped version
                        var newObject = new ConstructedConfigurationObject(constructedObject.Construction, newArguments);

                        // First we check if it's not directly equal to some object
                        container.TryAdd(newObject, out var equalRealObject);

                        // If there is no equal real object...
                        if (equalRealObject == null)
                        {
                            // W need to examine this object with respect to the tested configuration
                            var newObjectData = _constructor.Examine(newObject, testedConfigurationData.Manager);

                            // Make sure the examination went fine. If not, we can't do much
                            if (!newObjectData.SuccessfullyExamined)
                                return (null, null);

                            // If the object is not constructible, then the mapping is incorrect
                            if (newObjectData.InconstructibleObject != null)
                                return (null, null);

                            // If there is NO duplicate, then the mapping is not correct as well
                            if (newObjectData.Duplicates == (null, null))
                                return (null, null);

                            // Otherwise we know some real objects corresponds to the templated one
                            equalRealObject = newObjectData.Duplicates.olderObject;

                            // We can mark this non-trivial equality
                            equalities.Add(newObjectData.Duplicates);
                        }

                        // We can update the mapping so it can be used to construct other objects
                        mapping.Add(constructedObject, equalRealObject);
                    }

                    // If we got here, the mapping is successful. 
                    return (mapping, equalities);
                })
                // Skip unsuccessful ones
                .Where(mapping => mapping != (null, null))
                // Create the data for successful ones
                .Select(pair =>
                {
                    // Deconstruct
                    var (mapping, equalities) = pair;

                    #region Create remapped theorem

                    // Helper function to remapped a TheoremObject
                    TheoremObject Remap(TheoremObject theoremObject) => new TheoremObject(theoremObject.Signature, 
                        // Cast each internal object to the object corresponding in the mapping
                        theoremObject.InternalObjects.Select(internalobject => mapping[internalobject]).ToArray());

                    // Reconstruct the original theorem with respect to the mapping
                    var remappedTheorem = new Theorem(potentialConsequence.Configuration, originalTheorem.Type, originalTheorem.InvolvedObjects.Select(Remap).ToArray());

                    #endregion

                    // Skip not equal theorems
                    if (!AreTheoremsEqual(remappedTheorem, potentialConsequence))
                        return null;

                    // Otherwise we can create a correct result
                    return new SubtheoremData
                    {
                        // Set success
                        SuccessfullyAnalyzed = true,

                        // Set sub-theorem
                        IsSubtheorem = true,

                        // Create the list of objects on which the theorem was applied
                        MappedObjects = originalTheorem.Configuration.LooseObjectsHolder.LooseObjects.Select(o => mapping[o]).ToList(),

                        // Set found equalities
                        UsedEqualities = equalities
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

        private bool AreTheoremsEqual(Theorem theorem1, Theorem theorem2)
        {
            // Check the types first
            if (theorem1.Type != theorem2.Type)
                return false;

            // If they don't have the same number of involved objects, then they are not equal
            if (theorem1.InvolvedObjects.Count != theorem2.InvolvedObjects.Count)
                return false;

            // Helper comparer to determine equality of theorem objects
            // They must have equal signatures and equal sets of internal objects
            var comparer = new SimpleEqualityComparer<TheoremObject>((o1, o2) => o1.Signature == o2.Signature && o1.InternalObjects.ToSet().SetEquals(o2.InternalObjects), t => 0);
            
            // We need to take care particular types since they might have specific signatures
            switch (theorem1.Type)
            {
                // The cases where the objects can be in any order
                case TheoremType.CollinearPoints:
                case TheoremType.ConcurrentObjects:
                case TheoremType.ConcyclicPoints:
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.TangentCircles:

                    // Check if the sets of their internal objects are equal
                    return theorem1.InvolvedObjects.ToSet(comparer).SetEquals(theorem2.InvolvedObjects);

                // The case where there is one line and one circle (in this order)
                case TheoremType.LineTangentToCircle:

                    // Check the corresponding objects
                    return theorem1.InvolvedObjects.SequenceEqual(theorem2.InvolvedObjects, comparer);

                // The cases where we basically have the signature {{x, x}, {x, x}} 
                // (i.e. objects in pair can be in any order, and the pairs itself as well)
                case TheoremType.EqualAngles:
                case TheoremType.EqualLineSegments:

                    // Get the pairs for each object
                    var obj1Set1 = new HashSet<TheoremObject>(comparer) { theorem1.InvolvedObjects[0], theorem1.InvolvedObjects[1] };
                    var obj1Set2 = new HashSet<TheoremObject>(comparer) { theorem1.InvolvedObjects[2], theorem1.InvolvedObjects[3] };
                    var obj2Set1 = new HashSet<TheoremObject>(comparer) { theorem2.InvolvedObjects[0], theorem2.InvolvedObjects[1] };
                    var obj2Set2 = new HashSet<TheoremObject>(comparer) { theorem2.InvolvedObjects[2], theorem2.InvolvedObjects[3] };

                    // Write this boring condition that should do the job
                    return obj1Set1.SetEquals(obj2Set1) && obj1Set2.SetEquals(obj2Set2) ||
                           obj1Set1.SetEquals(obj2Set2) && obj1Set2.SetEquals(obj2Set1);

                // Default case
                default:
                    throw new GeoGenException("Unhandled type");
            }
        }

        private IEnumerable<Dictionary<ConfigurationObject, ConfigurationObject>> GenerateMappings(LooseObjectsHolder templateObjects, ConfigurationObjectsMap realObjects)
        {
            // Handle the case where there is no layout in the template, i.e. objects can be mapped without rules
            if (templateObjects.Layout == LooseObjectsLayout.None)
            {
                return templateObjects.ObjectsMap.Select(keyValue => realObjects[keyValue.Key]
                                                                .Variations(keyValue.Value.Count)
                                                                .Select(variation => variation.Zip(keyValue.Value, (real, original) => (real, original))))
                                          .Combine()
                                          .Select(mapping => mapping.Flatten().ToDictionary(pair => pair.original, pair => pair.real));

            }

            // TODO: Handle other layouts
            throw new NotImplementedException();

            // TODO: variation.Zip(keyValue.Value, (real, original) => (real, original)) can be extracted to an extension method
        }

        #endregion

    }
}