using GeoGen.Utilities;
using System;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// The data used by <see cref="TheoremSimplifier"/> containing a set of <see cref="SimplificationRule"/>s.
    /// </summary>
    public class TheoremSimplifierData
    {
        #region Public properties

        /// <summary>
        /// The set of simplification rules used by the algorithm.
        /// </summary>
        public IReadOnlyHashSet<SimplificationRule> Rules { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifierData"/> class.
        /// </summary>
        /// <param name="rules">The set of simplification rules used by the algorithm.</param>
        public TheoremSimplifierData(IReadOnlyHashSet<SimplificationRule> rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        #endregion
    }
}