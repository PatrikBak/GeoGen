using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that is used to define <see cref="ConstructedConfigurationObject"/>s.
    /// It's given by the input signature, which is a list of <see cref="ConstructionParameter"/>s, 
    /// and the output signature, which is a list of <see cref="ConfigurationObjectType"/>s. 
    /// </summary>
    public abstract class Construction : IdentifiedObject
    {
        #region Public abstract properties

        /// <summary>
        /// Gets the construction input signature, i.e. the list of construction parameters.
        /// </summary>
        public abstract IReadOnlyList<ConstructionParameter> ConstructionParameters { get; }

        public abstract ConfigurationObjectType OutputType { get; }

        /// <summary>
        /// Gets the name of the construction.
        /// </summary>
        public abstract string Name { get; }

        #endregion

        private Lazy<IReadOnlyDictionary<ConfigurationObjectType, int>> _objectTypesToNeededCountInitializer;

        /// <summary>
        /// Gets or sets the dictionary mapping configuration objects types 
        /// to the number of objects of that type that are needed to be 
        /// passed to the construction.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount => _objectTypesToNeededCountInitializer.Value;

        #region Constructor

        protected Construction()
        {
            _objectTypesToNeededCountInitializer = new Lazy<IReadOnlyDictionary<ConfigurationObjectType, int>>(DetermineObjectTypesToCount);
        }

        /// <summary>
        /// Gets a dictionary mapping object types to number of objects of that type needed
        /// to be passed to a given construction.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The dictionary.</returns>
        private Dictionary<ConfigurationObjectType, int> DetermineObjectTypesToCount()
        {
            // Initialize the resulting dictionary
            var dictionary = new Dictionary<ConfigurationObjectType, int>();

            // Local function that calculated the needed object types for one parameter
            void Examine(ConstructionParameter parameter)
            {
                // If we have an object parameter...
                if (parameter is ObjectConstructionParameter objectParameter)
                {
                    // Then we get this type
                    var type = objectParameter.ExpectedType;

                    // Make sure it's in the dictionary...
                    if (!dictionary.ContainsKey(type))
                        dictionary.Add(type, 0);

                    // Mark its occurrence
                    dictionary[type]++;

                    // And finish the method
                    return;
                }

                // Otherwise we have a set parameter
                var setParameter = (SetConstructionParameter) parameter;

                // We perform this examination on the inner parameters the given number of times
                // (not the most efficient way, but it definitely doesn't matter here)
                GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () => Examine(setParameter.TypeOfParameters));
            }

            // Perform the examination on the individual parameters
            ConstructionParameters.ForEach(Examine);

            // Return the already filled dictionary
            return dictionary;
        }

        #endregion
    }
}