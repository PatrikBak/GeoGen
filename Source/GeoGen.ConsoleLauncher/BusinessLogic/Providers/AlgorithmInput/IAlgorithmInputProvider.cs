using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="LoadedAlgorithmInput"/>s.
    /// </summary>
    public interface IAlgorithmInputProvider
    {
        /// <summary>
        /// Gets algorithm inputs.
        /// </summary>
        /// <returns>The algorithm inputs.</returns>
        Task<IReadOnlyList<LoadedAlgorithmInput>> GetAlgorithmInputsAsync();
    }
}
