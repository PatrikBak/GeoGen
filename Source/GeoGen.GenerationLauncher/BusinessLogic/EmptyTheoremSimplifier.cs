using GeoGen.Core;
using GeoGen.TheoremSimplifier;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// The implementation of <see cref="ITheoremSimplifier"/> that returns null.
    /// </summary>
    public class EmptyTheoremSimplifier : ITheoremSimplifier
    {
        /// <summary>
        /// Tries to simplify a given theorem together with the configuration where it holds true.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="theorem">The theorem to be simplified.</param>
        /// <param name="allTheorems">The map of all theorems holding in the configuration.</param>
        /// <returns>Either the tuple of simplified configuration and theorem; or null, if the algorithm couldn't do it.</returns>
        public (Configuration newConfiguration, Theorem newTheorem)? Simplify(Configuration configuration, Theorem theorem, TheoremMap allTheorems) => null;
    }
}
