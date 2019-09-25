using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.ExplicitLineWithIncidences"/>.
    /// </summary>
    public class ExplicitLineWithIncidencesDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the pairs of incidences
            foreach (var incidences in theorems.GetTheoremsOfTypes(Incidence).Subsets(2))
            {
                // Extract the common line
                var commonLine = incidences[0].InvolvedObjects.Intersect(incidences[1].InvolvedObjects).FirstOrDefault();

                // If there is no common line, or the common object is a point
                // then we can't do much
                if (commonLine == default || !(commonLine is LineTheoremObject))
                    continue;

                // Otherwise prepare the points by taking all the objects
                var points = incidences[0].InvolvedObjects.Concat(incidences[1].InvolvedObjects)
                    // Excluding the line
                    .Except(commonLine.ToEnumerable())
                    // Getting the inner objects
                    .Select(point => ((PointTheoremObject)point).ConfigurationObject)
                    // Enumerating
                    .ToArray();

                // Go through all the theorems that might have a line
                foreach (var theorem in theorems.GetTheoremsOfTypes(ParallelLines, PerpendicularLines, ConcurrentLines, ConcurrentObjects, LineTangentToCircle))
                {
                    // Get the redefinable line
                    var redefinableLine = theorem.InvolvedObjects.OfType<LineTheoremObject>()
                        // Take those that are either defined by points and contains our points
                        // Or it is explicit and equal to our common line
                        .Where(line => line.DefinedByPoints && points.All(line.Points.Contains) || line.Equals(commonLine))
                        // There must be at most one such object
                        .FirstOrDefault();

                    // If there is no line to be redefined, we can't do much
                    if (redefinableLine == default)
                        continue;

                    // Get the other objects
                    var otherObjects = theorem.InvolvedObjects.Where(theoremObject => theoremObject != redefinableLine);

                    // Prepare the redefined line...If the current line is defined by points
                    var redefinedLine = redefinableLine.DefinedByPoints ?
                        // Then we need to replace them with the common line
                        commonLine
                        // Otherwise we wrap the points in a line theorem object
                        : new LineTheoremObject(points[0], points[1]);

                    // Create the restated theorem
                    var restatedTheorem = new Theorem(theorem.Configuration, theorem.Type, otherObjects.Concat(redefinedLine));

                    // Return the derivations
                    yield return (new[] { theorem, incidences[0], incidences[1] }, restatedTheorem);
                    yield return (new[] { restatedTheorem, incidences[0], incidences[1] }, theorem);

                    // In some cases we can imply even the incidence
                    switch (theorem.Type)
                    {
                        // It's easy to see these are the types
                        case ConcurrentLines:
                        case ParallelLines:
                        case PerpendicularLines:

                            // For example, two parallel lines with one incidence mean the other
                            yield return (new[] { theorem, restatedTheorem, incidences[0] }, incidences[1]);
                            yield return (new[] { theorem, restatedTheorem, incidences[1] }, incidences[0]);
                            break;
                    }
                }
            }
        }
    }
}
