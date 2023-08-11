using GeoGen.Utilities;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a dictionary mapping <see cref="TheoremType"/> to lists of <see cref="Theorem"/>
    /// of that type.
    /// </summary>
    public class TheoremMap : ObjectMap<TheoremType, Theorem>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremMap"/> class.
        /// </summary>
        /// <param name="theorems">The theorems to be contained in the map.</param>
        public TheoremMap(IEnumerable<Theorem> theorems) : base(theorems)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="TheoremMap"/>.
        /// </summary>
        public TheoremMap() : base(Enumerable.Empty<Theorem>())
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds out if the map contains a given theorem.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if the given theorem is contained in the map; false otherwise.</returns>
        public bool ContainsTheorem(Theorem theorem) => this.GetValueOrDefault(theorem.Type)?.Contains(theorem) ?? false;

        #endregion

        #region GetKey implementation

        /// <inheritdoc/>
        protected override TheoremType GetKey(Theorem value) => value.Type;

        #endregion
    }
}