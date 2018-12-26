using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object of a <see cref="Configuration"/>. 
    /// </summary>
    public abstract class ConfigurationObject : IdentifiedObject
    {
        #region Public abstract properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public abstract ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Public abstract methods

        /// <summary>
        /// Enumerates the objects that are internally used to create this object. The order of this objects
        /// should match the order in which we can gradually construct them.
        /// </summary>
        /// <returns>A lazy enumerable of the internal objects.</returns>
        public abstract IEnumerable<ConfigurationObject> InternalObjects();

        #endregion

        #region To String

        /// <summary>
        /// Converts the object to a string. 
        /// NOTE: This method is not efficient should be used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString()
        {
            // First we need to get the object that this object is composed of
            var allObjects = InternalObjects().ToList();

            // Find the last letter
            var lastLetter = 'A' + allObjects.Count - 1;

            // Now we prepare the naming dictionary. We're goning to name all objects
            var namingDictionary = allObjects
                // They're ordered, we can give them the names, starting with A
                .Select((obj, index) => (configurationObject: obj, name: (char) (lastLetter - index)))
                // And wrap these names to a dictionary that we're going to use to convert each one to string
                .ToDictionary(pair => pair.configurationObject, pair => pair.name.ToString());

            // The output looks better reversed
            allObjects.Reverse();

            // Create the enumerable of object definitions, for example 'A=Point', or 'A=Incenter(B,C,D)'.
            var objectDefinitions = allObjects.Select(obj => $"{namingDictionary[obj]}={obj.ToString(namingDictionary)}");

            // Now we can construct the final string, for example 'Centroid(C,B,A) where C=Point, B=Point, A=Point'
            return $"{ToString(namingDictionary)}{(allObjects.Any() ? " where " : "")}{string.Join(", ", objectDefinitions)}";
        }

        /// <summary>
        /// Converts the object to a string using already set names of the objects.
        /// </summary>
        /// <param name="objectToStringMap"></param>
        /// <returns>A human-readable string representation of the object.</returns>
        protected abstract string ToString(IReadOnlyDictionary<ConfigurationObject, string> objectToStringMap);

        #endregion
    }
}