using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> that finds theorems that states that some objects
    /// have exactly one intersection point.
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

        #region Protected overridden methods

        /// <inheritdoc/>
        protected override bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects)
        {
            // Prepare the variable that indicates whether the objects
            // have an intersection point that lies outside of some picture
            var isIntersectionPointExternal = false;

            // We want these objects to have an intersection that 
            // is not in the picture for every single picture
            foreach (var picture in contextualPicture.Pictures)
            {
                // For a given picture we take the objects
                var analyticObjects = objects.Select(o => contextualPicture.GetAnalyticObject(o, picture)).ToArray();

                // Intersect them
                var intersections = AnalyticHelpers.Intersect(analyticObjects);

                // Make sure there is exactly one intersection point
                if (intersections.Length != 1)
                    return false;

                // If it is not in the picture, mark it
                if (!picture.Contains(intersections[0]))
                    isIntersectionPointExternal = true;
            }

            // If we're expecting an intersection point  outside the picture,
            // then the theorem is fine if and only if there is any
            if (ExpectAnyExternalIntersection)
                return isIntersectionPointExternal;

            // Otherwise it's fine
            return true;
        }

        #endregion
    }
}