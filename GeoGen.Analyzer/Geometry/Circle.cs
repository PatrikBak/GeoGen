namespace GeoGen.Analyzer.Geometry
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
    }
}