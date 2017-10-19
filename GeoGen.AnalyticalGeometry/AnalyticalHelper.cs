using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Core.Utilities;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A default implementation of <see cref="IAnalyticalHelper"/>.
    /// </summary>
    internal class AnalyticalHelper : IAnalyticalHelper
    {
        /// <summary>
        /// Finds all intersections of given analytical objects. They must not
        /// contain <see cref="Point"/>s and duplicate objects.
        /// </summary>
        /// <param name="inputObjects">The input objects.</param>
        /// <returns>The set of intersections. An empty set, if there's none.</returns>
        public HashSet<Point> Intersect(IEnumerable<IAnalyticalObject> inputObjects)
        {
            // Local function to find all intersections of two given analytical objects.
            HashSet<Point> Intersect(IAnalyticalObject o1, IAnalyticalObject o2)
            {
                var o1Line = o1 as Line?;
                var o1Circle = o1 as Circle?;

                var o2Line = o2 as Line?;
                var o2Circle = o2 as Circle?;

                // If we have two lines
                if (o1Line != null && o2Line != null)
                {
                    var result = o1Line.Value.IntersectionWith(o2Line.Value);

                    return result == null ? new HashSet<Point>() : new HashSet<Point> {result.Value};
                }

                // If we have two circles
                if (o1Circle != null && o2Circle != null)
                {
                    return o1Circle.Value.IntersectWith(o2Circle.Value).ToSet();
                }

                // Otherwise we have a line and a circle.
                Line line;
                Circle circle;

                // Find the line.
                if (o1Line != null)
                    line = o1Line.Value;
                else if (o2Line != null)
                    line = o2Line.Value;
                else
                    throw new Exception("Can't intersect point");

                // Find the circle.
                if (o1Circle != null)
                    circle = o1Circle.Value;
                else if (o2Circle != null)
                    circle = o2Circle.Value;
                else
                    throw new Exception("Can't intersect point");

                return circle.IntersectWith(line).ToSet();
            }

            var list = inputObjects.ToList();

            if (list.Count <= 1)
                throw new ArgumentException("There must be at least two objects to intersect.");

            // Pull first two objects
            var first = list[0];
            var second = list[1];

            // Intersect them
            var points = Intersect(first, second);

            // Exclude those intersections that don't line on all other objects
            points.RemoveWhere(point => list.Skip(2).All(obj => LiesOn(obj, point)));

            // Return the resulting points
            return points;
        }

        /// <summary>
        /// Checks if a given point lies on a given analytical object. The object
        /// must not be a points.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="point">The point.</param>
        /// <returns>true, if the point lies on the object, false otherwise.</returns>
        public bool LiesOn(IAnalyticalObject analyticalObject, Point point)
        {
            if (analyticalObject is Point)
                throw new ArgumentException("Analytical object can't be a point");

            if (analyticalObject is Line line)
                return line.Contains(point);

            if (analyticalObject is Circle circle)
                return circle.Contains(point);

            throw new Exception("Unhandled case");
        }
    }
}