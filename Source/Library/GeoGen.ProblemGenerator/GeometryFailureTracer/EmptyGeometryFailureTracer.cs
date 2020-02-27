using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// The implementation of <see cref="IGeometryFailureTracer"/> that does nothing.
    /// </summary>
    public class EmptyGeometryFailureTracer : IGeometryFailureTracer
    {
        /// <summary>
        /// Traces that given pictures couldn't be cloned and extended with the new object
        /// already drawn in pictures representing some configuration.
        /// </summary>
        /// <param name="previousPictures">The pictures that were correct and failed to add the new object.</param>
        /// <param name="newConfiguration">The new configuration that was attempted to be drawn.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void InconstructiblePicturesByCloning(PicturesOfConfiguration previousPictures, Configuration newConfiguration, InconsistentPicturesException exception)
        {
        }

        /// <summary>
        /// Traces that a given contextual picture couldn't be cloned and extended with the new object
        /// already drawn in pictures representing some configuration.
        /// </summary>
        /// <param name="previousContextualPicture">The contextual picture that was correct and failed to add the new object.</param>
        /// <param name="newConfigurationPictures">The pictures holding geometry data of the new object that was added.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void InconstructibleContextualPictureByCloning(ContextualPicture previousContextualPicture, PicturesOfConfiguration newConfigurationPictures, InconsistentPicturesException exception)
        {
        }
    }
}
