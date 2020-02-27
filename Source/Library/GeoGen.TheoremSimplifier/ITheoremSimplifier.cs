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
        /// <param name="theorem">The theorem to be simplified.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <returns>Either the tuple of simplified theorem and configuration; or null, if the algorithm couldn't do it.</returns>
        (Theorem newTheorem, Configuration newConfiguration)? Simplify(Theorem theorem, Configuration configuration);
    }
}