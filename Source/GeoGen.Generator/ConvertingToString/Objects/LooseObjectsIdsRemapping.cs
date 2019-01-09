using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a remapping of <see cref="LooseConfigurationObject"/>s to their new ids. In other words,
    /// it is a wrapper for a dictionary mapping each loose objects to an id distinct from the object's id. 
    /// If an object is not present in the dictionary, then it means its id should not be remapped. For no 
    /// remapping the constant value <see cref="NoRemapping"/> should be used.
    /// </summary>
    public class LooseObjectIdsRemapping
    {
        #region Public static constants

        /// <summary>
        /// The empty remapping, i.e. no object has its id remapped.
        /// </summary>
        public static readonly LooseObjectIdsRemapping NoRemapping = new LooseObjectIdsRemapping(new Dictionary<LooseConfigurationObject, int>());

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping each loose object to an id distinct from the object's id.
        /// </summary>
        private readonly IReadOnlyDictionary<LooseConfigurationObject, int> _remappedIds;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseObjectIdsRemapping"/> class.
        /// </summary>
        /// <param name="remappedIds">The dictionary mapping each loose object to an id distinct from the object's id.</param>
        public LooseObjectIdsRemapping(IReadOnlyDictionary<LooseConfigurationObject, int> remappedIds)
        {
            _remappedIds = remappedIds ?? throw new ArgumentNullException(nameof(remappedIds));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds the id to which a given loose object should be mapped to.
        /// </summary>
        /// <param name="looseObject">The loose object.</param>
        /// <returns>The remapped id, or the current id, if the object shouldn't be remapped.</returns>
        public int ResolveId(LooseConfigurationObject looseObject)
        {
            // If this object should be remapped...
            return _remappedIds.ContainsKey(looseObject) ?
                // Then we find the id to which it should be remapped
                _remappedIds[looseObject] :
                // Otherwise we take the object's id.
                looseObject.Id;
        }

        #endregion
    }
}