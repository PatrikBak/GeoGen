using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a validator that says if a given <see cref="PotentialTheorem"/> 
    /// is valid theorem in a given <see cref="Configuration"/>. It is supposed to
    /// be used for the theorems that use objects from the coniguration.
    /// </summary>
    public interface IPotentialTheoremValidator
    {
        /// <summary>
        /// Finds out if a given potential theorem represents a real theorem
        /// that holds true in a given configuration. 
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if the theoem is right; false otherwise.</returns>
        bool Validate(Configuration configuration, PotentialTheorem theorem);
    }
}