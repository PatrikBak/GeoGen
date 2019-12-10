using GeoGen.AnalyticGeometry;
using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a MetaPost figure.
    /// </summary>
    public class MetapostFigure
    {
        /// <summary>
        /// Generates the final code of the figure.
        /// </summary>
        /// <returns>The generated code.</returns>
        public string ToCode()
        {
            // This is some dummy code for now
            return @$"pair A,B,C,D,M;
B = (0,0);
C = (u,0);
A = point 2.5 of CircleWithDiameter(B,C);
M = 0.5[B,C];
D = ProjectedPoint(A,B,C);

draw A--B--C--cycle;
draw A--D;
draw A--M;

draw RightAngleMark(B,A,C);
draw RightAngleMark(M,D,A);

label.top(btex $A$ etex, A);
label.llft(btex $B$ etex, B);
label.lrt(btex $C$ etex, C);
label.bot(btex $D$ etex, D);
label.bot(btex $M$ etex, M);";
        }

        public void AddPoint(Point point, ObjectDrawingStyle style)
        {
        }

        public void AddSegment(Point point1, Point point2, ObjectDrawingStyle style, bool shifted)
        {
        }

        public void AddLine(Line line, Point[] points, ObjectDrawingStyle style, bool shifted)
        {
        }

        public void AddCircle(Circle circle, ObjectDrawingStyle style)
        {
        }
    }
}
