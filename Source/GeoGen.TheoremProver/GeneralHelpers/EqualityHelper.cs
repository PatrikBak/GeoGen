using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a helper class that can keeps track of equal objects and automatically infers other ones
    /// using transitivity of equality. This is done by keeping <see cref="EqualityGroups"/>.
    /// </summary>
    public class EqualityHelper
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects to the sets of objects equal to them.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, HashSet<ConfigurationObject>> _equalObjects = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();

        #endregion

        #region Public properties

        /// <summary>
        /// The enumerable of equality groups (two objects in the same group are equal).
        /// </summary>
        public IEnumerable<IReadOnlyCollection<ConfigurationObject>> EqualityGroups => _equalObjects.Values.Distinct();

        /// <summary>
        /// The enumerable of all objects tracked by the helper. 
        /// </summary>
        public IEnumerable<ConfigurationObject> AllObjects => _equalObjects.Keys;

        #endregion

        #region Public methods

        /// <summary>
        /// Marks two given objects as equal ones.
        /// </summary>
        /// <param name="equalObjects">The pair of equal objects.</param>
        public void MarkEqualObjects((ConfigurationObject, ConfigurationObject) equalObjects)
            // Delegate the call to the most general method 
            => MarkEqualObjects(equalObjects, out var _, shouldWeFindInferredEqualities: false, out var _);

        /// <summary>
        /// Marks two given objects as equal ones while returning whether this was a knew previously unknown equality.
        /// </summary>
        /// <param name="equalObjects">The pair of equal objects.</param>
        /// <param name="isThisEqualityNew">Indicates whether this equality was known before.</param>
        public void MarkEqualObjects((ConfigurationObject, ConfigurationObject) equalObjects, out bool isThisEqualityNew)
            // Delegate the call to the most general method 
            => MarkEqualObjects(equalObjects, out isThisEqualityNew, shouldWeFindInferredEqualities: false, out var _);

        /// <summary>
        /// Marks two given objects as equal ones and returns equalities that can be inferred based on transitivity.
        /// </summary>
        /// <param name="equalObjects">The pair of equal objects.</param>
        /// <param name="inferredEqualities">The array of new equalities that can be inferred via transitivity.</param>
        public void MarkEqualObjects((ConfigurationObject, ConfigurationObject) equalObjects, out (Theorem inferredEquality, Theorem[] usedEqualities)[] inferredEqualities)
            // Delegate the call to the most general method 
            => MarkEqualObjects(equalObjects, out var _, shouldWeFindInferredEqualities: true, out inferredEqualities);

        /// <summary>
        /// Finds the objects equal to a passed one, including the passed one.
        /// </summary>
        /// <param name="configurationObject">An object for whose equal objects we're querying.</param>
        /// <returns>An enumerable of objects equal to the passed one, including the passed one.</returns>
        public IEnumerable<ConfigurationObject> GetEqualObjects(ConfigurationObject configurationObject)
            // They should be in the dictionary, or if not, we only have this object
            => _equalObjects.GetValueOrDefault(configurationObject) ?? configurationObject.ToEnumerable();

        /// <summary>
        /// Finds out if given two objects are equal according to the already marked equalities.
        /// </summary>
        /// <param name="object1">The first object.</param>
        /// <param name="object2">The second object.</param>
        /// <returns>true, if the objects are equal; false otherwise.</returns>
        public bool AreEqual(ConfigurationObject object1, ConfigurationObject object2)
            // Get objects equal to the first one and see if the second one is among them
            => GetEqualObjects(object1).Contains(object2);

        #endregion

        #region Private methods

        /// <summary>
        /// Marks two given objects as equal ones.
        /// </summary>
        /// <param name="equalObjects">The pair of equal objects.</param>
        /// <param name="isThisEqualityNew">Indicates whether this equality was known before.</param>
        /// <param name="shouldWeFindInferredEqualities">Indicates whether we should find and set the <paramref name="inferredEqualities"/>.</param>
        /// <param name="inferredEqualities">The array of new equalities that can be inferred via transitivity.</param>
        private void MarkEqualObjects((ConfigurationObject, ConfigurationObject) equalObjects,
                                      out bool isThisEqualityNew,
                                      bool shouldWeFindInferredEqualities,
                                      out (Theorem inferredEquality, Theorem[] usedEqualities)[] inferredEqualities)
        {
            // Deconstruct
            var (object1, object2) = equalObjects;

            // Find the equality groups for the objects
            var group1 = _equalObjects.GetValueOrDefault(object1) ?? new HashSet<ConfigurationObject> { object1 };
            var group2 = _equalObjects.GetValueOrDefault(object2) ?? new HashSet<ConfigurationObject> { object2 };

            // If they are the same, we won't do more
            if (group1 == group2)
            {
                // This is not a new equality
                isThisEqualityNew = false;

                // No inferred equalities
                inferredEqualities = Array.Empty<(Theorem, Theorem[])>();

                // We're done
                return;
            }

            // If we got here, then we have different groups, i.e. this is a new equality
            isThisEqualityNew = true;

            // Before handling groups we could use what equalities their merge will bring
            // Any object from the first group will now be equal to any object from the second one
            var groupEqualities = group1.CombinedWith(group2).ToArray();

            // We're to add everything to the first group. If it's new, make sure it's in the dictionary
            if (!_equalObjects.ContainsKey(object1))
                _equalObjects.Add(object1, group1);

            // Make sure the objects from the second group have now the first one as their equality group
            group2.ForEach(secondGroupObject => _equalObjects[secondGroupObject] = group1);

            // Make sure the objects from the second group are merged to the first one
            group1.UnionWith(group2);

            #region Finding inferred equalities

            // If we shouldn't find inferred equalities...
            if (!shouldWeFindInferredEqualities)
            {
                // Then we won't
                inferredEqualities = null;

                // And we're done
                return;
            }

            // Otherwise prepare a list of there inferred equalities
            var inferredEqualitiesList = new List<(Theorem inferredEquality, Theorem[] usedEqualities)>();

            // Any object from the first group is now equal to any object of the second group
            foreach (var (newEqualityObject1, newEqualityObject2) in groupEqualities)
            {
                // If this is our equality, we can move on
                if (new[] { newEqualityObject1, newEqualityObject2 }.OrderlessEquals(new[] { object1, object2 }))
                    continue;

                // Otherwise we have a new inferred equality
                var inferredEquality = new Theorem(EqualObjects, newEqualityObject1, newEqualityObject2);

                // New need to create the used equalities
                var usedEqualities = new List<Theorem>
                { 
                    // One of them certainly is the one we used to find equality of the current groups
                    new Theorem(EqualObjects, object1, object2)
                };

                // If there was a non-trivial equality within the first group, add it
                if (!object1.Equals(newEqualityObject1))
                    usedEqualities.Add(new Theorem(EqualObjects, object1, newEqualityObject1));

                // If there was a non-trivial equality within the second group, add it
                if (!object2.Equals(newEqualityObject2))
                    usedEqualities.Add(new Theorem(EqualObjects, object2, newEqualityObject2));

                // Finally mark the inferred equality
                inferredEqualitiesList.Add((inferredEquality, usedEqualities.ToArray()));
            }

            // Set the inferred equalities
            inferredEqualities = inferredEqualitiesList.ToArray();

            #endregion
        }

        #endregion
    }
}