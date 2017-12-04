using System;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities
{
    public static class MathUtils
    {
        public static List<double> SolveQuadraticEquation(double a, double b, double c)
        {
            if ((RoundedDouble) a == 0)
                throw new ArithmeticException("The x^2 coefficient cannot be zero.");

            var d = (RoundedDouble) (b * b - 4 * a * c);

            if (d < 0)
                return new List<double>();

            if (d == 0)
                return new List<double> {-b / (2 * a)};

            var squareRoot = Math.Sqrt(d);

            var root1 = (-b - squareRoot) / (2 * a);
            var root2 = (-b + squareRoot) / (2 * a);

            return new List<double> {root1, root2};
        }

        public static double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180);
        }
    }
}