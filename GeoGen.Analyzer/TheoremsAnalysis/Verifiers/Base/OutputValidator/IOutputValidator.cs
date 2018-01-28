using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a validator that says if a given verifier output should
    /// be converted into a theorem.
    /// </summary>
    internal interface IOutputValidator
    {
        /// <summary>
        /// Finds out if a given verifier output represents a true and acceptable
        /// theorem for a configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="output">The output.</param>
        /// <returns>true, if the output represents a correct acceptable theorem; false otherwise.</returns>
        bool Validate(Configuration configuration, VerifierOutput output);
    }
}