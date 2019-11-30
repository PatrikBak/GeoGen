using GeoGen.Core;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// Represents the service that perform simplification of a theorem holding true in a configuration.
    /// </summary>
    public interface ITheoremSimplifier
    {
        /// <summary>
        /// Tries to simplify a given theorem together with the configuration where it holds true.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="theorem">The theorem to be simplified.</param>
        /// <param name="allTheorems">The map of all theorems holding in the configuration.</param>
        /// <returns>Either the tuple of simplified configuration and theorem; or null, if the algorithm couldn't do it.</returns>
        (Configuration newConfiguration, Theorem newTheorem)? Simplify(Configuration configuration, Theorem theorem, TheoremMap allTheorems);
    }
}