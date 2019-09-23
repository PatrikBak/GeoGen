using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.RedefinableByCollinearity"/>.
    /// </summary>
    public class CollinearityWithLinesFromPointsDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremsMap theorems)
        {
            // Go through the collinearities
            foreach (var collinearity in theorems.GetTheoremsOfTypes(CollinearPoints))
            {
                // Get the collinear points of this collinearity
                var collinearPoints = collinearity.InvolvedObjects
                    // Each involved object is a point
                    .Select(theoremObject => ((PointTheoremObject)theoremObject).ConfigurationObject)
                    // Enumerate
                    .ToArray();

                // Go through all the theorems that might have a line
                foreach (var theorem in theorems.GetTheoremsOfTypes(ParallelLines, PerpendicularLines, ConcurrentLines, ConcurrentObjects, LineTangentToCircle))
                {
                    // Get the redefinable line
                    var redefinableLine = theorem.InvolvedObjects.OfType<LineTheoremObject>()
                        // Defined by points
                        .Where(line => line.DefinedByPoints)
                        // And it's points must be among the collinearity points
                        .Where(line => line.Points.All(collinearPoints.Contains))
                        // There must be at most one such object
                        .FirstOrDefault();

                    // If there is no line to be redefined, we can't do much
                    if (redefinableLine == default)
                        continue;

                    // Get the other objects
                    var otherObjects = theorem.InvolvedObjects.Where(theoremObject => theoremObject != redefinableLine);

                    // Otherwise we can do the redefinition
                    // Get the third point that is not on the line
                    var thirdPoint = collinearPoints.First(point => !redefinableLine.Points.Contains(point));

                    // Create new redefined lines
                    var redefinedLine1 = new LineTheoremObject(thirdPoint, redefinableLine.PointsList[0]);
                    var redefinedLine2 = new LineTheoremObject(thirdPoint, redefinableLine.PointsList[1]);

                    // Create the restated theorems
                    var theorem1 = new Theorem(theorem.Configuration, theorem.Type, otherObjects.Concat(redefinedLine1));
                    var theorem2 = new Theorem(theorem.Configuration, theorem.Type, otherObjects.Concat(redefinedLine2));

                    // Return the derivations
                    yield return (new[] { theorem, collinearity }, theorem1);
                    yield return (new[] { theorem, collinearity }, theorem2);

                    // In some case two of these theorems can imply the last one
                    switch (theorem.Type)
                    {
                        // It's easy to see these are the types
                        case ConcurrentLines:
                        case ParallelLines:
                        case PerpendicularLines:

                            // For example, two parallel lines mean collinearity right away
                            yield return (new[] { theorem1, theorem2 }, collinearity);
                            break;
                    }
                }
            }
        }
    }
}
