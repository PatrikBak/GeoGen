using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A class containing static utilities for the manupulation with analytic objects.
    /// </summary>
    public static class AnalyticHelpers
    {
        /// <summary>
        /// Finds all intersections of given analytic objects. They must not
        /// contain <see cref="Point"/>s and duplicate objects.
        /// </summary>
        /// <param name="inputObjects">The input objects.</param>
        /// <returns>The set of intersections. An empty set, if there's none.</returns>
        public static HashSet<Point> Intersect(IEnumerable<AnalyticObject> inputObjects)
        {
            // Enumerate distinct objects
            var objectsList = inputObjects.ToList();

            // Pull first two objects
            var first = objectsList[0];
            var second = objectsList[1];

            // Intersect them
            var points = Intersect(first, second);

            // We iterate over remaining objects
            foreach (var analyticObject in objectsList.Skip(2))
            {
                // From current intersections remove the ones that doesn't lie on this object
                points.RemoveWhere(point => !LiesOn(analyticObject, point));
            }

            // Return the resulting points
            return points;
        }

        /// <summary>
        /// Finds all intersections of given analytic objects. They must not
        /// contain <see cref="Point"/>s and duplicate objects.
        /// </summary>
        /// <param name="inputObjects">The input objects.</param>
        /// <returns>The set of intersections. An empty set, if there's none.</returns>
        public static HashSet<Point> Intersect(params AnalyticObject[] inputObjects)
        {
            return Intersect(inputObjects);
        }

        /// <summary>
        /// Intersects two given analytic objects that are not points.
        /// </summary>
        /// <param name="analyticObject1">The first object.</param>
        /// <param name="analyticObject2">The second object.</param>
        /// <returns>The set of intersections.</returns>
        public static HashSet<Point> Intersect(AnalyticObject analyticObject1, AnalyticObject analyticObject2)
        {
            // Safely cast o1 to nullable Line and Circle
            var o1Line = analyticObject1 as Line;
            var o1Circle = analyticObject1 as Circle;

            // The same goes with for o2
            var o2Line = analyticObject2 as Line;
            var o2Circle = analyticObject2 as Circle;

            // If we have two lines
            if (o1Line != null && o2Line != null)
            {
                // Then we call the intersection method
                var result = o1Line.IntersectionWith(o2Line);

                // And if the intersection is null, return an empty set, otherwise the set
                // containing the result
                return result == null ? new HashSet<Point>() : new HashSet<Point> {result};
            }

            // If we have two circles
            if (o1Circle != null && o2Circle != null)
            {
                // Intersect the circles and cast the result to set
                return o1Circle.IntersectWith(o2Circle).ToSet();
            }

            // Otherwise we have a line and a circle.
            Line line;
            Circle circle;

            // Find the line.
            if (o1Line != null)
                line = o1Line;
            else if (o2Line != null)
                line = o2Line;
            else
                throw new AnalyticException("Passed analytic object can't be a point.");

            // Find the circle.
            if (o1Circle != null)
                circle = o1Circle;
            else if (o2Circle != null)
                circle = o2Circle;
            else
                throw new AnalyticException("Passed analytic object can't be a point.");

            // And finally intersect the circle and the line
            return circle.IntersectWith(line).ToSet();
        }

        /// <summary>
        /// Checks if a given point lies on a given analytic object. The object
        /// must not be a point.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <param name="point">The point.</param>
        /// <returns>true, if the point lies on the object, false otherwise.</returns>
        public static bool LiesOn(AnalyticObject analyticObject, Point point)
        {
            if (analyticObject is Line line)
                return line.Contains(point);

            if (analyticObject is Circle circle)
                return circle.Contains(point);

            throw new AnalyticException("Passed analytic object can't be a point.");
        }

        /// <summary>
        /// Finds out if given points are collinear.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>true if all points are collinear; false otherwise.</returns>
        public static bool AreCollinear(params Point[] points)
        {
            // Take two points and construct the line
            var line = new Line(points[0], points[1]);

            // Return if all other points lie on this line
            return points.Skip(2).All(line.Contains);
        }
    }
}