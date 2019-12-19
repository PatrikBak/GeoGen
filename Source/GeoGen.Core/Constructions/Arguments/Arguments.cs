using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a list of <see cref="ConstructionArgument"/> that is directly 
    /// part of the definition of <see cref="ConstructedConfigurationObject"/>s. 
    /// 
    /// FEATURE: I think the inner objects don't have to distinct, only within sets.
    /// 
    /// </summary>
    public class Arguments : IEnumerable<ConstructionArgument>
    {
        #region Public properties

        /// <summary>
        /// Gets the list of individual arguments.
        /// </summary>
        public IReadOnlyList<ConstructionArgument> ArgumentsList { get; }

        /// <summary>
        /// Gets the list of configuration objects that are obtained within the arguments
        /// in the order that we get if we recursively search through them from left to right. 
        /// For example: With { {A,B}, {C,D} } we might get A,B,C,D; or D,C,B,A. The order of objects
        /// within a set itself is not deterministic.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> FlattenedList { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> wrapping a given arguments list.
        /// </summary>
        /// <param name="argumentsList">The list of individual arguments.</param>
        public Arguments(IReadOnlyList<ConstructionArgument> argumentsList)
        {
            ArgumentsList = argumentsList ?? throw new ArgumentNullException(nameof(argumentsList));
            FlattenedList = ExtraxtInputObject();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Finds all the objects in the arguments and flattens them to a list.
        /// </summary>
        /// <returns>A list containing the flattened objects.</returns>
        private List<ConfigurationObject> ExtraxtInputObject()
        {
            // Prepare the result
            var result = new List<ConfigurationObject>();

            // Local function to extract objects from an argument
            void Extract(ConstructionArgument argument)
            {
                // Switch based on type
                switch (argument)
                {
                    // If we have an object argument
                    case ObjectConstructionArgument objectArgument:

                        // We simply add the internal object to the result
                        result.Add(objectArgument.PassedObject);

                        break;

                    // If we have a set argument
                    case SetConstructionArgument setArgument:

                        // We recursively call this function for its internal arguments
                        setArgument.PassedArguments.ForEach(Extract);

                        break;

                    // Default case
                    default:
                        throw new GeoGenException($"Unhandled type of construction argument: {argument.GetType()}");
                }
            }

            // Now we just call our local function for all the arguments
            ArgumentsList.ForEach(Extract);

            // And return the result
            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Recreates the arguments using a given mapping of loose objects.
        /// </summary>
        /// <param name="mapping">The mapping of the loose objects.</param>
        /// <returns>The remapped arguments.</returns>
        public Arguments Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
        {
            // Remap individual arguments using their remap method 
            return new Arguments(ArgumentsList.Select(argument => argument.Remap(mapping)).ToList());
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<ConstructionArgument> GetEnumerator() => ArgumentsList.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => ArgumentsList.GetHashCodeOfList();

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is an Arguments object
                && otherObject is Arguments arguments
                // And the arguments lists
                && arguments.ArgumentsList.SequenceEqual(ArgumentsList);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the arguments to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => ArgumentsList.ToJoinedString();

#endif

        #endregion
    }
}
