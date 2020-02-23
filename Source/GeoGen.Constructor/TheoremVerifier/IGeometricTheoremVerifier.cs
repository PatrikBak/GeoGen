using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that can verify a <see cref="Theorem"/> if it is true in all given <see cref="Pictures"/>.
    /// </summary>
    public interface IGeometricTheoremVerifier
    {
        /// <summary>
        /// Finds out if a given theorem is true in given pictures. It can also handle when the inner objects
        /// of the theorem are not yet constructed, in that case they get constructed, i.e. the pictures will be
        /// modified. If it's not wanted, the pictures have to be cloned before.
        /// </summary>
        /// <param name="pictures">The pictures where the theorem should be checked.</param>
        /// <param name="theorem">The theorem to be checked.</param>
        /// <returns>true, if the theorem can be constructed correctly and holds true in all the pictures; false otherwise.</returns>
        bool IsTrueInAllPictures(Pictures pictures, Theorem theorem);
    }
}
