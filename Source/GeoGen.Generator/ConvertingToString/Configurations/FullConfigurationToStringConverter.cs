using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="GeneratedConfiguration"/>,
    /// that converts a configuration to a string representing all the configurations isomorphic to it.
    /// This isomorphism is defined with this motivation: Assume we have two configurations, a triangle ABC
    /// with the midpoint M of BC, and a triangle ABC with the midpoint N of AC. They are not formally
    /// equal, but they're quite the same as far as the theorem finding is concerned. That's why we want 
    /// to keep exactly one of them, or in other words, say these two are equal.
    /// <para>
    /// The algorithm is following: For a given configuration we can say which configurations are isomorphic
    /// to it - the ones which can be obtained by reordering its loose objects in some way. For example,
    /// for a triangle we have 3 loose points, which yield 3! = 6 possible reorders of them, which means there
    /// might be at most 5 configurations isomorphic to it (even fewer, some of the might be equal). Therefore
    /// we might take a configuration, generate all the ones isomorphic to it, convert all of them to a string,
    /// and deterministically pick the representant, for example as the lexicographically minimal string.
    /// This is what indeed happens, but it is not implementing by creating new configurations. Instead, we
    /// convert the same configuration using different <see cref="LooseObjectIdsRemapping"/>s. For each of
    /// these mappings we have a single <see cref="IToStringConverter{T}"/> that is used together with a 
    /// <see cref="IGeneralConfigurationToStringConverter"/>.
    /// </para> 
    /// </summary>
    public class FullConfigurationToStringConverter : IToStringConverter<GeneratedConfiguration>
    {
        #region Dependencies

        /// <summary>
        /// The generic configuration to string converter to which the actual conversion is delegated.
        /// </summary>
        private readonly IGeneralConfigurationToStringConverter _configurationToString;

        #endregion

        #region Private fields

        /// <summary>
        /// The lazy evaluator for the list of all the generated to string converters which 
        /// are used to determine the unique represent of the symmetry class. For the details 
        /// see the documentation to the class.
        /// </summary>
        private Lazy<List<FuncToStringConverter<ConfigurationObject>>> _converters;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FullConfigurationToStringConverter"/> converter.
        /// </summary>
        /// <param name="looseObjects">The loose objects of every configuration used in this generation.</param>
        /// <param name="configurationToString">The generic configuration to string converter to which the actual conversion is delegated.</param>
        /// <param name="objectToString">The full object to string converter which is used together with the configuration to string converter.</param>
        public FullConfigurationToStringConverter(IReadOnlyList<LooseConfigurationObject> looseObjects, IGeneralConfigurationToStringConverter configurationToString, IFullObjectToStringConverter objectToString)
        {
            _configurationToString = configurationToString ?? throw new ArgumentNullException(nameof(configurationToString));

            // Construct the lazy evaluator of all possible to string converters, each representing
            // a single order of loose objects (in other words, a single remapping of loose objects)
            _converters = new Lazy<List<FuncToStringConverter<ConfigurationObject>>>(() =>
            {
                // We take all the permutations of the loose objects
                return looseObjects.Permutations()
                        // Cast each of them to the dictionary mapping those loose objects
                        // that are mapped to some id different then theirs
                        .Select(permutation =>
                        {
                            // Each permutation is first casted to the tuple of itself and id of the object to which this object is mapped
                            return permutation.Select((looseObject, index) => (looseObject, id : looseObjects[index].Id))
                                    // Then we exclude the ones that are mapped to itself
                                    .Where(pair => pair.looseObject.Id != pair.id)
                                    // And wrap the remaining ones to a dictionary
                                    .ToDictionary(pair => pair.looseObject, pair => pair.id);
                        })
                        // Wrap this dictionary into a LooseObjectIdsRemapping object, 
                        // making sure that we will reuse the empty remapping, if possible
                        .Select(dictionary => dictionary.IsEmpty() ? LooseObjectIdsRemapping.NoRemapping : new LooseObjectIdsRemapping(dictionary))
                        // Each of these remapping is then used to create a to string converter that is 
                        // required to be passed to the general configuration to string 
                        .Select(mapping => new FuncToStringConverter<ConfigurationObject>(obj => objectToString.ConvertToString(obj, mapping)))
                        // Finally wrap the result to a list
                        .ToList();
            });
        }

        #endregion

        #region IToStringConverter implementation

        /// <summary>
        /// Converts a given configuration to a string.
        /// </summary>
        /// <param name="configuration">The configuration to be converted.</param>
        /// <returns>A string representation of the configuration.</returns>
        public string ConvertToString(GeneratedConfiguration configuration)
        {
            // Initialize the minimal string
            string minimalString = null;

            // Determine the lexicographically minimal string representing the configuration
            // using all the converters, each representing a single order of the loose objects
            foreach (var converter in _converters.Value)
            {
                // Convert a given configuration to string using the gotten converter
                var stringVersion = _configurationToString.ConvertToString(configuration, converter);

                // If it's the first conversion or we have a smaller string...
                if (minimalString == null || string.CompareOrdinal(minimalString, stringVersion) < 0)
                {
                    // Set the minimal string to the current one 
                    minimalString = stringVersion;
                }
            }

            // We should have the minimal string now
            return minimalString;
        }

        #endregion
    }
}