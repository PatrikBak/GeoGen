using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    internal class TangentCirclesVerifier : TheoremVerifierBase
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
        {
            // Find all new circles. At least one of them must be included in a new theorem
            var newCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // Find all old circles.
            var oldCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // A local function for combinining all the circles so that at least one of them is new
            IEnumerable<(CircleObject circle1, CircleObject circle2)> CombineCircles()
            {
                // First combine the new circles with themselves
                for (var i = 0; i < newCircles.Count; i++)
                {
                    for (var j = i + 1; j < newCircles.Count; j++)
                    {
                        yield return (newCircles[i], newCircles[j]);
                    }
                }

                // Now combine new circles with old ones
                foreach (var newCircle in newCircles)
                {
                    foreach (var oldCircle in oldCircles)
                    {
                        yield return (newCircle, oldCircle);
                    }
                }
            }

            // Now execute the local function to find all potential pairs of circles
            foreach (var (circle1, circle2) in CombineCircles())
            {
                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Pull analytical circles representing each one
                    var analyticalCircle1 = container.GetAnalyticalObject<Circle>(circle1, objectsContainer);
                    var analyticalCircle2 = container.GetAnalyticalObject<Circle>(circle2, objectsContainer);

                    // Return if there are tangent to each other
                    return analyticalCircle1.IsTangentTo(analyticalCircle2);
                }

                // Construct the output
                yield return new PotentialTheorem
                {
                    TheoremType = Type,
                    InvolvedObjects = new[] { circle1, circle2 },
                    VerifierFunction = Verify
                };
            }
        }
    }
}