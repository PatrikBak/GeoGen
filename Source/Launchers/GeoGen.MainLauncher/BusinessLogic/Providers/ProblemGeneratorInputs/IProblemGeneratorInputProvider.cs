using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="LoadedProblemGeneratorInput"/>s.
    /// </summary>
    public interface IProblemGeneratorInputProvider
    {
        /// <summary>
        /// Gets problem generator inputs.
        /// </summary>
        /// <returns>The loaded problem generator inputs.</returns>
        Task<IReadOnlyList<LoadedProblemGeneratorInput>> GetProblemGeneratorInputsAsync();
    }
}