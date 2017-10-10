using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.AnalyticalGeometry
{
    internal class Point 
    {
        public double X { get; }

        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point operator -(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static Point operator /(Point point, double factor)
        {
            return new Point(point.X / factor, point.Y / factor);
        }

        public static Point operator *(Point point, double factor)
        {
            return new Point(point.X * factor, point.Y * factor);
        }

        public static bool operator ==(Point left, Point right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !Equals(left, right);
        }

        


        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();

            return hash;
        }
    }
}