using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.AnalyticalGeometry
{
    internal class Circle : GeometricalObject
    {
        public Point Center { get; }

        public double Radius { get; }

        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public static bool operator ==(Circle left, Circle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Circle left, Circle right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(GeometricalObject other)
        {
            var circle = other as Circle;

            if (circle == null)
                return false;

            return AnalyticalHelpers.AreCirclesEqual(circle, this);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(null, obj))
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((GeometricalObject) obj);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Center.GetHashCode();
            hash = hash * 23 + Radius.GetHashCode();

            return hash;
        }
    }
}