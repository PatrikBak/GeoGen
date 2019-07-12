using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    public class LineTangentToCircleAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualPicture contextualPicture)
        {
            // Find new circles. Either a new line or a new circle must be included in every new theorem
            var newCircles = contextualPicture.GetGeometricObjects<CircleObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludeCirces = true
            }).ToList();

            // Find new lines. Either a new line or a new circle must be included in every new theorem
            var newLines = contextualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludeLines = true,
            }).ToList();

            // Find all circles.
            var allCircles = contextualPicture.GetGeometricObjects<CircleObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.All,
                IncludeCirces = true
            }).ToList();

            // Find old lines.
            var oldLines = contextualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.Old,
                IncludeLines = true,
            }).ToList();

            // A local helper function for combining pairs consisting of
            // one line and one circle where at least one of them is new
            IEnumerable<(LineObject, CircleObject)> CombineLinesWithCircles()
            {
                // First combine the new lines with all the circles
                foreach (var newLine in newLines)
                    foreach (var anyCircle in allCircles)
                        yield return (newLine, anyCircle);

                // Then combine the new circles with just the old lines
                foreach (var newCircle in newCircles)
                    foreach (var oldLine in oldLines)
                        yield return (oldLine, newCircle);
            }

            // Go through all the possible combinations
            foreach (var (line, circle) in CombineLinesWithCircles())
            {
                // Construct the verifier function
                bool Verify(IPicture picture)
                {
                    // Pull analytic circles representing each one
                    var analyticLine = contextualPicture.GetAnalyticObject<Line>(line, picture);
                    var analyticCircle = contextualPicture.GetAnalyticObject<Circle>(circle, picture);

                    // Return if there are tangent to each other
                    return analyticCircle.IsTangentTo(analyticLine);
                }

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our line and circle
                    InvolvedObjects = new GeometricObject[] { line, circle },
                };
            }
        }
    }
}