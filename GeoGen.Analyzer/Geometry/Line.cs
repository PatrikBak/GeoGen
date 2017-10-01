namespace GeoGen.Analyzer.Geometry
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
    }
}