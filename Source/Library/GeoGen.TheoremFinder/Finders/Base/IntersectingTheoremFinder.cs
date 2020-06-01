using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
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

        #region Public overridden methods

        /// <inheritdoc/>
        public override bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem)
        {
            // If we don't care whether the intersection point is outside or inside of the picture,
            // then there is no reason to say a theorem is invalid
            if (!ExpectAnyExternalIntersection)
                return true;

            // Otherwise it might have happened that the new point is the one where the old theorem
            // stated an intersection theorem. We need to check this. Let's take the new point
            var newPoint = contextualPicture.NewPoints.FirstOrDefault();

            // If the last object hasn't been a point, then nothing as explained could have happened
            if (newPoint == null)
                return true;

            // Otherwise we need to check whether the old theorem doesn't state that some objects
            // have an intersection point equal to the new point
            return oldTheorem.InvolvedObjects
                // We know the objects are with points
                .Cast<TheoremObjectWithPoints>()
                // For each we will find the corresponding geometric object definable by points
                .Select(objectWithPoints =>
                {
                    // If the object is defined explicitly, then we simply ask the picture to do the job
                    if (objectWithPoints.DefinedByExplicitObject)
                        return (DefinableByPoints)contextualPicture.GetGeometricObject(objectWithPoints.ConfigurationObject);

                    // Otherwise we need to find the inner points
                    var innerPoints = objectWithPoints.Points
                        // As geometric objects
                        .Select(contextualPicture.GetGeometricObject)
                        // They are points
                        .Cast<PointObject>()
                        // Enumerate
                        .ToArray();

                    // Base on the type of object we will take all lines / circles passing through the first point
                    return (objectWithPoints switch
                    {
                        // If we have a line, take lines
                        LineTheoremObject _ => innerPoints[0].Lines.Cast<DefinableByPoints>(),

                        // If we have a circle, take circles
                        CircleTheoremObject _ => innerPoints[0].Circles,

                        // Unhandled cases
                        _ => throw new TheoremFinderException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {objectWithPoints.GetType()}")
                    })
                    // Take the first line or circle that contains all the points
                    .First(lineOrCircle => lineOrCircle.ContainsAll(innerPoints));
                })
                // The theorem is valid if and only if the new point is not the intersection
                // point of all of its inner object, i.e. there is an object that does not contain it
                .Any(lineOrCircle => !lineOrCircle.Points.Contains(newPoint));
        }

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
                var analyticObjects = objects.Select(_object => contextualPicture.GetAnalyticObject(_object, picture)).ToArray();

                // Intersect them
                var intersections = AnalyticHelpers.Intersect(analyticObjects);

                // Make sure there is exactly one intersection point
                if (intersections.Length != 1)
                    return false;

                // If it is not in the picture, mark it
                if (!picture.Contains(intersections[0]))
                    isIntersectionPointExternal = true;
            }

            // If we're expecting an intersection point outside the picture,
            // then the theorem is fine if and only if there is any
            if (ExpectAnyExternalIntersection)
                return isIntersectionPointExternal;

            // Otherwise it's fine
            return true;
        }

        #endregion
    }
}