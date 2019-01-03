//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using GeoGen.AnalyticGeometry;
//using FluentAssertions;

//namespace GeoGen.AnalyticGeometry.Tests
//{
//    /// <summary>
//    /// The test class for <see
//    /// </summary>
//    [TestFixture]
//    public class DoubleExtentionsTest
//    {
//        #region Helper method

//        /// <summary>
//        /// Parses a binary string to a double, for example: 1.1 becomes 
//        /// a double with the decimal value 1.5.
//        /// </summary>
//        /// <param name="binaryString">The binary string to be parsed.</param>
//        /// <returns>The parsed double.</returns>
//        private static double ParseBinaryString(string binaryString)
//        {
//            // Get the decimal point index
//            var pointIndex = binaryString.IndexOf('.');

//            // If there's none, pretend it's at the end
//            if (pointIndex == -1)
//                pointIndex = binaryString.Length;               

//            // Check the minus sign
//            var isNegative = binaryString[0] == '-';

//            // Get the starting index
//            var startIndex = isNegative ? 1 : 0;

//            // Prepare the result
//            var result = 0d;

//            // Add the numbers on the left from the point
//            for (var i = startIndex; i < pointIndex; i++)
//                if (binaryString[i] == '1')
//                    result += 1 << (pointIndex - i - 1);

//            // Add the numbers on the right from the point
//            for (var i = pointIndex + 1; i < binaryString.Length; i++)
//                if (binaryString[i] == '1')
//                    result += 1.0 / (1 << (i - pointIndex));

//            // If it's negative, negate it...
//            if (isNegative)
//                result = -result;

//            // Return the result
//            return result;
//        }

//        #endregion

//        #region Rounding test

//        [Test]
//        public void Test_Rounding_Regular_Numbers()
//        {
//            // The number of binary places to which we round
//            const int binaryPlaces = 4;

//            // Test integers (they shouldn't be rounded at all)
//            ParseBinaryString("1").Rounded(binaryPlaces).Should().Be(ParseBinaryString("1"));
//            ParseBinaryString("0").Rounded(binaryPlaces).Should().Be(ParseBinaryString("0"));
//            ParseBinaryString("10").Rounded(binaryPlaces).Should().Be(ParseBinaryString("10"));
//            ParseBinaryString("10101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("10101"));
//            ParseBinaryString("-1").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-1"));
//            ParseBinaryString("-0").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-0"));
//            ParseBinaryString("-10").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-10"));
//            ParseBinaryString("-10101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-10101"));

//            // Test numbers with few places (they shouldn't be rounded either)
//            ParseBinaryString("0.1").Rounded(binaryPlaces).Should().Be(ParseBinaryString("0.1"));
//            ParseBinaryString("1.01").Rounded(binaryPlaces).Should().Be(ParseBinaryString("1.01"));
//            ParseBinaryString("11.111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("11.111"));
//            ParseBinaryString("111.0111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("111.0111"));
//            ParseBinaryString("-0.1").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-0.1"));
//            ParseBinaryString("-1.01").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-1.01"));
//            ParseBinaryString("-11.111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-11.111"));
//            ParseBinaryString("-111.0111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-111.0111"));

//            // Test numbers that should be rounded
//            ParseBinaryString("1.1010101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("1.101"));
//            ParseBinaryString("0.11110101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("0.1111"));
//            ParseBinaryString("101.1010111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("101.101"));
//            ParseBinaryString("101.110101001").Rounded(binaryPlaces).Should().Be(ParseBinaryString("101.1101"));
//            ParseBinaryString("-1.1010101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-1.101"));
//            ParseBinaryString("-0.11110101").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-0.1111"));
//            ParseBinaryString("-101.1010111").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-101.101"));
//            ParseBinaryString("-101.110101001").Rounded(binaryPlaces).Should().Be(ParseBinaryString("-101.1101"));
//        }

//        [Test]
//        public void Test_Rounding_With_Many_Decimal_Places()
//        {
//            // Try 5,6 to 1000 places
//            for (var numberOfPlaces = 5; numberOfPlaces < 1000; numberOfPlaces++)
//            {
//                ParseBinaryString("1.0001").Rounded(numberOfPlaces).Should().Be(ParseBinaryString("1.0001"));
//                ParseBinaryString("111111111111101.0001").Rounded(numberOfPlaces).Should().Be(ParseBinaryString("111111111111101.0001"));
//                ParseBinaryString("-1.0001").Rounded(numberOfPlaces).Should().Be(ParseBinaryString("-1.0001"));
//                ParseBinaryString("-111111111111101.0001").Rounded(numberOfPlaces).Should().Be(ParseBinaryString("-111111111111101.0001"));

//            }
//        }

//        [Test]
//        public void Test_Rounding_With_Zero_Places()
//        {
//            ParseBinaryString("1.1").Rounded(0).Should().Be(ParseBinaryString("1"));
//            ParseBinaryString("1.11").Rounded(0).Should().Be(ParseBinaryString("1"));
//            ParseBinaryString("11.1").Rounded(0).Should().Be(ParseBinaryString("11"));
//            ParseBinaryString("1101.0001").Rounded(0).Should().Be(ParseBinaryString("1101"));
//            ParseBinaryString("101010101011.11").Rounded(0).Should().Be(ParseBinaryString("101010101011"));
//        }

//        #endregion
//    }
//}
