using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> that finds theorems that are intersecting objects.
    /// </summary>
    public abstract class IntersectingTheoremFinder : AbstractTheoremFinder
    {
        #region Protected abstract properties

        /// <summary>
        /// Indicates whether we want intersected objects to have at least one 
        /// intersection that lies outside of the picture.
        /// </summary>
        protected abstract bool ExpectAnyExternalIntersection { get; }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Returns if a given number intersections is allowed for this type of theorem.
        /// </summary>
        /// <param name="numberOfIntersections">The number of intersections to be questioned.</param>
        /// <returns>true, if this number is allowed; false otherwise.</returns>
        protected abstract bool IsNumberOfIntersectionsAllowed(int numberOfIntersections);

        #endregion

        #region Protected overridden methods

        /// <summary>
        /// Finds out if given geometric objects represent a true theorem.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <param name="objects">The geometric objects that represent the theorem.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects)
        {
            // Prepare the variable that indicates whether the objects
            // have an intersection that lies outside of some picture
            var anyExternalIntersection = false;

            // We want these objects to have an intersection that 
            // is not in the picture for every single picture
            foreach (var picture in contextualPicture.Pictures)
            {
                // For a given picture we take the objects
                var analyticObjects = objects.Select(o => contextualPicture.GetAnalyticObject(o, picture)).ToArray();

                // Intersect them
                var intersections = AnalyticHelpers.Intersect(analyticObjects);

                // Validate the number of intersections 
                if (!IsNumberOfIntersectionsAllowed(intersections.Length))
                    return false;

                // Mark if there is any intersection that is not in the picture
                if (intersections.Any(point => !picture.Contains(point)))
                    anyExternalIntersection = true;
            }

            // If we're expecting an intersecting outside the picture,
            // then the theorem is fine if and only if there is any
            if (ExpectAnyExternalIntersection)
                return anyExternalIntersection;

            // Otherwise it's fine
            return true;
        }

        #endregion
    }
}