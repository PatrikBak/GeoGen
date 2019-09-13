using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that is able to derive new theorems in 
    /// a configuration from easy ones (so-called sub-theorems).
    /// </summary>
    public interface ISubtheoremsDeriver
    {
        /// <summary>
        /// Performs the sub-theorem derivation on a given input.
        /// </summary>
        /// <param name="input">The input for the deriver.</param>
        /// <returns>The derived theorems wrapped in output objects.</returns>
        IEnumerable<SubtheoremsDeriverOutput> DeriveTheorems(SubtheoremsDeriverInput input);
    }
}