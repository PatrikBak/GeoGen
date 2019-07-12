using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for the type <see cref="TheoremType.ConcyclicPoints"/>.
    /// </summary>
    public class ConcyclicPointsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualPicture contextualPicture)
        {
            // Take the new points. At least one of the involved points must be new
            return contextualPicture.GetGeometricObjects<PointObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludePoints = true,
            })
            // And find all the circles that pass through it
            .SelectMany(point => point.Circles.Select(circle => (point, circle)))
            // Take only those circles that contain at least 4 points
            .Where(pair => pair.circle.Points.Count >= 4)
            // And for every such a circle take those quadruples of points that contain the new one we're interested in
            .SelectMany(pair => pair.circle.Points.Subsets(4).Where(points => points.Contains(pair.point)))
            // Each such a quadruples represents a theorem (not even potential, the contextual picture made sure it's true)
            .Select(quadruple => new PotentialTheorem
            {
                // Set the type using the base property
                TheoremType = Type,

                // Set the verifier function to a constant function returning always true
                VerificationFunction = _ => true,

                // Set the involved objects to the these quadruples of points
                InvolvedObjects = quadruple
            });
        }
    }
}