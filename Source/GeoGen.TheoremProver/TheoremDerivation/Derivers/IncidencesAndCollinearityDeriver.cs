using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.IncidencesAndCollinearity"/>.
    /// </summary>
    public class IncidencesAndCollinearityDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the triples of incidences
            foreach (var incidences in theorems.GetTheoremsOfTypes(Incidence).Subsets(3))
            {
                // Get their points
                var points = incidences.Select(i => i.InvolvedObjects.OfType<PointTheoremObject>().First().ConfigurationObject).ToArray();

                // Get their lines
                var lines = incidences.Select(i => i.InvolvedObjects.OfType<TheoremObjectWithPoints>().First()).ToArray();

                // Make sure the lines are lines and there is exactly one
                if (!lines.All(l => l is LineTheoremObject) || lines.Distinct().Count() != 1)
                    continue;

                // Prepare the collinearity
                var collinearity = new Theorem(CollinearPoints, points);

                // Any three of our theorems imply the last
                yield return (new[] { incidences[0], incidences[1], incidences[2] }, collinearity);
                yield return (new[] { collinearity, incidences[1], incidences[2] }, incidences[0]);
                yield return (new[] { incidences[0], collinearity, incidences[2] }, incidences[1]);
                yield return (new[] { incidences[0], incidences[1], collinearity }, incidences[2]);
            }
        }
    }
}