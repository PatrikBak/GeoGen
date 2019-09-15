using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents metadata of a derivation that uses <see cref="DerivationRule.DefinableSimpler"/>.
    /// </summary>
    public class DefinableSimplerDerivationData : TheoremDerivationData
    {
        #region Public properties

        /// <summary>
        /// The objects that are not needed to state the theorem.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> RedundantObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinableSimplerDerivationData"/> class.
        /// </summary>
        /// <param name="redundantObjects">The objects that are not needed to state the theorem.</param>
        public DefinableSimplerDerivationData(IReadOnlyList<ConfigurationObject> redundantObjects)
            : base(DerivationRule.DefinableSimpler)
        {
            RedundantObjects = redundantObjects ?? throw new ArgumentNullException(nameof(redundantObjects));
        }

        #endregion
    }
}