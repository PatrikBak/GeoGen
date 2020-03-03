using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.TheoremProver
{
    public class InferenceRuleApplier : IInferenceRuleApplier
    {
        #region Mapping class

        /// <summary>
        /// A helper class that handles information about mapping between template objects and real objects. The main part
        /// is <see cref="ConfigurationObjectMapping"/> that contains mapping of template <see cref="ConfigurationObject"/>s
        /// of the <see cref="InferenceRule"/> that is being applied.
        /// <para>
        /// It also handles mapping template <see cref="TheoremObject"/>s. These objects are not necessary, because all the 
        /// needed information is in the mapping of configuration object templates, however, they are used to significantly 
        /// speed up the mapping algorithm.
        /// </para>
        /// </summary>
        private class Mapping
        {
            #region Private fields

            /// <summary>
            /// The dictionary mapping template configuration objects from the inference rule to real configuration objects.
            /// </summary>
            private readonly Dictionary<ConfigurationObject, ConfigurationObject> _configurationObjectMapping;

            /// <summary>
            /// The dictionary mapping template theorem objects from the inference rule's assumptions to real theorem objects.
            /// </summary>
            private readonly Dictionary<TheoremObject, TheoremObject> _theoremObjectMapping;

            /// <summary>
            /// The set of equalities that have been implicitly used in this mapping.
            /// </summary>
            private readonly HashSet<Theorem> _usedEqualities;

            #endregion

            #region Public properties

            /// <summary>
            /// The dictionary mapping template configuration objects from the inference rule to real configuration objects.
            /// This is a read-only view of <see cref="_configurationObjectMapping"/>.
            /// </summary>
            public IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> ConfigurationObjectMapping => _configurationObjectMapping;

            /// <summary>
            /// The equalities that have been implicitly used in this mapping. This is a read-only view of <see cref="_usedEqualities"/>.
            /// </summary>
            public IEnumerable<Theorem> UsedEqualities => _usedEqualities;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Mapping"/> class.
            /// </summary>
            public Mapping()
            {
                _configurationObjectMapping = new Dictionary<ConfigurationObject, ConfigurationObject>();
                _theoremObjectMapping = new Dictionary<TheoremObject, TheoremObject>();
                _usedEqualities = new HashSet<Theorem>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Mapping"/> class by cloning a given mapping
            /// and including a new mapped pair.
            /// </summary>
            /// <param name="mapping">The mapping to be cloned.</param>
            /// <param name="mappedPair">The new mapped pair to be included.</param>
            public Mapping(Mapping mapping, (ConfigurationObject templateObject, ConfigurationObject realObject) mappedPair)
            {
                // Clone the configuration object mapping dictionary
                _configurationObjectMapping = new Dictionary<ConfigurationObject, ConfigurationObject>(mapping.ConfigurationObjectMapping)
                {
                    // Add the newly mapped pair
                    { mappedPair.templateObject, mappedPair.realObject }
                };

                // Clone the theorem object mapping dictionary
                _theoremObjectMapping = new Dictionary<TheoremObject, TheoremObject>(mapping._theoremObjectMapping);

                // Clone the used equality collection
                _usedEqualities = new HashSet<Theorem>(mapping._usedEqualities);
            }

            #endregion

            #region Public methods

            /// <summary>
            /// Makes sure that every template theorem object that has all its needed components already
            /// map is included and mapped. 
            /// </summary>
            /// <param name="templateObjects">The template theorems objects to be included, if they can be mapped implicitly.</param>
            public void EnsureMappableObjectsAreMapped(IEnumerable<TheoremObject> templateObjects)
            {
                // Go through the given template theorem objects
                foreach (var templateObject in templateObjects)
                {
                    // If it's mapped, skip it
                    if (_theoremObjectMapping.ContainsKey(templateObject))
                        continue;

                    // It still might be mappable at this stage if its inner objects
                    var isMappable = templateObject.GetInnerConfigurationObjects()
                        // Are all mapped
                        .All(ConfigurationObjectMapping.ContainsKey);

                    // If it's not mappable, move on
                    if (!isMappable)
                        continue;

                    // Otherwise it is mappable and we do the mapping
                    var realObject = templateObject.Remap(ConfigurationObjectMapping);

                    // And include it in the mapping
                    _theoremObjectMapping.TryAdd(templateObject, realObject);
                }
            }

            /// <summary>
            /// Finds unmapped theorem object templates that are among given ones.
            /// </summary>
            /// <param name="templateObjects">The template theorem objects that should be reduced to unmapped ones.</param>
            /// <returns>The unmapped theorem object templates.</returns>
            public IReadOnlyList<TheoremObject> FindUnmappedObjects(IEnumerable<TheoremObject> templateObjects)
                // Take those template objects 
                => templateObjects
                    // That aren't in the mapping dictionary
                    .Where(templateObject => !_theoremObjectMapping.ContainsKey(templateObject))
                    // Enumerate
                    .ToArray();

            /// <summary>
            /// Finds unmapped configuration object templates that are among given ones.
            /// </summary>
            /// <param name="templateObjects">The template configuration objects that should be reduced to unmapped ones.</param>
            /// <returns>The unmapped configuration object templates.</returns>
            public IReadOnlyList<ConfigurationObject> FindUnmappedObjects(IEnumerable<ConfigurationObject> templateObjects)
                // Take those template objects 
                => templateObjects
                    // That aren't in the mapping dictionary
                    .Where(templateObject => !ConfigurationObjectMapping.ContainsKey(templateObject))
                    // Enumerate
                    .ToArray();

            /// <summary>
            /// Finds the real theorem objects that are mapped to passed template ones.
            /// </summary>
            /// <param name="templateObjects">The template theorem objects.</param>
            /// <returns>The set of real theorem objects mapped to the passed template ones.</returns>
            public IReadOnlyHashSet<TheoremObject> FindMappedRealObjects(IEnumerable<TheoremObject> templateObjects)
                // Take the template objects 
                => templateObjects
                    // Find their real mapped pair
                    .Select(_theoremObjectMapping.GetValueOrDefault)
                    // That is there
                    .Where(realObject => realObject != null)
                    // Enumerate
                    .ToReadOnlyHashSet();

            /// <summary>
            /// Finds the real configuration objects that are mapped to passed template ones.
            /// </summary>
            /// <param name="templateObjects">The template configuration objects.</param>
            /// <returns>The set of real configuration objects mapped to the passed template ones.</returns>
            public IReadOnlyHashSet<ConfigurationObject> FindMappedRealObjects(IEnumerable<ConfigurationObject> templateObjects)
                // Take the template objects 
                => templateObjects
                    // Find their real mapped pair
                    .Select(ConfigurationObjectMapping.GetValueOrDefault)
                    // That is there
                    .Where(realObject => realObject != null)
                    // Enumerate
                    .ToReadOnlyHashSet();

            /// <summary>
            /// Includes a given mapped pair of theorem objects to the mapping.
            /// </summary>
            /// <param name="mappedPair">The mapped pair of theorem objects.</param>
            /// <returns>The current mapping for chaining.</returns>
            public Mapping WithMappedPair((TheoremObject templateObject, TheoremObject realObject) mappedPair)
            {
                // Add the mapped template object
                _theoremObjectMapping.TryAdd(mappedPair.templateObject, mappedPair.realObject);

                // Return the mapping for chaining
                return this;
            }

            /// <summary>
            /// Includes the equality of given objects as a theorem layer returned in <see cref="UsedEqualities"/>.
            /// This equality is considered only if the objects are not trivially equal (i.e. via Equals).
            /// </summary>
            /// <param name="equalObjects">The pair of equal objects to be included.</param>
            /// <returns>The current mapping for chaining.</returns>
            public Mapping WithUsedEquality((ConfigurationObject object1, ConfigurationObject object2) equalObjects)
            {
                // Deconstruct
                var (object1, object2) = equalObjects;

                // If the objects are trivially equal, don't do anything
                if (object1.Equals(object2))
                    return this;

                // Otherwise create an equality theorem
                var equality = new Theorem(TheoremType.EqualObjects, object1, object2);

                // Include it
                _usedEqualities.Add(equality);

                // Return the mapping for chaining
                return this;
            }

            #endregion
        }

        #endregion

        #region IInferenceRuleApplier implementation

        /// <inheritdoc/>
        public IEnumerable<(Theorem inferredTheorem, Theorem[] negativeAssumptions, Theorem[] positiveAssumptions)> InferTheorems(InferenceRuleApplierInput input)
        {
            #region Pre-exclusion 

            // Make sure that for every construction
            var doWeHaveMappableDeclaredObject = input.InferenceRule.NeededConstructionTypes
                // The number of available objects is at least the needed count
                // NOTE: A premapped real object should be returned by the factory too, therefore it is counted in 
                .All(pair => input.MappableObjectsFactory(pair.Key).Count() >= pair.Value);

            // If we don't have enough objects of the right types, we're done
            if (!doWeHaveMappableDeclaredObject)
                return Enumerable.Empty<(Theorem, Theorem[], Theorem[])>();

            // Make sure that for every assumption
            var doWeHaveMappableAssumption = input.InferenceRule.NeededAssumptionTypes
                // The number of available theorems of this type is at least the needed count
                // NOTE: A premapped real assumption should be returned by the factory too, therefore it is counted in 
                .All(pair => input.MappableTheoremsFactory(pair.Key).Count() >= pair.Value);

            // If we don't have enough assumptions of the right types, we're done
            if (!doWeHaveMappableAssumption)
                return Enumerable.Empty<(Theorem, Theorem[], Theorem[])>();

            #endregion

            // Prepare the enumerable of the current considered mapping, initially an empty mapping
            var mappings = new Mapping().ToEnumerable();

            #region Pre-mapping

            // If there is a premapped object, do the pre-mapping
            if (input.PremappedObject != null)
                mappings = mappings.SelectMany(mapping => Map(mapping, input.PremappedObject.Value, input.NormalizationFunction, input.EqualObjectsFactory));

            // If there is a premapped assumption, do the pre-mapping
            if (input.PremappedAssumption != null)
            {
                // Deconstruct
                var (template, real) = input.PremappedAssumption.Value;

                // Do the pre-mapping
                mappings = mappings.SelectMany(mapping => Map(mapping, input.InferenceRule.ExplicitObjectsMappableToImplicitOnes, template, real.ToEnumerable(), input.NormalizationFunction, input.EqualObjectsFactory));
            }

            // If there is a premapped conclusion, do the pre-mapping
            if (input.PremappedConclusion != null)
                mappings = mappings.SelectMany(mapping => Map(mapping, input.InferenceRule.ExplicitObjectsMappableToImplicitOnes, input.InferenceRule.Conclusion, input.PremappedConclusion.ToEnumerable(), input.NormalizationFunction, input.EqualObjectsFactory));

            #endregion

            // We need to gradually include the declared objects
            return input.InferenceRule.DeclaredObjects.Aggregate
                (
                    // The initial seed is enumerable containing the current mappings
                    mappings,

                    // The mapping function takes currently available mappings and includes the current template
                    (currentMappings, templateObject) =>
                    {
                        // If this object has been premapped, we're fine, since it's already included
                        if (input.PremappedObject != null && input.PremappedObject.Value.declaredTemplateObject.Equals(templateObject))
                            return currentMappings;

                        // Otherwise the object hasn't been premapped and we need to find options for it
                        // We need to take the real constructed objects that are suitable
                        return input.MappableObjectsFactory(templateObject.Construction)
                            // Every such real object will yield more results
                            .SelectMany(realObject => currentMappings
                                // For every currently available mapping we will use the lower function to yield correct mappings
                                .SelectMany(mapping => Map(mapping, (templateObject, realObject), input.NormalizationFunction, input.EqualObjectsFactory)));
                    }
                )
                // Now we have mapped the declared objects. 
                // We need to gradually include the flattened assumptions too
                .SelectMany(currentMapping => input.InferenceRule.AssumptionGroups.Flatten().Aggregate
                    (
                        // The initial seed will be the current mapping
                        currentMapping.ToEnumerable(),

                        // The mapping function takes currently available mappings and includes the current assumption template
                        (currentMappings, templateAssumption) =>
                        {
                            // If this assumption has been premapped, we're fine, since it's already included
                            if (input.PremappedAssumption != null && input.PremappedAssumption.Value.templateAssumption.Equals(templateAssumption))
                                return currentMappings;

                            // Otherwise the assumption hasn't been premapped and we need to find options for it
                            var realAssumptions = input.MappableTheoremsFactory(templateAssumption.Type);

                            // For currently available mappings  
                            return currentMappings.SelectMany(mapping =>
                                // We will find options to include the real assumptions using the lower method
                                Map(mapping, input.InferenceRule.ExplicitObjectsMappableToImplicitOnes, templateAssumption, realAssumptions, input.NormalizationFunction, input.EqualObjectsFactory));
                        }
                    ))
                // Now we finally have mappings that include the assumptions and we can find the conclusion
                .Select(mapping =>
                {
                    // If the conclusion is premapped...
                    return input.PremappedConclusion != null
                        // Then use this value (and include the mapping too)
                        ? (conclusion: input.PremappedConclusion, mapping)
                        // Otherwise do the remapping (and include the mapping too)
                        : (conclusion: input.InferenceRule.Conclusion.Remap(mapping.ConfigurationObjectMapping,
                            // While premapping, we need to take into account that we might have created artificial
                            // objects such as LineTheoremObject with an explicit object LineFromPoints(A, B) and we 
                            // need to replace these with implicit ones to get a clear statement that is expected
                            flattenObjectsFromPoints: true), mapping);
                })
                // Take only those pairs where the conclusion has been successfully remapped
                .Where(pair => pair.conclusion != null)
                // Finally create the final triple of the conclusion, 
                .Select(pair => (pair.conclusion,
                    // Negative assumptions that are taken from the rule
                    negativeAssumptions: input.InferenceRule.NegativeAssumptions
                            // Each is remapped
                            .Select(assumption => assumption.Remap(pair.mapping.ConfigurationObjectMapping))
                            // And those that are not degenerated are taken (degenerated means not true, which is fine)
                            .Where(assumption => assumption != null)
                            // Enumerate 
                            .ToArray(),
                    // And the positive assumptions that are taken from the rule's assumption groups
                    positiveAssumptions: input.InferenceRule.AssumptionGroups.Flatten()
                        // Map each
                        .Select(assumption => assumption.Remap(pair.mapping.ConfigurationObjectMapping,
                            // While premapping, we need to take into account that we might have created artificial
                            // objects such as LineTheoremObject with an explicit object LineFromPoints(A, B) and we 
                            // need to replace these with implicit ones to get a clear statement that is expected
                            flattenObjectsFromPoints: true))
                        // Include the used equalities as assumptions
                        .Concat(pair.mapping.UsedEqualities)
                        // Enumerate
                        .ToArray()));
        }

        /// <summary>
        /// Finds all available mappings where a given template theorem to mapped to one of provided real ones.
        /// </summary>
        /// <param name="currentMapping">The current mapping that is being performed.</param>
        /// <param name="explicitObjectsMappableToImplicitOnes">The set <see cref="InferenceRule.ExplicitObjectsMappableToImplicitOnes"/> from the rule being applied.</param>
        /// <param name="templateTheorem">The template theorem to be mapped.</param>
        /// <param name="potentialRealTheorems">The enumerable of available real theorems that may be mapped to the template one.</param>
        /// <param name="normalizationFunction">The normalization function. For more information about normalized see the documentation of <see cref="NormalizationHelper"/>.</param>
        /// <param name="equalObjectFunction">The function for retrieving equal objects. For more information see the documentation of <see cref="InferenceRuleApplierInput"/>.</param>
        /// <returns>The available mappings that include the given options.</returns>
        private static IEnumerable<Mapping> Map(Mapping currentMapping,
                                                IReadOnlyHashSet<TheoremObjectWithPoints> explicitObjectsMappableToImplicitOnes,
                                                Theorem templateTheorem,
                                                IEnumerable<Theorem> potentialRealTheorems,
                                                Func<ConfigurationObject, ConfigurationObject> normalizationFunction,
                                                Func<ConfigurationObject, Construction, IEnumerable<ConstructedConfigurationObject>> equalObjectFunction)
        {
            // First ensure that all the template objects are mapped 
            currentMapping.EnsureMappableObjectsAreMapped(templateTheorem.InvolvedObjects);

            // Find the templates that are to be mapped yet
            var unmappedTemplates = currentMapping.FindUnmappedObjects(templateTheorem.InvolvedObjects);

            // Find to what the inner real objects are mapped already
            var mappedRealObjects = currentMapping.FindMappedRealObjects(templateTheorem.InvolvedObjects);

            // Take the available theorems
            return potentialRealTheorems
                // For each take those objects
                .Select(theorem => theorem.InvolvedObjects
                    // That aren't mapped yet
                    .Where(theoremObject => !mappedRealObjects.Contains(theoremObject))
                    // Enumerate
                    .ToArray())
                // Take only those whose count corresponds to the needed number of objects to map
                .Where(unmappedObjects => unmappedTemplates.Count == unmappedObjects.Length)
                // Such objects will represent a potentially correct mapping
                .SelectMany(unmappedObjects => unmappedObjects
                    // These objects can be in any order
                    .Permutations()
                    // For each such an order
                    .SelectMany(orderedUnmappedObjects =>
                        // Zip the objects to the mapped pairs
                        unmappedTemplates.Zip(orderedUnmappedObjects)
                            // Aggregate the mappings to include every pair
                            .Aggregate
                            (
                                // The initial seed will be the current mapping
                                currentMapping.ToEnumerable(),

                                // The mapping function takes currently available mappings and for each includes the given pair
                                (currentMappings, mappedPair) => currentMappings.SelectMany(mapping => Map(mapping, mappedPair,
                                    // Allow explicit to implicit object mapping only if the template is among those for which it is allowed
                                    allowExplicitToImplicitObjectMapping: explicitObjectsMappableToImplicitOnes.Contains(mappedPair.First), normalizationFunction, equalObjectFunction))
                            )));
        }

        /// <summary>
        /// Finds all available mappings that include a given pair of the template and real object.
        /// </summary>
        /// <param name="currentMapping">The current mapping that is being performed.</param>
        /// <param name="mappedPair">The (template, real) pair that is to be mapped.</param>
        /// <param name="allowExplicitToImplicitObjectMapping">Indicates whether we may map a template object of type <see cref="TheoremObjectWithPoints"/>
        /// that are <see cref="TheoremObjectWithPoints.DefinedByExplicitObject"/> to a one <see cref="TheoremObjectWithPoints.DefinedByPoints"/>.</param>
        /// <param name="normalizationFunction">The normalization function. For more information about normalized see the documentation of <see cref="NormalizationHelper"/>.</param>
        /// <param name="equalObjectFunction">The function for retrieving equal objects. For more information see the documentation of <see cref="InferenceRuleApplierInput"/>.</param>
        /// <returns>The available mappings that include the given pair.</returns>
        private static IEnumerable<Mapping> Map(Mapping currentMapping,
                                                (TheoremObject templateObject, TheoremObject realObject) mappedPair,
                                                bool allowExplicitToImplicitObjectMapping,
                                                Func<ConfigurationObject, ConfigurationObject> normalizationFunction,
                                                Func<ConfigurationObject, Construction, IEnumerable<ConstructedConfigurationObject>> equalObjectFunction)
        {
            // Deconstruct
            var (templateObject, realObject) = mappedPair;

            // If they don't have the same type, then there is no option to map them
            if (!templateObject.GetType().Equals(realObject.GetType()))
                return Enumerable.Empty<Mapping>();

            // Otherwise switch based on their common type
            switch (templateObject)
            {
                // If we have a point
                case PointTheoremObject templatePoint:

                    // Cast the other object (that we already know has the same type)
                    var realPoint = (PointTheoremObject)realObject;

                    // Delegate the call to the function mapping objects
                    return Map(currentMapping, (templatePoint.ConfigurationObject, realPoint.ConfigurationObject), normalizationFunction, equalObjectFunction)
                        // Include the mapped theorem object pair
                        .Select(mapping => mapping.WithMappedPair(mappedPair));

                // If we have an object with points, i.e. a line or circle
                case TheoremObjectWithPoints templateObjectWithPoints:

                    // Cast the other object (that we already know has the same type)
                    var realObjectWithPoints = (TheoremObjectWithPoints)realObject;

                    // If the template is explicit...
                    if (templateObjectWithPoints.DefinedByExplicitObject)
                    {
                        // And the real explicit is explicit too
                        if (realObjectWithPoints.DefinedByExplicitObject)
                            // Delegate the call to the function mapping their objects
                            return Map(currentMapping, (templateObjectWithPoints.ConfigurationObject, realObjectWithPoints.ConfigurationObject), normalizationFunction, equalObjectFunction)
                                // Include the mapped theorem object pair
                                .Select(mapping => mapping.WithMappedPair(mappedPair));

                        // If the mapping of an explicit to implicit object is not allowed, we can't do the mapping
                        if (!allowExplicitToImplicitObjectMapping)
                            return Enumerable.Empty<Mapping>();

                        // Otherwise we're trying to map an explicit object to an implicit one
                        // We can do it by faking the real object as an explicit one using the right construction
                        var maskedRealObject = templateObjectWithPoints switch
                        {
                            // Line case means we create a line from points
                            LineTheoremObject _ => new ConstructedConfigurationObject(LineFromPoints, realObjectWithPoints.PointsList.ToArray()),

                            // Circle case means we create a circumcircle
                            CircleTheoremObject _ => new ConstructedConfigurationObject(Circumcircle, realObjectWithPoints.PointsList.ToArray()),

                            // Unhandled cases
                            _ => throw new TheoremProverException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {templateObjectWithPoints.GetType()}")
                        };

                        // Now we can do delegate the call to the function mapping objects
                        return Map(currentMapping, (templateObjectWithPoints.ConfigurationObject, maskedRealObject), normalizationFunction, equalObjectFunction)
                                // Include the mapped theorem object pair
                                .Select(mapping => mapping.WithMappedPair(mappedPair));
                    }

                    // If we got here, we know the template is defined by points
                    // If the real object isn't, we can't do the mapping
                    if (!realObjectWithPoints.DefinedByPoints)
                        return Enumerable.Empty<Mapping>();

                    // Otherwise they are both defined by points and these points
                    return templateObjectWithPoints.Points
                            // Can be in any order
                            .Permutations()
                            // For each such an order
                            .SelectMany(innerTemplatePoints =>
                                // Zip the objects to the mapped pairs
                                innerTemplatePoints.Zip(realObjectWithPoints.Points)
                                    // Aggregate the mappings to include every pair
                                    .Aggregate
                                    (
                                        // The initial seed will be the current mapping
                                        currentMapping.ToEnumerable(),

                                        // The mapping function takes currently available mappings 
                                        (currentMappings, mappedPair) => currentMappings
                                            // And from each finds new ones by including the given pair
                                            .SelectMany(mapping => Map(mapping, mappedPair, normalizationFunction, equalObjectFunction))
                                    ))
                            // Include the mapped theorem object pair
                            .Select(mapping => mapping.WithMappedPair(mappedPair));

                // If we have a line segment
                case LineSegmentTheoremObject templateSegment:

                    // Cast the other object ((that we already know has the same type)
                    var realLineSegment = (LineSegmentTheoremObject)realObject;

                    // Its paired points can be in any order
                    return templateSegment.PointSet.Permutations()
                        // For each such an order
                        .SelectMany(innerTemplatePoints =>
                            // Zip the points to mapped pairs
                            innerTemplatePoints.Zip(realLineSegment.PointSet)
                                // Aggregate the mappings to include both these pairs
                                .Aggregate
                                (
                                    // The initial seed will be the current mapping
                                    currentMapping.ToEnumerable(),

                                    // The mapping function takes currently available mappings by including the given pair
                                    (currentMappings, mappedPair) => currentMappings
                                        // And from each finds new ones by including the given pair
                                        .SelectMany(mapping => Map(mapping, mappedPair, allowExplicitToImplicitObjectMapping, normalizationFunction, equalObjectFunction))
                                ));

                // Unhandled cases
                default:
                    throw new TheoremProverException($"Unhandled type of {nameof(TheoremObject)}: {templateObject.GetType()}");
            }
        }

        /// <summary>
        /// Finds all available mappings that include a given pair of the template and real object.
        /// </summary>
        /// <param name="currentMapping">The current mapping that is being performed.</param>
        /// <param name="mappedPair">The (template, real) pair that is to be mapped.</param>
        /// <param name="normalizationFunction">The normalization function. For more information about normalized see the documentation of <see cref="NormalizationHelper"/>.</param>
        /// <param name="equalObjectFunction">The function for retrieving equal objects. For more information see the documentation of <see cref="InferenceRuleApplierInput"/>.</param>
        /// <returns>The available mappings that include the given pair.</returns>
        private static IEnumerable<Mapping> Map(Mapping currentMapping,
                                                (ConfigurationObject templateObject, ConfigurationObject realObject) mappedPair,
                                                Func<ConfigurationObject, ConfigurationObject> normalizationFunction,
                                                Func<ConfigurationObject, Construction, IEnumerable<ConstructedConfigurationObject>> equalObjectFunction)
        {
            // Deconstruct
            var (templateObject, realObject) = mappedPair;

            // Find the normalized real object, since we want to map templates only to normalized versions
            // If the object has no normal function, then it might be just because it is an artificial object
            // with construction of type LineFromPoints or Circumcenter. We will let it go
            var normalizedRealObject = normalizationFunction(realObject) ?? realObject;

            // If the current template is mapped...
            if (currentMapping.ConfigurationObjectMapping.ContainsKey(templateObject))
                // Then if they are mapped to the same thing
                return normalizedRealObject.Equals(currentMapping.ConfigurationObjectMapping[templateObject])
                    // Then we don't need to modify this mapping
                    ? currentMapping.ToEnumerable()
                    // Otherwise we are in a contradiction
                    : Enumerable.Empty<Mapping>();

            // If we got here, then the current template object is not mapped. If some template is already
            // mapped to the current real object, then we have a problem, since we want mappings to be 1 to 1
            if (currentMapping.ConfigurationObjectMapping.Values.Contains(normalizedRealObject))
                return Enumerable.Empty<Mapping>();

            // Now we can finally look at the objects. Switch based on the type of the template one
            switch (templateObject)
            {
                // If we have loose template...
                case LooseConfigurationObject _:

                    // The mapping is certainly correct and we may include templateObject --> normalizedRealObject
                    return new Mapping(currentMapping, (templateObject, normalizedRealObject))
                        // Add the potential normalization equality to the mapping
                        .WithUsedEquality((realObject, normalizedRealObject))
                        // Return as an enumerable
                        .ToEnumerable();

                // If we have a constructed template...
                case ConstructedConfigurationObject constructedTemplate:

                    // Then the real object must also be constructed
                    if (!(realObject is ConstructedConfigurationObject constructedRealObject))
                        return Enumerable.Empty<Mapping>();

                    // And must have the same construction as the template
                    if (!constructedRealObject.Construction.Equals(constructedTemplate.Construction))
                        return Enumerable.Empty<Mapping>();

                    // Get the inner templates to be mapped
                    var innerTemplates = constructedTemplate.PassedArguments.FlattenedList;

                    // Find those of them that are to be mapped yet
                    var unmappedInnerTemplates = currentMapping.FindUnmappedObjects(innerTemplates);

                    // Find to what the inner real objects are mapped already
                    var mappedInnerRealObjects = currentMapping.FindMappedRealObjects(innerTemplates);

                    // Find the inner real objects to be mapped by taking the inner objects of the real object
                    var innerRealObjectsToBeMapped = constructedRealObject.PassedArguments.FlattenedList
                        // Take those that are not among the already mapped objects
                        .Where(innerRealObject => !mappedInnerRealObjects.Contains(innerRealObject))
                        // Enumerate
                        .ToArray();

                    // The number of templates to be mapped must match the number of objects that are not mapped
                    // NOTE: Based on the normalization invariant, the inner objects of the real objects should be
                    //       normalized and we are also mapping templates only to normalized version thanks to the
                    //       normalization function, so this call works correctly
                    if (unmappedInnerTemplates.Count != innerRealObjectsToBeMapped.Length)
                        return Enumerable.Empty<Mapping>();

                    // If we got here, then we need to finally try some mapping. Every permutation of the templates
                    return unmappedInnerTemplates.Permutations()
                        // Is zipped with the real objects to be mapped
                        .Select(permutation => permutation.Zip(innerRealObjectsToBeMapped))
                        // We need to gradually include these pairs
                        .SelectMany(pairs => pairs.Aggregate
                        (
                            // The initial seed will be the current mapping
                            currentMapping.ToEnumerable(),

                            // The mapping function takes currently available mappings and includes the current mapped inner pair
                            (currentMappings, innerPair) =>
                            {
                                // Deconstruct
                                var (innerTemplate, innerReal) = innerPair;

                                // Include the pair in each of the currently available mappings based on the template type
                                return currentMappings.SelectMany(mapping => innerTemplate switch
                                {
                                    // If we have a loose inner template, then we simply recursively call this method,
                                    // which will take care of all the boring work from the beginning of the method
                                    LooseConfigurationObject _ => Map(mapping, innerPair, normalizationFunction, equalObjectFunction),

                                    // If we have a constructed template, then it might mapped to any equal object to the current real
                                    // that has the right construction (see the InferenceRuleApplierInput for an example)
                                    ConstructedConfigurationObject constructedTemplate => equalObjectFunction(innerReal, constructedTemplate.Construction)
                                        // Each such an option is then handled recursively with the call of this method 
                                        // NOTE: The maximal level of recursion here is at most the number of declared objects of an
                                        //       inference rule, which in practice is not a lot
                                        .SelectMany(equalObject => Map(mapping, (constructedTemplate, equalObject), normalizationFunction, equalObjectFunction)),

                                    // Unhandled cases
                                    _ => throw new TheoremProverException($"Unhandled type of {nameof(ConfigurationObject)}: {innerTemplate.GetType()}")
                                });
                            }
                        ))
                        // Successful mapping might still not be correct, because we have taken all possible permutations of inner
                        // templates and some of them might not represents a correct object after reconstructing it (for example
                        // PointReflection(X, Y) is not equal to PointReflection(Y, X)). Therefore we need to check the mapping for this
                        .Where(mapping =>
                        {
                            // Find the inner real objects that resulted from the mapping by taking the inner templates
                            var innerRealObjects = innerTemplates
                                // Each template has to be mapped already
                                .Select(template => mapping.ConfigurationObjectMapping[template])
                                // Enumerate
                                .ToArray();

                            // Check if we can even create an object like this
                            // NOTE: This call works thanks to the fact that both objects have their arguments normalized
                            return constructedRealObject.CanWeReorderArgumentsLikeThis(innerRealObjects);
                        })
                        // For each correct mapping we can now freely include templateObject --> normalizedRealObject 
                        .Select(mapping => new Mapping(mapping, (templateObject, normalizedRealObject))
                            // And also the potential normalization equality
                            .WithUsedEquality((realObject, normalizedRealObject)));

                // Unhandled cases
                default:
                    throw new TheoremProverException($"Unhandled type of {nameof(ConfigurationObject)}: {templateObject.GetType()}");
            }
        }

        #endregion
    }
}