using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;
using NUnit.Framework;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class MathUtilsTest
    {
        [Test]
        public void Test_Solve_Quadratic_Equation_With_No_Root()
        {
            var coefficients = new List<Tuple<double, double, double>>
            {
                new Tuple<double, double, double>(17, 1, 25),
                new Tuple<double, double, double>(-11, 4, -13),
                new Tuple<double, double, double>(25, 10, 1 + Math.Pow(0.1, RoundedDouble.DoubleRoundingPrecision)),
                new Tuple<double, double, double>(1, 1, 1)
            };

            foreach (var coefficient in coefficients)
            {
                var solutions = MathUtils.SolveQuadraticEquation(coefficient.Item1, coefficient.Item2, coefficient.Item3);
                Assert.IsEmpty(solutions);
            }
        }

        [Test]
        public void Test_Solve_Quadratic_Equation_With_One_Root()
        {
            var coefficients = new List<Tuple<double, double, double>>
            {
                new Tuple<double, double, double>(1, 2, 1),
                new Tuple<double, double, double>(1, -2, 1),
                new Tuple<double, double, double>(-70, Math.Sqrt(14), -0.05)
            };

            var roots = new List<double>
            {
                -1,
                1,
                Math.Sqrt(14) / 140
            };

            for (var i = 0; i < coefficients.Count; i++)
            {
                var tuple = coefficients[i];
                var solutions = MathUtils.SolveQuadraticEquation(tuple.Item1, tuple.Item2, tuple.Item3);
                Assert.AreEqual(1, solutions.Count);
                Assert.AreEqual(solutions[0], roots[i]);
            }
        }

        [Test]
        public void Test_Solve_Quadratic_Equation_With_Two_Roots()
        {
            var coefficients = new List<Tuple<double, double, double>>
            {
                new Tuple<double, double, double>(1, -1, -2),
                new Tuple<double, double, double>(1, -7, -5),
                new Tuple<double, double, double>(-2, -7, -1)
            };

            var roots = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(-1, 2),
                new Tuple<double, double>(7.0 / 2 + Math.Sqrt(69) / 2, 7.0 / 2 - Math.Sqrt(69) / 2),
                new Tuple<double, double>(-7.0 / 4 + Math.Sqrt(41) / 4, -7.0 / 4 - Math.Sqrt(41) / 4)
            };

            for (var i = 0; i < coefficients.Count; i++)
            {
                var current = coefficients[i];
                var solutions = MathUtils.SolveQuadraticEquation(current.Item1, current.Item2, current.Item3);
                Assert.AreEqual(2, solutions.Count);
                Assert.IsTrue(solutions.Contains(roots[i].Item1));
                Assert.IsTrue(solutions.Contains(roots[i].Item2));
            }
        }

        [Test]
        public void Test_Solve_Quadratic_Equation_When_A_Is_Zero()
        {
            var coefficients = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(1, 5),
                new Tuple<double, double>(2, 3),
                new Tuple<double, double>(-4, 5),
                new Tuple<double, double>(-7, 11)
            };

            foreach (var coefficient in coefficients)
            {
                Assert.Throws<ArithmeticException>(() => MathUtils.SolveQuadraticEquation(0, coefficient.Item1, coefficient.Item2));
            }
        }

        [Test]
        public void Test_To_Radians()
        {
            Assert.AreEqual((RoundedDouble)0, (RoundedDouble)Math.Sin(MathUtils.ToRadians(0)));
            Assert.AreEqual((RoundedDouble)(Math.Sqrt(2) / 2), (RoundedDouble)Math.Sin(MathUtils.ToRadians(45)));
            Assert.AreEqual((RoundedDouble)1, (RoundedDouble)Math.Sin(MathUtils.ToRadians(90)));
            Assert.AreEqual((RoundedDouble)0, (RoundedDouble)Math.Sin(MathUtils.ToRadians(180)));
            Assert.AreEqual((RoundedDouble)0, (RoundedDouble)Math.Sin(MathUtils.ToRadians(360)));
        }
    }
}