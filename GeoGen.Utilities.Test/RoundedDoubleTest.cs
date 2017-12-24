using System;
using System.Collections.Generic;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Utilities.RoundedDouble;

namespace GeoGen.Core.Test.Utilities
{
    [TestFixture]
    public class RoundedDoubleTest
    {
        private static double TestNumber(int numberOfDecimalOnes)
        {
            var result = 0.0;
            var currentPower = 0.1;

            for (var i = 0; i < numberOfDecimalOnes; i++)
            {
                result += currentPower;
                currentPower /= 10;
            }
            return result;
        }

        [Test]
        public void Test_Constructor()
        {
            var numbers = new List<double>
            {
                TestNumber(0),
                TestNumber(1),
                TestNumber(10),
                TestNumber(DoubleRoundingPrecision - 1),
                TestNumber(DoubleRoundingPrecision),
                TestNumber(DoubleRoundingPrecision + 1),
                TestNumber(DoubleRoundingPrecision + 10)
            };

            var roundedValues = new List<double>
            {
                0,
                0.1,
                0.1111111111,
                TestNumber(DoubleRoundingPrecision - 1),
                TestNumber(DoubleRoundingPrecision),
                TestNumber(DoubleRoundingPrecision),
                TestNumber(DoubleRoundingPrecision)
            };

            for (var i = 0; i < numbers.Count; i++)
            {
                var original = numbers[i];
                var rounded = new RoundedDouble(original);

                Assert.AreEqual(original, rounded.OriginalValue);
                Assert.AreEqual(roundedValues[i], rounded.RoundedValue);
            }
        }

        [Test]
        public void Test_Implicit_Cast_From_Rounded_Double_To_Double()
        {
            var rounded = new RoundedDouble(TestNumber(DoubleRoundingPrecision + 1));

            var number = 1.0 * rounded;

            Assert.AreEqual(number, rounded.OriginalValue);
            Assert.AreEqual(TestNumber(DoubleRoundingPrecision), rounded.RoundedValue);
        }

        [Test]
        public void Test_Implicit_Cast_From_Double_To_Rounded_Double()
        {
            var number = TestNumber(DoubleRoundingPrecision + 1);

            RoundedDouble rounded = number;

            Assert.AreEqual(number, rounded.OriginalValue);
            Assert.AreEqual(TestNumber(DoubleRoundingPrecision), rounded.RoundedValue);
        }

        [Test]
        public void Test_Plus_Minus_Multiply_Divide_Operators_Persevers_Standard_Double_Operators()
        {
            var doubles = new List<double>
            {
                0.5,
                0.7,
                7e-15,
                4e-66,
                12,
                7e100,
                TestNumber(DoubleRoundingPrecision),
                TestNumber(DoubleRoundingPrecision + 10),
                TestNumber(DoubleRoundingPrecision - 1)
            };

            foreach (var double1 in doubles)
            {
                foreach (var double2 in doubles)
                {
                    RoundedDouble rounded1 = double1;
                    RoundedDouble rounded2 = double2;

                    var sum = rounded1 + rounded2;
                    var difference = rounded1 - rounded2;
                    var product = rounded1 * rounded2;
                    var division = rounded1 / rounded2;

                    Assert.AreEqual(rounded1 + rounded2, sum);
                    Assert.AreEqual(rounded1 - rounded2, difference);
                    Assert.AreEqual(rounded1 * rounded2, product);
                    Assert.AreEqual(rounded1 / rounded2, division);
                }
            }
        }

        [Test]
        public void Test_Comparision_Operators_Are_Invonked_On_Rounded_Values()
        {
            var double1 = (RoundedDouble) TestNumber(DoubleRoundingPrecision);
            var double2 = (RoundedDouble) TestNumber(DoubleRoundingPrecision + 2);

            Assert.IsFalse(double1 < double2);
            Assert.IsTrue(double1 <= double2);
            Assert.IsFalse(double1 > double2);
            Assert.IsTrue(double1 >= double2);
            Assert.IsTrue(double1 == double2);
            Assert.IsFalse(double1 != double2);
        }

        [Test]
        public void Test_Equal_And_Hash_Code_Square_Roots()
        {
            var double1 = (RoundedDouble) Math.Sqrt(1.0 / 3);
            var double2 = (RoundedDouble) (Math.Sqrt(3) / 3);

            Assert.IsTrue(double1 == double2);
            Assert.IsTrue(double2 == double1);
            Assert.IsFalse(double1 != double2);
            Assert.IsFalse(double2 != double1);
            Assert.IsTrue(double1.Equals(double2));
            Assert.IsTrue(double2.Equals(double1));
            Assert.IsTrue(double2.GetHashCode() == double1.GetHashCode());
        }

        [Test]
        public void Test_Equal_And_Hash_Code_Tangens()
        {
            var a1 = MathUtils.ToRadians(23);
            var a2 = MathUtils.ToRadians(111);

            var double1 = (RoundedDouble) Math.Tan(a1 + a2);
            var tan1 = Math.Tan(a1);
            var tan2 = Math.Tan(a2);

            var double2 = (RoundedDouble) ((tan1 + tan2) / (1 - tan1 * tan2));

            Assert.IsTrue(double1 == double2);
            Assert.IsTrue(double2 == double1);
            Assert.IsFalse(double1 != double2);
            Assert.IsFalse(double2 != double1);
            Assert.IsTrue(double1.Equals(double2));
            Assert.IsTrue(double2.Equals(double1));
            Assert.IsTrue(double2.GetHashCode() == double1.GetHashCode());
        }

        [Test]
        public void Test_Equal_And_Hash_Code_Dividing_Doubles()
        {
            const double v1 = 0.7;
            const double v2 = 0.025;
            const double result = v1 / v2;

            var double1 = (RoundedDouble) result;
            var double2 = (RoundedDouble) 28;

            Assert.IsTrue(double1 == double2);
            Assert.IsTrue(double2 == double1);
            Assert.IsFalse(double1 != double2);
            Assert.IsFalse(double2 != double1);
            Assert.IsTrue(double1.Equals(double2));
            Assert.IsTrue(double2.Equals(double1));
            Assert.IsTrue(double2.GetHashCode() == double1.GetHashCode());
        }
    }
}