using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.ExplicitCircleWithIncidences"/>.
    /// </summary>
    public class ExplicitCircleWithIncidencesDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the quadruples of incidences
            foreach (var incidences in theorems.GetObjectsForKeys(Incidence).Subsets(3))
            {
                // Extract the common circle
                var commonCircle = incidences[0].InvolvedObjects.Intersect(incidences[1].InvolvedObjects).Intersect(incidences[2].InvolvedObjects).FirstOrDefault();

                // If there is no common c, or the common object is not a circle
                // then we can't do much
                if (commonCircle == default || !(commonCircle is CircleTheoremObject))
                    continue;

                // Otherwise prepare the points by taking all the objects
                var points = incidences[0].InvolvedObjects.Concat(incidences[1].InvolvedObjects).Concat(incidences[2].InvolvedObjects)
                    // Excluding the circle
                    .Except(commonCircle.ToEnumerable())
                    // Getting the inner objects
                    .Select(point => ((PointTheoremObject)point).ConfigurationObject)
                    // Enumerating
                    .ToArray();

                // Go through all the theorems that might have a circle
                foreach (var theorem in theorems.GetObjectsForKeys(LineTangentToCircle, TangentCircles, ConcurrentObjects))
                {
                    // Get the redefinable circle
                    var redefinableCircle = theorem.InvolvedObjects.OfType<CircleTheoremObject>()
                        // Take those that are either defined by points and contains our points
                        // Or it is explicit and equal to our common circle
                        .Where(circle => circle.DefinedByPoints && points.All(circle.Points.Contains) || circle.Equals(commonCircle))
                        // There must be at most one such object
                        .FirstOrDefault();

                    // If there is no circle to be redefined, we can't do much
                    if (redefinableCircle == default)
                        continue;

                    // Get the other objects
                    var otherObjects = theorem.InvolvedObjects.Where(theoremObject => theoremObject != redefinableCircle);

                    // Prepare the redefined circle...If the current circle is defined by points
                    var redefinedCircle = redefinableCircle.DefinedByPoints ?
                        // Then we need to replace them with the common circle
                        commonCircle
                        // Otherwise we wrap the points in a circle theorem object
                        : new CircleTheoremObject(points[0], points[1], points[2]);

                    // Create the restated theorem
                    var restatedTheorem = new Theorem(theorem.Type, otherObjects.Concat(redefinedCircle));

                    // Return the derivations
                    yield return (new[] { theorem, incidences[0], incidences[1], incidences[2] }, restatedTheorem);
                    yield return (new[] { restatedTheorem, incidences[0], incidences[1], incidences[2] }, theorem);
                }
            }
        }
    }
}
