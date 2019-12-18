using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// A helper class that is able to detect whether a given <see cref="Configuration"/> should be excluded
    /// during the generation process. This is done via a non-trivial algorithm that assigns to every
    /// set of equal configurations that could be generated the unique representant that was the 
    /// representant of such a set in the previous iteration.
    /// </summary>
    public class GenerationContext
    {
        #region Private fields

        /// <summary>
        /// The assigned ids of the loose objects of the initial and therefore any subsequent configuration.
        /// </summary>
        private readonly Dictionary<LooseConfigurationObject, int> _looseObjectsId = new Dictionary<LooseConfigurationObject, int>();

        /// <summary>
        /// The assigned ids of constructions that might appear in object's definitions.
        /// </summary>
        private readonly Dictionary<Construction, int> _constructionsId = new Dictionary<Construction, int>();

        /// <summary>
        /// The initial objects of every configuration.
        /// </summary>
        private readonly IReadOnlyList<ConfigurationObject> _initialObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationContext"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration of the generation process.</param>
        /// <param name="constructions">The constructions used in the generation process.</param>
        public GenerationContext(Configuration initialConfiguration, IReadOnlyHashSet<Construction> constructions)
        {
            // Set the initial objects
            _initialObjects = initialConfiguration.ConstructedObjects;

            // Assign ids to loose objects
            initialConfiguration.LooseObjects.ForEach((looseObject, index) => _looseObjectsId.Add(looseObject, index));

            // Assign ids to constructions used in the generation
            constructions
                // As well as to the constructions from the initial objects that are not a part of the generation
                .Concat(initialConfiguration.ConstructedObjects.Select(constructedObject => constructedObject.Construction))
                // We need distinct ones so that we don't identify the same one twice
                .Distinct()
                // Add the id to the dictionary
                .ForEach((construction, index) => _constructionsId.Add(construction, index));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds out if the configuration should be excluded by the algorithm, because it is 
        /// not the representant of the equivalence class of equal configurations.
        /// </summary>
        /// <param name="configuration">The configuration that should be tested for exclusion.</param>
        /// <returns>true, if the configuration should be excluded; false otherwise</returns>
        public bool ShouldBeExcluded(Configuration configuration)
        {
            // We're going to take all configurations equal to this that can be generated (contain the 
            // initial constructed objects) and for each look for their 'normal', minimal order of the 
            // remaining added constructed objects. This way we determine the unique representant that
            // can be generated. It's not easy to see why this works...

            // Take all mappings to symmetry classes
            var normalOrderOfAddedObjects = configuration.LooseObjectsHolder.GetSymmetryMappings()
                // For a given mapping take the constructed objects
                .Select(mapping => configuration.ConstructedObjects
                    // Reconstruct them
                    .Select(construtedObject => construtedObject.Remap(mapping))
                    // Exclude the initial objects
                    .Except(_initialObjects)
                    // Enumerate
                    .ToArray())
                // Take only those mappings that don't change the initial objects
                // Other configurations certainly won't be correct
                .Where(objects => objects.Length + _initialObjects.Count == configuration.ConstructedObjects.Count)
                // For each set of added constructed objects we find the normal order
                .Select(addedObjects => addedObjects
                        // Consider their permutations
                        .Permutations()
                        // That represent a correctly constructible order
                        .Where(permutation => permutation.RepresentsConstructibleOrder())
                        // Enumerate each
                        .Select(permutation => (permutation: permutation.ToArray(),
                            // And also convert each permutation to an array of string
                            strings: permutation.Select(ConfigurationObjectToString).ToArray()))
                        // Take the one with lexicographically smallest one
                        .Min((pair1, pair2) => pair1.strings.CompareToLexicographically(pair2.strings))
                        // Unwrap the permutation
                        .permutation)
                // From each order we find its string representation (again)
                .Select(pair => (order: pair.ToArray(), strings: pair.Select(ConfigurationObjectToString).ToArray()))
                // Find the lexicographically smallest one
                .MinItem(pair => pair.strings, Comparer<string[]>.Create((a1, a2) => a1.CompareToLexicographically(a2)))
                // And unwrap the order
                .order;

            // The configuration is correct if and only if its order of added objects is the normal object
            // Therefore the take the constructed objects
            return !configuration.ConstructedObjects
                // From the ones that have been added
                .ItemsBetween(_initialObjects.Count, configuration.ConstructedObjects.Count)
                // And sequentially compare them with the normal order
                .SequenceEqual(normalOrderOfAddedObjects);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a given configuration object to a string.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be converted.</param>
        /// <returns>The string representation of the configuration object.</returns>
        private string ConfigurationObjectToString(ConfigurationObject configurationObject) => configurationObject switch
        {
            // If we have a loose object, simply check the dictionary
            LooseConfigurationObject looseObject => _looseObjectsId[looseObject].ToString(),

            // If we have a constructed object, then add the construction 
            ConstructedConfigurationObject constructedObject => $"{_constructionsId[constructedObject.Construction]}" +
                    // And the joined arguments
                    $"({constructedObject.PassedArguments.Select(ArgumentToString).ToJoinedString(",")})",

            // Something else...
            _ => throw new GeneratorException($"Unhandled type of {nameof(ConfigurationObject)}: {configurationObject.GetType()}")
        };

        /// <summary>
        /// Converts a given argument to a string.
        /// </summary>
        /// <param name="argument">The argument to be converted.</param>
        /// <returns>The string representation of the argument.</returns>
        private string ArgumentToString(ConstructionArgument argument) => argument switch
        {
            // If we have an object arguments, then use the method to convert its inner object
            ObjectConstructionArgument objectArgument => ConfigurationObjectToString(objectArgument.PassedObject),

            // If we have a set argument, then convert the individual arguments, join them, and wrap in curly braces
            SetConstructionArgument setArgument => $"{{{setArgument.PassedArguments.Select(ArgumentToString).Ordered().ToJoinedString(";")}}}",

            // Something else...
            _ => throw new GeneratorException($"Unhandled type of {nameof(ConstructionArgument)}: {argument.GetType()}")
        };

        #endregion
    }
}