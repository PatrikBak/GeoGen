using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.EqualLineSegments"/>.
    /// </summary>
    public class EqualLineSegmentsTheoremFinder : TrueInAllPicturesTheoremFinder
    {
        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Get all points
            return contextualPicture.AllPoints.ToList()
                // And all its pairs
                .UnorderedPairs().ToList()
                // And all pairs of these pairs
                .UnorderedPairs()
                // Each represents 4 points
                .Select(points => new[] { points.Item1.Item1, points.Item1.Item2, points.Item2.Item1, points.Item2.Item2 });
        }

        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new points
            var newPoints = contextualPicture.NewPoints.ToList();

            // Find old points
            var oldPoints = contextualPicture.OldPoints.ToList();

            #region Getting new line segments

            // Prepare the list of new line segments
            var newLineSegments = new List<(PointObject, PointObject)>();

            // Combine the new points with themselves
            foreach (var pairOfPoints in newPoints.UnorderedPairs())
                newLineSegments.Add(pairOfPoints);

            // Combine the new points with the old ones
            foreach (var newPoint in newPoints)
                foreach (var oldPoint in oldPoints)
                    newLineSegments.Add((newPoint, oldPoint));

            #endregion

            // Get the old line segments
            var oldLineSegments = oldPoints.UnorderedPairs().ToList();

            // Combine the new line segments with themselves
            foreach (var ((point1, point2), (point3, point4)) in newLineSegments.UnorderedPairs())
                yield return new[] { point1, point2, point3, point4 };

            // Combine the new line segments with the old ones
            foreach (var (point1, point2) in newLineSegments)
                foreach (var (point3, point4) in oldLineSegments)
                    yield return new[] { point1, point2, point3, point4 };
        }

        /// <inheritdoc/>
        protected override bool IsTrue(IAnalyticObject[] objects)
            // Return if the lengths of the represented line segment are the same
            => ((Point)objects[0]).DistanceTo((Point)objects[1]).Rounded() == ((Point)objects[2]).DistanceTo((Point)objects[3]).Rounded();

        /// <inheritdoc/>
        public override bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem)
            // No restrictions
            => true;
    }
}
