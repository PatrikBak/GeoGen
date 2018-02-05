using System;
using System.Collections.Generic;
using GeoGen.Utilities;

namespace GeoGen.AnalyticalGeometry.Mathematics
{
    /// <summary>
    /// A utilities for math operations.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Solves a given quadratic equation. This method internally uses <see cref="RoundedDouble"/>s.
        /// </summary>
        /// <param name="a">The a coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <param name="b">The b coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <param name="c">The c coefficient of the equation ax^2 + bx + c = 0.</param>
        /// <returns>The list of solutions. If there is no solution, an empty list.</returns>
        public static List<decimal> SolveQuadraticEquation(decimal a, decimal b, decimal c)
        {
            // Calculate and round the discriminant
            var d = (RoundedDouble) (b * b - 4 * a * c);

            // If it less than zero, then we have no solutions
            if (d < RoundedDouble.Zero)
                return new List<decimal>();

            // If it's exactly zero, then we have exactly one solution
            if (d == RoundedDouble.Zero)
                return new List<decimal> {-b / (2 * a)};

            // Otherwise we have 2 solution. Let's calculate sqrt(d).
            var squareRoot = (decimal)Math.Sqrt((double) d.OriginalValue);

            // The solutions are then give by the formula (-b +- sqrt(d)) / (2a)
            // This can be easily concluded using the completion to square.
            var root1 = (-b - squareRoot) / (2 * a);
            var root2 = (-b + squareRoot) / (2 * a);

            // Finally we wrap the solutions as list.
            return new List<decimal> {root1, root2};
        }

        /// <summary>
        /// Converts a given angle to radius. 
        /// </summary>
        /// <param name="angleInDegrees">The angel in degrees.</param>
        /// <returns>The angel in radians.</returns>
        public static double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180);
        }
    }
}