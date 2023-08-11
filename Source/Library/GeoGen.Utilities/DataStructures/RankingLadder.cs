using System.Collections;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a ladder of top <see cref="Capacity"/> items. It can handle when multiple items have
    /// the same rank (<see cref="Add(TItem, TRank)"/>. Items are ranked from the highest <see cref="TRank"/> value.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <typeparam name="TRank">The type of rank used in the items.</typeparam>
    public class RankingLadder<TItem, TRank> : IEnumerable<(TItem item, TRank rank)> where TRank : IComparable<TRank>
    {
        #region Private properties

        /// <summary>
        /// The content of the ladder, i.e. the sorted dictionary mapping rank onto items with this rank.
        /// They are naturally sorted from the smallest one.
        /// </summary>
        private readonly SortedDictionary<TRank, List<TItem>> _content = new SortedDictionary<TRank, List<TItem>>();

        #endregion

        #region Public properties

        /// <summary>
        /// The maximal number of elements of this ladder. 
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// The current number of items of the ladder.
        /// </summary>
        public int NumberOfItems { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingLadder{TItem, TRank}"/> class.
        /// </summary>
        /// <param name="capacity">The maximal number of elements of this ladder.</param>
        public RankingLadder(int capacity)
        {
            // Check capacity
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The value of {nameof(capacity)} should be at least 1.");

            Capacity = capacity;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a given item with a given rank to the ladder. If the rank of this item is at most the 
        /// smallest rank, then the item is not added. Otherwise the item is added and if the current number
        /// of items is now <see cref="Capacity"/> + 1, then one item with the smallest rank is removed. 
        /// If there are more items like that, then the oldest one is chosen. 
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="rank">The rank of the item to add.</param>
        /// <param name="removedItem">The item that got removed because the capacity has been reached.</param>
        public void Add(TItem item, TRank rank, out TItem removedItem)
        {
            // If there is still room for the item, add it
            if (NumberOfItems < Capacity)
            {
                // Add the item to the right list
                _content.GetValueOrCreateNewAddAndReturn(rank).Add(item);

                // Count it in
                NumberOfItems++;

                // No removed item
                removedItem = default;

                // We're done
                return;
            }

            // Otherwise we might want to remove the item
            // Get the currently smallest rank
            var smallestRank = _content.First().Key;

            // If this item doesn't have a higher rank, then we do nothing
            if (rank.CompareTo(smallestRank) <= 0)
            {
                // No removed item
                removedItem = default;

                // We're done
                return;
            }

            // Otherwise we add the item
            _content.GetValueOrCreateNewAddAndReturn(rank).Add(item);

            // And we need to remove one of them with the smallest rank
            // Get the list of all of such values
            var smallestRankedItems = _content.First().Value;

            // Set the removed item
            removedItem = smallestRankedItems[0];

            // Remove it
            smallestRankedItems.Remove(removedItem);

            // If this was the only one, remove the rank entirely
            if (smallestRankedItems.IsEmpty())
                _content.Remove(smallestRank);
        }

        /// <summary>
        /// Removes a given item with a given rank from the ladder.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <param name="rank">The rank of the item.</param>
        public void Remove(TItem item, TRank rank)
        {
            // Find the right list by the rank
            var itemsWithThisRank = _content[rank];

            // Remove the item from it
            itemsWithThisRank.Remove(item);

            // If the list is empty, remove the rank entirely
            if (itemsWithThisRank.IsEmpty())
                _content.Remove(rank);
        }

        /// <summary>
        /// Finds out if placing of an item with a passed rank would change the ladder.
        /// </summary>
        /// <param name="rank">The rank to be checked.</param>
        /// <returns>true, if an item with the passed rank would be acceptable; false otherwise.</returns>
        public bool IsTherePlaceFor(TRank rank)
            // Either the capacity isn't full yet or it has a higher rank than the smallest one
            => NumberOfItems < Capacity || _content.First().Key.CompareTo(rank) < 0;

        #endregion

        #region IEnumerable implementation

        /// <inheritdoc/>
        public IEnumerator<(TItem item, TRank rank)> GetEnumerator() =>
            // The items are sorted in the descending order, i.e. we want to reverse them in the end
            _content.SelectMany(pair => pair.Value.Select(rank => (rank, pair.Key))).Reverse().GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}