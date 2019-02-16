using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for the type <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    public class CollinearPointsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Take the new points. At least one of the involved points must be new
            return container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = true,
            })
            // And find all the lines that pass through it
            .SelectMany(point => point.Lines.Select(line => (point, line)))
            // Take only those lines that contain at least three points
            .Where(pair => pair.line.Points.Count >= 3)
            // And for every such a line take those triples of points that contain the new one we're interested in
            .SelectMany(pair => pair.line.Points.Subsets(3).Where(points => points.Contains(pair.point)))
            // Each such a triple represents a theorem (not even potential, the contextual container made sure it's true)
            .Select(triple => new PotentialTheorem
            {
                // Set the type using the base property
                TheoremType = Type,

                // Set the verifier function to a constant function returning always true
                VerificationFunction = _ => true,

                // Set the involved objects to the these triple of points
                InvolvedObjects = triple
            });
        }
    }
}