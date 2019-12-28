using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a tracer of geometry failures caused by <see cref="InconsistentPicturesException"/>s
    /// during a run of <see cref="SubtheoremDeriver"/>.
    /// </summary>
    public interface ISubtheoremDeriverGeometryFailureTracer
    {
        /// <summary>
        /// Traces that drawing of an object using the input from given pictures couldn't be performed consistently.
        /// </summary>
        /// <param name="constructedObject">The object that couldn't be drawn consistently.</param>
        /// <param name="pictures">The pictures from which the input for the construction was taken.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        void UndrawableObject(ConstructedConfigurationObject constructedObject, PicturesOfConfiguration pictures, InconsistentPicturesException exception);

        /// <summary>
        /// Traces that an examination via <see cref="ContextualPicture.GetGeometricObject(IReadOnlyDictionary{Picture, AnalyticGeometry.IAnalyticObject})"/>
        /// couldn't be performed due to an inconsistency.
        /// </summary>
        /// <param name="constructedObject">The object that couldn't be examined consistently.</param>
        /// <param name="contextualPicture">The contextual picture where that was used to perform the examination.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        void UnexaminableObjectInContextualPicture(ConstructedConfigurationObject constructedObject, ContextualPicture contextualPicture, InconsistentPicturesException exception);
    }
}