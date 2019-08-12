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
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(ContextualPicture contextualPicture)
        {
            // Take the new point
            var point = contextualPicture.GetGeometricObjects<PointObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludePoints = true,
            })
            // There should be at most one
            .SingleOrDefault();

            // If there's none, we can't do more
            if (point == null)
                return new PotentialTheorem[0];

            // Take its circles 
            return point.Circles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // For each take all triples of its points distinct from our point + append our point
                .SelectMany(circle => circle.Points.Where(_point => _point != point).Subsets(3).Select(subset => subset.Concat(point)))
                // Each of these quadruples makes a theorem
                .Select(points => new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the verifier function to a constant function returning always true
                    VerificationFunction = _ => true,

                    // Set the involved objects to the these triple of points
                    InvolvedObjects = points
                });
        }
    }
}