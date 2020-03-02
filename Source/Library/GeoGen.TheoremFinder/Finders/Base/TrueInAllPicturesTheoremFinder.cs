using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// Represents a theorem finder that simply checks a theorem against every picture.
    /// </summary>
    public abstract class TrueInAllPicturesTheoremFinder : AbstractTheoremFinder
    {
        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected abstract bool IsTrue(IAnalyticObject[] objects);

        /// <inheritdoc/>
        protected override bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects)
            // Return if the theorem is true in all pictures       
            => contextualPicture.Pictures.All(picture =>
                // To find out if it's true in the given one we use our abstract method
                IsTrue(objects.Select(geometricObject => contextualPicture.GetAnalyticObject(geometricObject, picture)).ToArray()));
    }
}
