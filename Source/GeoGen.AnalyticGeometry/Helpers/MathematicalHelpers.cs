using GeoGen.Utilities;
using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A static helper class for math operations.
    /// </summary>
    public static class MathematicalHelpers
    {
        /// <summary>
        /// Solves a given quadratic equation of the form ax^2 + bx + c = 0. This 
        /// method internally uses <see cref="DoubleExtensions.Rounded(double, int)"/> method.
        /// </summary>
        /// <param name="a">The a coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <param name="b">The b coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <param name="c">The c coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <returns>An array of solutions. If there is no solution, an empty array.</returns>
        public static double[] SolveQuadraticEquation(double a, double b, double c)
        {
            // Calculate the discriminant
            var D = b * b - 4 * a * c;

            // If it less than zero, then we have no solutions
            if (D.Rounded() < 0)
                return new double[0];

            // If it's exactly zero, then we have exactly one solution
            if (D.Rounded() == 0)
                return new[] { -b / (2 * a) };

            // Otherwise we have 2 solution. Let's calculate sqrt(D).
            var sqrtD = Math.Sqrt(D);

            // The solutions are then give by the formula (-b +- sqrt(D)) / (2a)
            // This can be easily concluded using the completion to square.
            var root1 = (-b - sqrtD) / (2 * a);
            var root2 = (-b + sqrtD) / (2 * a);

            // Return them in an array
            return new[] { root1, root2 };
        }

        /// <summary>
        /// Converts a given angle in degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double ToRadians(double angleInDegrees) => angleInDegrees * Math.PI / 180;
    }
}