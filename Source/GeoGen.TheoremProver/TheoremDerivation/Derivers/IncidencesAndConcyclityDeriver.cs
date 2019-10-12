using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.IncidencesAndConcyclity"/>.
    /// </summary>
    public class IncidencesAndConcyclityDeriver : TheoremDeriverBase
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
            foreach (var incidences in theorems.GetTheoremsOfTypes(Incidence).Subsets(4))
            {
                // Get their points
                var points = incidences.Select(i => i.InvolvedObjects.OfType<PointTheoremObject>().First().ConfigurationObject).ToArray();

                // Get their circles
                var circles = incidences.Select(i => i.InvolvedObjects.OfType<TheoremObjectWithPoints>().First()).ToArray();

                // Make sure the circles are circles and there is exactly one
                if (!circles.All(c => c is CircleTheoremObject) || circles.Distinct().Count() != 1)
                    continue;

                // Prepare the concyclity
                var concyclity = new Theorem(ConcyclicPoints, points);

                // Any four of our theorems imply the last
                yield return (new[] { incidences[0], incidences[1], incidences[2], incidences[3] }, concyclity);
                yield return (new[] { concyclity, incidences[1], incidences[2], incidences[3] }, incidences[0]);
                yield return (new[] { incidences[0], concyclity, incidences[2], incidences[3] }, incidences[1]);
                yield return (new[] { incidences[0], incidences[1], concyclity, incidences[3] }, incidences[2]);
                yield return (new[] { incidences[0], incidences[1], incidences[2], concyclity }, incidences[3]);
            }
        }
    }
}