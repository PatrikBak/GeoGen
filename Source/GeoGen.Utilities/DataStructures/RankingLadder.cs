using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a ladder of top <see cref="Capacity"/> items. It can handle when multiple items have
    /// the same rank (<see cref="Add(TItem, TRank)"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <typeparam name="TRank">The type of rank used in the items.</typeparam>
    public class RankingLadder<TItem, TRank> : IEnumerable<(TItem item, TRank rank)> where TRank : IComparable<TRank>
    {
        #region Private properties

        /// <summary>
        /// The content of the ladder, i.e. the sorted dictionary mapping rank onto items with this rank.
        /// </summary>
        private readonly SortedDictionary<TRank, List<TItem>> _content = new SortedDictionary<TRank, List<TItem>>();

        #endregion

        #region Public properties

        /// <summary>
        /// The maximal number of elements of this ladder. If it's <see cref="int.MaxValue"/>, then there are no limits.
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
        /// <param name="capacity">The maximal number of elements of this ladder. If it's <see cref="int.MaxValue"/>, then there are no limits.</param>
        public RankingLadder(int capacity)
        {
            // Check capacity
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The value of {nameof(capacity)} should be at least 1.");

            Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingLadder{TItem, TRank}"/> class
        /// with no capacity limits.
        /// </summary>
        public RankingLadder()
            : this(int.MaxValue)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a given item with a given rank to the ladder. If the rank of this item is at most the 
        /// smallest rank, then the item is not added. Otherwise the item is added and if the current number
        /// of items is now <see cref="Capacity"/> + 1, then one item with the smallest rank is removed. 
        /// If there are more items like that, then the oldest one is chosen. 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="contentChanged">Indicates whether the content of the ladder has been changed.</param>
        public void Add(TItem item, TRank rank, out bool contentChanged)
        {
            // If there is still room for the item, add it
            if (NumberOfItems < Capacity)
            {
                // Add the item to the right list
                _content.GetValueOrCreateNewAddAndReturn(rank).Add(item);

                // Count it in
                NumberOfItems++;

                // Set the content changed value
                contentChanged = true;

                // We're done
                return;
            }

            // Otherwise we might want to remove the item
            // Get the currently smallest rank
            var smallestRank = _content.First().Key;

            // If this item doesn't have a higher rank, then we do nothing
            if (rank.CompareTo(smallestRank) <= 0)
            {
                // Set the content changed value
                contentChanged = false;

                // We're done
                return;
            }

            // Otherwise we add the item
            _content.GetValueOrCreateNewAddAndReturn(rank).Add(item);

            // And we need to remove one of them with the smallest rank
            // Get the list of all of them
            var smallestRankedItems = _content.First().Value;

            // Remove the first
            smallestRankedItems.RemoveAt(0);

            // If this was the only one, remove the rank
            if (smallestRankedItems.IsEmpty())
                _content.Remove(smallestRank);

            // Set the content changed value
            contentChanged = true;
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<(TItem item, TRank rank)> GetEnumerator() =>
            // The items are sorted in the descending order, i.e. we want to reverse them in the end
            _content.SelectMany(pair => pair.Value.Select(rank => (rank, pair.Key))).Reverse().GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}