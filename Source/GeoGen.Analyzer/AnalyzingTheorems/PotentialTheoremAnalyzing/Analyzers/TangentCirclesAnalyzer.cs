using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    public class TangentCirclesAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Find new circles. At least one of them must be included in every new theorem
            var newCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeCirces = true
            }).ToList();

            // Find all circles.
            var allCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).ToList();

            // Go through all the new circles
            foreach (var newCircle in newCircles)
            {
                // Go through all the circles
                foreach (var anyCircle in allCircles)
                {
                    // If we happen to have equal ones, skip them
                    if (newCircle == anyCircle)
                        continue;

                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // Cast the circles to their analytic versions
                        var analyticCircle1 = container.GetAnalyticObject<Circle>(newCircle, objectsContainer);
                        var analyticCircle2 = container.GetAnalyticObject<Circle>(anyCircle, objectsContainer);

                        // Return if there are tangent to each other
                        return analyticCircle1.IsTangentTo(analyticCircle2);
                    }

                    // Lazily return the output
                    yield return new PotentialTheorem
                    {
                        // Set the type using the base property
                        TheoremType = Type,

                        // Set the function
                        VerificationFunction = Verify,

                        // Set the involved objects to our two circles
                        InvolvedObjects = new[] { newCircle, anyCircle }
                    };
                }
            }
        }
    }
}