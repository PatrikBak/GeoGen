using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents <see cref="ITheoremDeriver"/> that derives theorems using the <see cref="DerivationRule.ConcyclityWithCirclesFromPoints"/>.
    /// </summary>
    public class ConcyclityWithCirclesFromPointsDeriver : TheoremDeriverBase
    {
        /// <summary>
        /// Takes new theorems and based on logical reason comes up with relationships between them,
        /// i.e. which theorem would be sufficient to prove to come up with some other one of them.
        /// </summary>
        /// <param name="theorems">The theorems between which we're finding relationships.</param>
        /// <returns>The enumerable of all found relationships, i.e. assumptions and the theorem that follows from them.</returns>
        public override IEnumerable<(IReadOnlyList<Theorem> assumptions, Theorem impliedTheorem)> DeriveTheorems(TheoremMap theorems)
        {
            // Go through the concyclities
            foreach (var concyclity in theorems.GetObjectsForKeys(ConcyclicPoints))
            {
                // Get the concyclic points of this concyclity
                var concyclicPoints = concyclity.InvolvedObjects
                    // Each involved object is a point
                    .Select(theoremObject => ((PointTheoremObject)theoremObject).ConfigurationObject)
                    // Enumerate
                    .ToArray();

                // Go through all the theorems that have a circle
                foreach (var theorem in theorems.GetObjectsForKeys(LineTangentToCircle, TangentCircles, ConcurrentObjects))
                {
                    // Get the redefinable circle
                    var redefinableCircle = theorem.InvolvedObjects.OfType<CircleTheoremObject>()
                        // Defined by points
                        .Where(circle => circle.DefinedByPoints)
                        // And it's points must be among the concyclity points
                        .Where(circle => circle.Points.All(concyclicPoints.Contains))
                        // There must be at most one such object
                        .FirstOrDefault();

                    // If there is no circle to be redefined, we can't do much
                    if (redefinableCircle == default)
                        continue;

                    // Get the other objects
                    var otherObjects = theorem.InvolvedObjects.Where(theoremObject => theoremObject != redefinableCircle);

                    // Otherwise we can do the redefinition
                    // Get the fourth point that is not on the circle
                    var fourthPoint = concyclicPoints.First(point => !redefinableCircle.Points.Contains(point));

                    // Create new redefined circles
                    var redefinedCircle1 = new CircleTheoremObject(fourthPoint, redefinableCircle.PointsList[0], redefinableCircle.PointsList[1]);
                    var redefinedCircle2 = new CircleTheoremObject(fourthPoint, redefinableCircle.PointsList[0], redefinableCircle.PointsList[2]);
                    var redefinedCircle3 = new CircleTheoremObject(fourthPoint, redefinableCircle.PointsList[1], redefinableCircle.PointsList[2]);

                    // Create the restated theorems
                    var theorem1 = new Theorem(theorem.Type, otherObjects.Concat(redefinedCircle1));
                    var theorem2 = new Theorem(theorem.Type, otherObjects.Concat(redefinedCircle2));
                    var theorem3 = new Theorem(theorem.Type, otherObjects.Concat(redefinedCircle3));

                    // Return the derivations
                    yield return (new[] { theorem, concyclity }, theorem1);
                    yield return (new[] { theorem, concyclity }, theorem2);
                    yield return (new[] { theorem, concyclity }, theorem3);
                }
            }
        }
    }
}