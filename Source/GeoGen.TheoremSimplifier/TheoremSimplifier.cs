using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremSimplifier"/> that gets the needed <see cref="SimplificationRule"/>s
    /// in a <see cref="TheoremSimplifierData"/> object.
    /// </summary>
    public class TheoremSimplifier : ITheoremSimplifier
    {
        #region Private fields

        /// <summary>
        /// The data for the simplifier.
        /// </summary>
        private readonly TheoremSimplifierData _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifier"/> class.
        /// </summary>
        /// <param name="data">The data for the simplifier.</param>
        public TheoremSimplifier(TheoremSimplifierData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        #endregion

        #region ITheoremSimplifier implementation

        /// <inheritdoc/>
        public (Theorem newTheorem, Configuration newConfiguration)? Simplify(Theorem theorem, Configuration configuration)
        {
            // Prepare the dictionary of replaced theorem objects
            var replacedTheoremObjects = new Dictionary<TheoremObject, TheoremObject>();

            #region Mapping theorem objects

            // We will try to replace each theorem object
            foreach (var theoremObject in theorem.InvolvedObjects)
            {
                // We're going to try to use some simplification rule for it
                foreach (var simplifcationRule in _data.Rules)
                {
                    // If the template simplifiable object has a different type, then we can't do much
                    if (!theoremObject.GetType().Equals(simplifcationRule.SimplifableObject.GetType()))
                        continue;

                    // Otherwise switch based on object type
                    switch (theoremObject)
                    {
                        // Point simplification is not handled, let's move on
                        case PointTheoremObject _:
                            continue;

                        // With objects with points (lines / circles) we can try to map those points
                        case TheoremObjectWithPoints objectWithPoints:

                            // If this object doesn't have a point, we can't do much
                            if (!objectWithPoints.DefinedByPoints)
                                continue;

                            #region Mapping line / circle

                            // Get the original points
                            var originalPoints = objectWithPoints.PointsList;

                            // Get the template points
                            var templatePoints = (TheoremObjectWithPoints)simplifcationRule.SimplifableObject;

                            // Go through the permutations of template points, each representing 
                            // a possible mapping between them and original points
                            foreach (var orderedTemplatePoints in templatePoints.PointsList.Permutations())
                            {
                                #region Trying various mappings

                                // Enumerate them
                                foreach (var mapping in Map(orderedTemplatePoints, originalPoints))
                                {
                                    #region Mapping the simplified object

                                    // Otherwise we can use it to come up with the new theorem object
                                    // First we must include its objects in the mapping (since Remap method requires it)
                                    simplifcationRule.SimplifiedObject.GetInnerConfigurationObjects()
                                        // Only those that aren't there yet
                                        .Where(configurationObject => !mapping.ContainsKey(configurationObject))
                                        // They are definitely constructed (loose should have been mapped by now)
                                        .Cast<ConstructedConfigurationObject>()
                                        // Remap each
                                        .Select(constructedObject => (originalObject: constructedObject, mappedObject: new ConstructedConfigurationObject
                                        (
                                            // The construction stays the same
                                            construction: constructedObject.Construction,

                                            // Map its arguments one by one
                                            input: constructedObject.PassedArguments.FlattenedList.Select(innerObject => mapping[innerObject]).ToArray()
                                        )))
                                        // Add them to the mapping
                                        .ForEach(pair => mapping.Add(pair.originalObject, pair.mappedObject));

                                    // Now we can finally map the simplified object
                                    var newTheoremObject = simplifcationRule.SimplifiedObject.Remap(mapping);

                                    #endregion

                                    #region Checking if the mapping doesn't increase the number of configuration objects

                                    // We need to find how many configuration objects we would need if we did the replaced
                                    // In order to find out take the theorem objects
                                    var neededObjects = theorem.InvolvedObjects
                                        // Exclude the one to be replaced
                                        .Except(theoremObject.ToEnumerable())
                                        // Include the new one
                                        .Concat(newTheoremObject)
                                        // Find the inner configuration objects for each theorem object
                                        .SelectMany(theoremObject => theoremObject.GetInnerConfigurationObjects())
                                        // Use our helper method to find all the objects needed to define these
                                        .GetDefiningObjects()
                                        // And simply take their count
                                        .Count;

                                    // If this replacement would complicate the theorem, we need to keep looking...
                                    if (neededObjects > configuration.AllObjects.Count)
                                        continue;

                                    #endregion

                                    // At this point the mapping is okay and we can add the object
                                    replacedTheoremObjects.Add(theoremObject, newTheoremObject);

                                    // We don't have to look for another one
                                    break;
                                }

                                #endregion

                                // If the mapping has been successful, we don't have to look for another one
                                if (replacedTheoremObjects.ContainsKey(theoremObject))
                                    break;
                            }

                            #endregion

                            break;

                        // Unhandled cases
                        default:
                            throw new TheoremSimplifierException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {theoremObject.GetType()}");
                    }

                    // If the mapping has been successful, we don't have to look for another one
                    if (replacedTheoremObjects.ContainsKey(theoremObject))
                        break;
                }
            }

            #endregion

            #region Constructing the final result

            // If no object has been mapped, then we cannot simplify the theorem
            if (replacedTheoremObjects.IsEmpty())
                return null;

            // Otherwise we can recreate the theorem
            var newTheorem = new Theorem(theorem.Type, theorem.InvolvedObjects
                // Each object is either preserved or taken from the mapping
                .Select(theoremObject => replacedTheoremObjects.GetValueOrDefault(theoremObject) ?? theoremObject));

            // Now we need to find the new configuration. We do that by finding 
            // the objects that are now unnecessary with respect to the new theorem
            var unnecessaryObjects = newTheorem.GetUnnecessaryObjects(configuration);

            // Now we create the new configuration by copying its objects in their order
            var newConfiguration = new Configuration(configuration.LooseObjectsHolder, configuration.ConstructedObjects
                // With the exclusion of the objects that are now not necessary
                .Except(unnecessaryObjects)
                // And the inclusion the new objects that are taken from the inner objects of the new theorem
                .Concat(newTheorem.GetInnerConfigurationObjects()
                    // And are constructed
                    .OfType<ConstructedConfigurationObject>()
                    // With the exclusion of those that already are there
                    .Where(innerObject => !configuration.ConstructedObjectsSet.Contains(innerObject)))
                // They are constructed
                .Cast<ConstructedConfigurationObject>()
                // Now we can enumerate
                .ToArray());

            // Finally we can return the result
            return (newTheorem, newConfiguration);

            #endregion
        }

        /// <summary>
        /// Finds all mappings of template objects that respect the provided mapping of given template and original points.
        /// </summary>
        /// <param name="templatePoints">The template points to be mapped to the points of the original configuration.</param>
        /// <param name="originalPoints">The points of the original configuration to which the template points should be mapped.</param>
        /// <returns>The enumerable of correct mappings.</returns>
        private IEnumerable<Dictionary<ConfigurationObject, ConfigurationObject>> Map(IReadOnlyList<ConfigurationObject> templatePoints,
                                                                                      IReadOnlyList<ConfigurationObject> originalPoints)
            // First we zip the objects 
            => templatePoints.Zip(originalPoints)
                // Now we need to use aggregation. Each tuple generates multiple possible mappings
                .Aggregate
                (
                    // The initial seed is enumerable containing just the empty dictionary
                    new Dictionary<ConfigurationObject, ConfigurationObject>().ToEnumerable(),

                    // The mapping function takes currently available mappings and from each finds new ones
                    (currentMappings, mappedPair) => currentMappings.SelectMany(currentMapping =>
                    {
                        // Get the objects
                        var (template, original) = mappedPair;

                        // With this current mapping we can finally look at the objects
                        // Switch based on the template one
                        switch (template)
                        {
                            // If we have loose template...
                            case LooseConfigurationObject _:

                                // Then we need to check whether it hasn't already been mapped
                                // If yes it's been mapped
                                if (currentMapping.ContainsKey(template))
                                {
                                    // Then we need to find to what it's been mapped
                                    var mappedTemplate = currentMapping[template];

                                    // If this object equals to what this is supposed to be mapped
                                    return original.Equals(mappedTemplate) ?
                                        // Then this mapping doesn't offer anything new
                                        currentMapping.ToEnumerable() :
                                        // Otherwise the mapping cannot be done
                                        Enumerable.Empty<Dictionary<ConfigurationObject, ConfigurationObject>>();
                                }

                                // If the object hasn't been mapped yet, then we do the mapping
                                currentMapping.Add(template, original);

                                // Make sure there are no duplicates in the mapping
                                // If yes, then it's incorrect
                                if (currentMapping.Values.AnyDuplicates())
                                    return Enumerable.Empty<Dictionary<ConfigurationObject, ConfigurationObject>>();

                                // Otherwise it's fine
                                return currentMapping.ToEnumerable();

                            // If we have a construct template...
                            case ConstructedConfigurationObject constructedTemplate:

                                // Then the original object must also be and must have the same construction
                                if (!(original is ConstructedConfigurationObject constructedOriginal) || !constructedOriginal.Construction.Equals(constructedTemplate.Construction))
                                    return Enumerable.Empty<Dictionary<ConfigurationObject, ConfigurationObject>>();

                                // If it's the case, then we might have more possible mappings
                                // Every permutation of it's flattened objects
                                return constructedTemplate.PassedArguments.FlattenedList.Permutations()
                                    // That yields the same object after reconstruction
                                    .Where(permutation => constructedTemplate.Equals(new ConstructedConfigurationObject(constructedTemplate.Construction, permutation)))
                                    // Is attempted to be used
                                    .Select(permutation =>
                                    {
                                        // For a given permutation we want to either return null, or a new mapping
                                        // Prepare the dictionary with the new mapping by copying the current one
                                        var newMapping = new Dictionary<ConfigurationObject, ConfigurationObject>(currentMapping);

                                        // Now go through the zipped objects 
                                        foreach (var (innerTemplate, innerOriginal) in permutation.Zip(constructedOriginal.PassedArguments.FlattenedList))
                                        {
                                            // The inner objects of the template must be loose. 
                                            // Therefore they are mapped like loose ones
                                            // If the current one is mapped
                                            if (newMapping.ContainsKey(innerTemplate))
                                            {
                                                // Then we need to find to what it's been mapped
                                                var mappedTemplate = newMapping[innerTemplate];

                                                // If this object isn't equal to what this is supposed to be mapped,
                                                // then this mapping is not correct
                                                if (!innerOriginal.Equals(mappedTemplate))
                                                    return null;

                                                // Otherwise we continue with another mapped pair
                                                continue;
                                            }

                                            // If the current one isn't mapped, we can add it to the mapping
                                            newMapping.Add(innerTemplate, innerOriginal);
                                        }

                                        // If we got here, then the mapping is correct
                                        // Now we just add the current mapped objects
                                        newMapping.Add(template, original);

                                        // Make sure there are no duplicates in the mapping
                                        // If yes, then it's incorrect
                                        if (newMapping.Values.AnyDuplicates())
                                            return null;

                                        // Otherwise it's fine
                                        return newMapping;

                                    })
                                    // Take only correct results
                                    .Where(mapping => mapping != null);

                            // Unhandled cases
                            default:
                                throw new GeoGenException($"Unhandled type of {nameof(ConfigurationObject)}: {template.GetType()}");
                        }
                    })
                );

        #endregion
    }
}