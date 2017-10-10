using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.AnalyticalGeometry
{
    internal class Circle 
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

       

        
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Center.GetHashCode();
            hash = hash * 23 + Radius.GetHashCode();

            return hash;
        }
    }
}