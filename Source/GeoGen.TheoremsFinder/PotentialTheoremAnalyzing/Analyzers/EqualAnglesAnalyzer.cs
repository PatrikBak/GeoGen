using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.EqualAngles"/>.
    /// </summary>
    public class EqualAnglesAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(ContextualPicture contextualPicture)
        {
            // Find new lines.  At least one of them must be included in every new theorem
            var newLines = contextualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludeLines = true
            }).ToList();

            // Find old lines. 
            var oldLines = contextualPicture.GetGeometricObjects<LineObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.Old,
                IncludeLines = true
            }).ToList();

            // A local helper function for getting all the pairs of line
            // representing an angle where at leasts one line is new
            IEnumerable<(LineObject line1, LineObject line2)> NewAngles()
            {
                // First combine the new lines with themselves
                foreach (var pairOfLines in newLines.UnorderedPairs())
                    yield return pairOfLines;

                // Now combine the new lines with the old ones
                foreach (var newLine in newLines)
                    foreach (var oldLine in oldLines)
                        yield return (newLine, oldLine);
            }

            // A local helper function for getting all the pairs of 
            // angles where at least one contains a new line
            IEnumerable<((LineObject, LineObject), (LineObject, LineObject))> NewPairsOfAngles()
            {
                // First enumerate the new angles
                var newAngles = NewAngles().ToList();

                // Now enumerate the old line angles
                var oldAngles = oldLines.UnorderedPairs().ToList();

                // Now we can combine the new angles with themselves
                foreach (var pairOfNewAngles in newAngles.UnorderedPairs())
                    yield return pairOfNewAngles;

                // And the new angles with the old ones
                foreach (var newAngle in newAngles)
                    foreach (var oldAngle in oldAngles)
                        yield return (newAngle, oldAngle);
            }

            // Go through all the possible combinations
            foreach (var ((line1, line2), (line3, line4)) in NewPairsOfAngles())
            {
                // Construct the verifier function
                bool Verify(Picture picture)
                {
                    // Cast the lines to their analytic versions
                    var analyticLine1 = contextualPicture.GetAnalyticObject<Line>(line1, picture);
                    var analyticLine2 = contextualPicture.GetAnalyticObject<Line>(line2, picture);
                    var analyticLine3 = contextualPicture.GetAnalyticObject<Line>(line3, picture);
                    var analyticLine4 = contextualPicture.GetAnalyticObject<Line>(line4, picture);

                    // Find their angles
                    var angle1 = AnalyticHelpers.AngleBetweenLines(analyticLine1, analyticLine2).Rounded();
                    var angle2 = AnalyticHelpers.AngleBetweenLines(analyticLine3, analyticLine4).Rounded();

                    // Return if they match and are not equal to 0 (i.e. parallelity)
                    return angle1 == angle2 && angle1 != 0;
                }

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our four angle lines
                    InvolvedObjects = new[] { line1, line2, line3, line4 }
                };
            }
        }
    }
}