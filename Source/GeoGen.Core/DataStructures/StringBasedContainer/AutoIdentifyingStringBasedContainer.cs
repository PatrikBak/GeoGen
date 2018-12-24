using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="StringBasedContainer{T}"/> that is able to automatically identify the passed
    /// objects with ids. This identification happens right away the item is added, regardless of the 
    /// presence of an equal version.
    /// </summary>
    /// <typeparam name="T">The type of the identified object in the container.</typeparam>
    public class AutoIdentifyingStringBasedContainer<T> : StringBasedContainer<T> where T : IdentifiedObject
    {
        #region Protected properties

        /// <summary>
        /// The id prepare for the next object.
        /// </summary>
        protected int _nextObjectId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBasedContainer{T}"/> class.
        /// </summary>
        /// <param name="converter">The converter of items to string.</param>
        /// <param name="initialItems">The initial items that should be added to the container.</param>
        public AutoIdentifyingStringBasedContainer(IToStringConverter<T> converter, IEnumerable<T> initialItems = null)
            : base(converter, initialItems)
        {
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// Adds a given item to the container and identifies it with next available id.
        /// If an equal version of the item is present in the container, the item won't be
        /// added and the <paramref name="equalItem"/> will be set to this equal version. 
        /// Otherwise the <paramref name="equalItem"/> will be set to the default value 
        /// of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="equalItem">Either the equal version of the passed item from the container (if there's any), or the default value of the type <typeparamref name="T"/>.</param>
        public override void Add(T item, out T equalItem)
        {
            // Identify the item
            item.Id = _nextObjectId++;

            // Call the base method
            base.Add(item, out equalItem);
        }

        #endregion
    }
}
