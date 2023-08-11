using GeoGen.Utilities;
using System.Collections;

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
        /// Gets the dictionary mapping configuration objects types to the number of 
        /// objects of that type that are needed to create corresponding <see cref="Arguments"/>.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectTypesToNeededCount { get; }

        /// <summary>
        /// Gets the list representing what types of objects should be pass to create 
        /// <see cref="Arguments"/> corresponding to this signature.
        /// </summary>
        public IReadOnlyList<ConfigurationObjectType> ObjectTypes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="parameters">The parameters that define the signature.</param>
        public Signature(IReadOnlyList<ConstructionParameter> parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            // Use the helper method to find object types represented by the parameters
            ObjectTypes = DetermineObjectTypes();

            // Derive the dictionary mapping types to their count from the objects types list
            ObjectTypesToNeededCount = ObjectTypes.GroupBy(type => type).ToDictionary(group => group.Key, group => group.Count());
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns if we the signature can be matched with the objects from given objects map. 
        /// In other words, if there are enough objects to be passed to the arguments.
        /// </summary>
        /// <param name="availableObjects">The available objects that we can use.</param>
        /// <returns>true, if the signature can be matched, false otherwise.</returns>
        public bool CanBeMatched(ConfigurationObjectMap availableObjects)
        {
            // Let's have a look at each pair of [ObjectType, NeededCount] to find out
            // if we have enough objects from this type
            foreach (var pair in ObjectTypesToNeededCount)
            {
                // Deconstruct
                var (type, neededCount) = pair;

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
        /// Constructs construction arguments that match the given construction parameters. 
        /// </summary>
        /// <param name="objects">The configuration objects used in the created arguments.</param>
        /// <returns>The created arguments matching the parameters.</returns>
        public Arguments Match(IReadOnlyList<ConfigurationObject> objects)
        {
            // Make sure the counts are fine
            if (objects.Count != ObjectTypes.Count)
                throw new GeoGenException($"The number of required objects {ObjectTypes.Count} is not equal to the number of passed objects {objects.Count}.");

            // Prepare the variable indicating the current index of the object being matched
            var index = 0;

            // Local function that creates an argument matching a given parameter
            ConstructionArgument CreateArgument(ConstructionParameter parameter)
            {
                // Switch based on the type
                switch (parameter)
                {
                    // If we have an object parameter
                    case ObjectConstructionParameter objectParameter:

                        // Get the object for comfort
                        var currentObject = objects[index];

                        // Make sure the type matches
                        if (currentObject.ObjectType != ObjectTypes[index])
                            throw new GeoGenException($"The object on the index {index} should have the type {ObjectTypes[index]}, but has {currentObject.ObjectType}.");

                        // Increase the index so that the next time we match the next object
                        index++;

                        // Return the object argument
                        return new ObjectConstructionArgument(currentObject);

                    // If we have a set parameter
                    case SetConstructionParameter setParameter:

                        // Create arguments set that we're going to fill
                        var arguments = new HashSet<ConstructionArgument>();

                        // For the expected number of times...
                        GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () =>
                        {
                            // Recursively call this function to obtain a new argument
                            var newArgument = CreateArgument(setParameter.TypeOfParameters);

                            // And update the arguments set
                            var hasBeenAdded = arguments.Add(newArgument);

                            // If it hasn't been added, i.e. there are duplicates, make aware
                            if (!hasBeenAdded)
                                throw new GeoGenException($"The object contains duplicate arguments.");
                        });

                        // Finally return the set construction argument wrapping the filled set
                        return new SetConstructionArgument(arguments);

                    // Unhandled cases
                    default:
                        throw new GeoGenException($"Unhandled type of {nameof(ConstructionParameter)}: {parameter.GetType()}");
                }
            }

            // Execute the create arguments function for particular parameters and wrap the result into Arguments
            return new Arguments(Parameters.Select(CreateArgument).ToList());
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds the list of object types where each type represents to the i-th object 
        /// that should be passed in order to match this signature.
        /// </summary>
        /// <returns>The list of object types of particular objects to be passed.</returns>
        private IReadOnlyList<ConfigurationObjectType> DetermineObjectTypes()
        {
            // Prepare a result
            var result = new List<ConfigurationObjectType>();

            // Local function that handles a single parameter
            void HandleParameter(ConstructionParameter parameter)
            {
                // Switch based on the type
                switch (parameter)
                {
                    // If we have an object parameter
                    case ObjectConstructionParameter objectParameter:

                        // We simply add its type
                        result.Add(objectParameter.ObjectType);

                        break;

                    // If we have a set parameter
                    case SetConstructionParameter setParameter:

                        // We recursively perform this function the needed number of times on the repeated parameters
                        GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () => HandleParameter(setParameter.TypeOfParameters));

                        break;

                    // Unhandled cases
                    default:
                        throw new GeoGenException($"Unhandled type of {nameof(ConstructionParameter)}: {parameter.GetType()}");
                }
            }

            // Handle every parameter
            Parameters.ForEach(HandleParameter);

            // Return the result
            return result;
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

        #region To String

        /// <inheritdoc/>
        public override string ToString() => Parameters.ToJoinedString(", ");

        #endregion
    }
}