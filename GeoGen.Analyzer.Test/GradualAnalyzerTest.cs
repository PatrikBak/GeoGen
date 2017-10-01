using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.Utilities;

namespace GeoGen.Analyzer.Test
{
    [TestFixture]
    public class GradualAnalyzerTest
    {
        private static GradualAnalyzer Analyzer()
        {
            var finder = SimpleMock<ITheoremsFinder>();
            var container = SimpleMock<IGeometryHolder>();

            return new GradualAnalyzer(container, finder);
        }

        [Test]
        public void Test_Finder_Cant_Be_Null()
        {
            var container = SimpleMock<IGeometryHolder>();

            Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(container, null));
        }

        [Test]
        public void Test_Container_Cant_Be_Null()
        {
            var finder = SimpleMock<ITheoremsFinder>();

            Assert.Throws<ArgumentNullException>(() => new GradualAnalyzer(null, finder));
        }

        [Test]
        public void Test_Old_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Analyzer().Analyze(null, new List<ConfigurationObject>()));
        }

        [Test]
        public void Test_New_Objects_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Analyzer().Analyze(new List<ConfigurationObject>(), null));
        }
    }
}