using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that finds all theorems that are true in a configuration.
    /// </summary>
    public interface ICompleteTheoremsFinder
    {
        /// <summary>
        /// Finds all the theorems that are true in a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The theorems.</returns>
        List<Theorem> FindAllTheorems(Configuration configuration);
    }
}
