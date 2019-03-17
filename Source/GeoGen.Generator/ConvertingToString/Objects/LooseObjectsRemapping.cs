using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a remapping of <see cref="LooseConfigurationObject"/>s to some other ones. In other words,
    /// it is a wrapper for a dictionary mapping each loose objects to other loose objects. If an object is
    /// not present in the dictionary, then it means it should not be remapped. For no remapping the 
    /// constant value <see cref="NoRemapping"/> should be used, because of caching. Two new instances of an
    /// empty dictionary might have distinct hash codes (and usually have).
    /// </summary>
    public class LooseObjectsRemapping
    {
        #region Public static constants

        /// <summary>
        /// The empty remapping, i.e. no object is remapped.
        /// </summary>
        public static readonly LooseObjectsRemapping NoRemapping = new LooseObjectsRemapping(new Dictionary<LooseConfigurationObject, LooseConfigurationObject>());

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary representing the remapping.
        /// </summary>
        private readonly IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> _remappedObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectsRemapping"/> class.
        /// </summary>
        /// <param name="remappedObjects">The dictionary mapping each loose object to some other loose object.</param>
        public LooseObjectsRemapping(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> remappedObjects)
        {
            _remappedObjects = remappedObjects ?? throw new ArgumentNullException(nameof(remappedObjects));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Maps a given loose object with respect to this remapping. If the object shouldn't
        /// be remapped, the method will return the passed one.
        /// </summary>
        /// <param name="looseObject">The loose object to be mapped.</param>
        /// <returns>The remapped object, if there is any remapping defined; otherwise the passed object.</returns>
        public LooseConfigurationObject Map(LooseConfigurationObject looseObject)
        {
            // If this object should be remapped...
            return _remappedObjects.ContainsKey(looseObject) ?
                // Then we return to which object
                _remappedObjects[looseObject] :
                // Otherwise we return the same object
                looseObject;
        }

        #endregion
    }
}