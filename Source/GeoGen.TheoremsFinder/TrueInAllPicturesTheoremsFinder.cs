using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a theorem finder that simply checks a theorem against every picture.
    /// </summary>
    public abstract class TrueInAllPicturesTheoremsFinder : AbstractTheoremsFinder
    {
        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected abstract bool IsTrue(IAnalyticObject[] objects);

        /// <summary>
        /// Finds out if given geometric objects represent a true theorem.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <param name="objects">The geometric objects that represent the theorem.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects)
        {
            // Return if the theorem is true in all pictures       
            return contextualPicture.Pictures.All(picture =>
                // To find out if it's true in the given one we use our abstract method
                IsTrue(objects.Select(geometricObject => contextualPicture.GetAnalyticObject(geometricObject, picture)).ToArray()));
        }
    }
}
