using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a signature of a <see cref="Construction"/>, i.e. the way <see cref="ConfigurationObject"/>s
    /// should be composed into <see cref="ConstructionArgument"/> to represents an input of the construction.
    /// It is defined as a list of <see cref="ConstructionParameter"/>.
    /// </summary>
    public class Signature : IEnumerable<ConstructionParameter>
    {
        #region Public properties

        /// <summary>
        /// Gets the list of construction parameters that represent this signature.
        /// </summary>
        public IReadOnlyList<ConstructionParameter> Parameters { get; }

        /// <summary>
        /// Gets or sets the dictionary mapping configuration objects types to the number of 
        /// objects of that type that are needed to create corresponding <see cref="Arguments"/>.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="parameters">The parameters that define the signature.</param>
        public Signature(IReadOnlyList<ConstructionParameter> parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            ObjectTypesToNeededCount = DetermineObjectTypesToCount();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns if we the signature can be matched with the objects from given objects map. 
        /// In other words, if there are enough objects to be passed to the arguments.
        /// </summary>
        /// <param name="availableObjects">The available objects that we can use.</param>
        /// <returns>true, if the signature can be matched, false otherwise.</returns>
        public bool CanBeMatched(ConfigurationObjectsMap availableObjects)
        {
            // Let's have a look at each pair of [ObjectType, NeededCount] to find out
            // if we have enough objects from this type
            foreach (var pair in ObjectTypesToNeededCount)
            {
                // Pull the objects type
                var type = pair.Key;

                // Pull the needed count
                var neededCount = pair.Value;

                // If there is no object of the type, we certainly can't match the signature
                if (!availableObjects.ContainsKey(type))
                    return false;

                // If there are more needed arguments than available objects, 
                // then we can't match the signature either
                if (neededCount > availableObjects[type].Count)
                    return false;
            }

            // If we got here, we can match it
            return true;
        }

        /// <summary>
        /// Constructs construction arguments that match the given construction 
        /// parameters. The objects that are actually passed to the arguments 
        /// are given in a configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters to be matched.</param>
        /// <param name="objectsMap">The configuration objects used in the created arguments.</param>
        /// <returns>The created arguments matching the parameters.</returns>
        public Arguments Match(ConfigurationObjectsMap objectsMap)
        {
            // Create a dictionary mapping object types to the current index of the object of that type
            // that is ready to be passed to the next object argument. Initially, these indices are 0.
            var indices = objectsMap.ToDictionary(keyValue => keyValue.Key, keyValue => 0);

            // Local function that creates an argument matching a given parameter
            ConstructionArgument CreateArgument(ConstructionParameter parameter)
            {
                // If the parameter is an object parameter...
                if (parameter is ObjectConstructionParameter objectParameter)
                {
                    // Then we simply ask for the next object of the expected type
                    // and increase the index for the next expected object of this type
                    var nextObject = objectsMap[objectParameter.ObjectType][indices[objectParameter.ObjectType]++];

                    // And return the object argument wrapping this object
                    return new ObjectConstructionArgument(nextObject);
                }

                // Otherwise we have a set construction parameter
                var setParameter = (SetConstructionParameter) parameter;

                // Create arguments list that we're going to fill
                var arguments = new List<ConstructionArgument>();

                // For the expected number of times...
                GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () =>
                {
                    // Recursively call this function to obtain a new argument
                    var newArgument = CreateArgument(setParameter.TypeOfParameters);

                    // And update the arguments list
                    arguments.Add(newArgument);
                });

                // Finally return the set construction argument wrapping the filled list
                return new SetConstructionArgument(arguments);
            }

            // Execute the create arguments function for particular parameters and wrap the result into Arguments
            return new Arguments(Parameters.Select(CreateArgument).ToList());
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<ConstructionParameter> GetEnumerator() => Parameters.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        #region To String

        /// <summary>
        /// Converts a given signature to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the signature.</returns>
        public override string ToString() => ToStringHelper.SignatureToString(this);

        #endregion
    }
}