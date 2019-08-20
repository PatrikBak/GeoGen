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
    /// The algorithm is the following: For a given configuration we can say which configurations are isomorphic
    /// to it - the ones which can be obtained by reordering its loose objects in some way. For example,
    /// for a triangle we have 3 loose points, which yield 3! = 6 possible reorders of them, which means there
    /// might be at most 5 configurations isomorphic to it (even fewer, some of the might be equal). Therefore
    /// we might take a configuration, generate all the ones isomorphic to it, convert all of them to a string,
    /// and deterministically pick the representant, for example as the lexicographically minimal string.
    /// This is what indeed happens, but it is not implementing by creating new configurations. Instead, we
    /// convert the same configuration using different <see cref="LooseObjectsRemapping"/>s. For each of
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

        /// <summary>
        /// The full object to string converter which is used together with the configuration to string converter.
        /// </summary>
        private readonly IFullObjectToStringConverter _objectToString;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping loose object holders to the list of all the created to string
        /// converters which are used to determine the unique represent of the symmetry class.
        /// For the details see the documentation to the class.
        /// </summary>
        private readonly Dictionary<LooseObjectsHolder, List<FuncToStringConverter<ConfigurationObject>>> _convertersMap = new Dictionary<LooseObjectsHolder, List<FuncToStringConverter<ConfigurationObject>>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FullConfigurationToStringConverter"/> converter.
        /// </summary>
        /// <param name="configurationToString">The generic configuration to string converter to which the actual conversion is delegated.</param>
        /// <param name="objectToString">The full object to string converter which is used together with the configuration to string converter.</param>
        public FullConfigurationToStringConverter(IGeneralConfigurationToStringConverter configurationToString, IFullObjectToStringConverter objectToString)
        {
            _configurationToString = configurationToString ?? throw new ArgumentNullException(nameof(configurationToString));
            _objectToString = objectToString ?? throw new ArgumentNullException(nameof(objectToString));
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
            // Get the converters, or create and return them if they are not created yet
            return _convertersMap.GetOrAdd(configuration.LooseObjectsHolder, () =>
                {
                    // Get the loose objects list for comfort
                    var looseObjects = configuration.LooseObjects;

                    // TODO: This might not be the best solution. It works for layouts
                    //       where the objects can be in any order, but otherwise 
                    //       the behavior seems incorrect and random. 
                    // 
                    // Construct all possible to string converters, each representing
                    // a single order of loose objects (in other words, a single remapping of loose objects)
                    // We take all the permutations of the loose objects
                    return looseObjects.Permutations()
                            // Cast each of them to the dictionary mapping those loose objects
                            // that are mapped to some different ones
                            .Select(permutation =>
                            {
                                // Each permutation is first casted to the tuple of the object that is mapped to the result of the mapping
                                return permutation.Select((looseObject, index) => (from: looseObjects[index], to: looseObject))
                                        // Then we exclude the ones that are mapped to itself
                                        .Where(pair => pair.from != pair.to)
                                        // And wrap the remaining ones to a dictionary
                                        .ToDictionary(pair => pair.from, pair => pair.to);
                            })
                            // Wrap this dictionary into a LooseObjectsRemapping object, 
                            // making sure that we will reuse the empty remapping, if possible
                            .Select(dictionary => dictionary.IsEmpty() ? LooseObjectsRemapping.NoRemapping : new LooseObjectsRemapping(dictionary))
                            // Each of these remapping is then used to create a to string converter that is 
                            // required to be passed to the general configuration to string 
                            .Select(mapping => new FuncToStringConverter<ConfigurationObject>(obj => _objectToString.ConvertToString(obj, mapping)))
                            // Finally wrap the result to a list
                            .ToList();
                })
                // Use each of them to convert the configuration to a string
                .Select(converter => _configurationToString.ConvertToString(configuration, converter))
                // Take the lexicographically the smallest one
                .Min();
        }

        #endregion
    }
}