using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.AnalyticalGeometry
{
    internal class Line : GeometricalObject
    {
        public Point EndPoint1 { get; }

        public Point EndPoint2 { get; }

        public Line(Point endPoint1, Point endPoint2)
        {
            EndPoint1 = endPoint1;
            EndPoint2 = endPoint2;
        }

        public override bool Equals(GeometricalObject other)
        {
            var line = other as Line;

            if (line == null)
                return false;

            return AnalyticalHelpers.AreLinesEqual(this, line);
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
            hash = hash * 23 + EndPoint1.GetHashCode();
            hash = hash * 23 + EndPoint2.GetHashCode();

            return hash;
        }
    }
}