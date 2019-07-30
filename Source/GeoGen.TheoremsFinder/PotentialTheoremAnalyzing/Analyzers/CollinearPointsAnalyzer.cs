using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for the type <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    public class CollinearPointsAnalyzer : PotentialTheoremsAnalyzerBase
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
            // And find all the lines that pass through it
            .SelectMany(point => point.Lines)
            // That contain at least 3 points
            .Where(line => line.Points.Count >= 3)
            // Take distinct ones
            .Distinct()
            // Each line makes a theorem
            .Select(line => new PotentialTheorem
            {
                // Set the type using the base property
                TheoremType = Type,

                // Set the verifier function to a constant function returning always true
                VerificationFunction = _ => true,

                // Set the involved objects to the these triple of points
                InvolvedObjects = line.Points
            });
        }
    }
}