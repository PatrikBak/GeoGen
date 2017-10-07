using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.AnalyticalGeometry
{
    internal static class AnalyticalHelpers
    {
        public static bool AreLinesEqual(Line line1, Line line2)
        {
            throw new NotImplementedException();
        }

        public static bool ArePointsEqual(Point point1, Point point2)
        {
            return point1.X.AlmostEquals(point2.X) && point1.Y.AlmostEquals(point2.Y);
        }

        public static bool AreCirclesEqual(Circle circle1, Circle circle2)
        {
            throw new NotImplementedException();
        }

        public static Point Midpoint(Point point1, Point point2)
        {
            throw new NotImplementedException();
        }

        public static Point IntersectionOfLines(List<Point> points)
        {
            throw new NotImplementedException();
        }

        public static bool ArePointsCollinear(List<Point> points)
        {
            throw new NotImplementedException();
        }
    }
}