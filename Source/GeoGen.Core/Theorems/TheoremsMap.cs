using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="TheoremType"/> to lists of <see cref="Theorem"/>
    /// of that type.
    /// </summary>
    public class TheoremsMap : ObjectsMap<TheoremType, Theorem>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsMap"/> class.
        /// </summary>
        /// <param name="theorems">The theorems to be contained in the map.</param>
        public TheoremsMap(IEnumerable<Theorem> theorems) : base(theorems)
        {
        }

        #endregion

        #region GetKey implementation

        /// <summary>
        /// Gets the key for a given value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The key.</returns>
        protected override TheoremType GetKey(Theorem value) => value.Type;

        #endregion
    }
}