namespace GeoGen.Analyzer.Geometry
{
    internal class Point : GeometricalObject
    {
        public double X { get; }

        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}