using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a construction that is used to define <see cref="ConstructedConfigurationObject"/>s.
    /// It's given by the input signature, which is a list of <see cref="ConstructionParameter"/>s, 
    /// and the <see cref="ConfigurationObjectType"/> of the output object. Every construction has 
    /// a name that should be the unique identifier for it. It inherits from <see cref="IdentifiedObject"/>,
    /// since some GeoGen algorithms require to work with an integer id. 
    /// </summary>
    public abstract class Construction : IdentifiedObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction input signature, i.e. the list of construction parameters.
        /// </summary>
        public IReadOnlyList<ConstructionParameter> Parameters { get; }

        /// <summary>
        /// Gets the output type of the construction.
        /// </summary>
        public ConfigurationObjectType OutputType { get; }

        /// <summary>
        /// Gets the name of the construction. The name should be its unique identifier.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the dictionary mapping configuration objects types to the number of 
        /// objects of that type that are needed to create corresponding <see cref="Arguments"/>.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Construction"/> class.
        /// </summary>
        /// <param name="name">The name of the construction.</param>
        /// <param name="parameters">The parameters representing the signature of the construction.</param>
        /// <param name="outputType">The output type of the construction.</param>
        protected Construction(string name, IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectType outputType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            OutputType = outputType;
            ObjectTypesToNeededCount = DetermineObjectTypesToCount();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets a dictionary mapping object types to the number of objects of that type 
        /// that this construction requires.
        /// </summary>
        /// <returns>The dictionary mapping object types to the needed counts.</returns>
        private Dictionary<ConfigurationObjectType, int> DetermineObjectTypesToCount()
        {
            // Initialize the resulting dictionary
            var dictionary = new Dictionary<ConfigurationObjectType, int>();

            // Local function that calculates the needed object types for one parameter
            // and updates the dictionary
            void Examine(ConstructionParameter parameter)
            {
                // If we have an object parameter...
                if (parameter is ObjectConstructionParameter objectParameter)
                {
                    // Then we get this type
                    var type = objectParameter.ObjectType;

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
            Parameters.ForEach(Examine);

            // Return the resulting dictionary
            return dictionary;
        }

        #endregion
    }
}