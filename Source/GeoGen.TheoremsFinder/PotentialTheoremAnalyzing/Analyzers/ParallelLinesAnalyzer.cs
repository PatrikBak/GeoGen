using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.ParallelLines"/>.
    /// </summary>
    public class ParallelLinesAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(ContextualPicture contextualPicture)
        {
            // Find new lines. At least one of them must be included in every new theorem
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

            // A local helper function for getting all the pairs of 
            // lines where at least of them is new
            IEnumerable<(LineObject, LineObject)> NewPairsOfLines()
            {
                // First combine the new lines with themselves
                foreach (var pairOfLines in newLines.UnorderedPairs())
                    yield return pairOfLines;

                // Now combine the new lines with the old ones
                foreach (var newLine in newLines)
                    foreach (var oldLine in oldLines)
                        yield return (newLine, oldLine);
            }

            // Go through all the possible combinations
            foreach (var (line1, line2) in NewPairsOfLines())
            {
                // Construct the verifier function
                bool Verify(Picture picture)
                {
                    // Cast the lines to their analytic versions
                    var analyticLine1 = contextualPicture.GetAnalyticObject<Line>(line1, picture);
                    var analyticLine2 = contextualPicture.GetAnalyticObject<Line>(line2, picture);

                    // Return if there are perpendicular
                    return analyticLine1.IsParallelTo(analyticLine2);
                }

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our two lines
                    InvolvedObjects = new[] { line1, line2 }
                };
            }
        }
    }
}