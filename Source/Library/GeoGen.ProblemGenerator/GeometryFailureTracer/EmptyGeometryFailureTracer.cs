using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// The implementation of <see cref="IGeometryFailureTracer"/> that does nothing.
    /// </summary>
    public class EmptyGeometryFailureTracer : IGeometryFailureTracer
    {
        /// <inheritdoc/>
        public void InconstructiblePicturesByCloning(PicturesOfConfiguration previousPictures, Configuration newConfiguration, InconsistentPicturesException exception)
        {
        }

        /// <inheritdoc/>
        public void InconstructibleContextualPictureByCloning(ContextualPicture previousContextualPicture, PicturesOfConfiguration newConfigurationPictures, InconsistentPicturesException exception)
        {
        }
    }
}
