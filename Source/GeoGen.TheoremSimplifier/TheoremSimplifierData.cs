using System;
using System.Collections.Generic;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// The data used by <see cref="TheoremSimplifier"/> containing a set of <see cref="SimplificationRule"/>s.
    /// </summary>
    public class TheoremSimplifierData
    {
        #region Public properties

        /// <summary>
        /// The simplification rules used by the algorithm.
        /// </summary>
        public IReadOnlyCollection<SimplificationRule> Rules { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifierData"/> class.
        /// </summary>
        /// <param name="rules">The simplification rules used by the algorithm..</param>
        public TheoremSimplifierData(IReadOnlyCollection<SimplificationRule> rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        #endregion
    }
}