using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test
{
    [TestFixture]
    public class GradualAnalyzerTest
    {
        public static bool HasMinimalDifference(double value1, double value2, int units)
        {
            long lValue1 = BitConverter.DoubleToInt64Bits(value1);
            long lValue2 = BitConverter.DoubleToInt64Bits(value2);

            // If the signs are different, return false except for +0 and -0.
            if (lValue1 >> 63 != lValue2 >> 63)
            {
                if (value1 == value2)
                    return true;

                return false;
            }

            long diff = Math.Abs(lValue1 - lValue2);

            if (diff <= units)
                return true;

            return false;
        }

        private static GradualAnalyzer Analyzer()
        {
            var finder = SimpleMock<ITheoremsFinder>();
            var container = SimpleMock<IGeometryHolder>();

            //return new GradualAnalyzer(container, finder);
            return null;
        }


        [Test]
        public void Test_Finder_Cant_Be_Null()
        {
            var container = SimpleMock<IGeometryHolder>();

            //Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(container, null));
        }

        [Test]
        public void Test_Container_Cant_Be_Null()
        {
            var finder = SimpleMock<ITheoremsFinder>();

            //   Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(null, finder));
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Analyzer().Analyze(new List<ConfigurationObject>(), null));
        }
    }
}