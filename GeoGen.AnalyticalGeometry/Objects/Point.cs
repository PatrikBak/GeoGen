namespace GeoGen.AnalyticalGeometry.Objects
{
    public sealed class Point : AnalyticalObject
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

        public static Point Midpoint(Point point1, Point point2)
        {
            return (point1 + point2) / 2;
        }

        private bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((Point) obj);
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