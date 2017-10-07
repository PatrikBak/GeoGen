using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Test.TestHelpers;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;
using GeoGen.Core.Utilities.VariationsProviding;
using Moq;
using NUnit.Framework;
using static GeoGen.Analyzer.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Analyzer.Test.Theorems
{
    [TestFixture]
    public class TheoremsFinderTest
    {
        private static TheoremsFinder TheoremsFinder(Dictionary<ConfigurationObjectType, int> signature)
        {
            var verifierMock = new Mock<ITheoremVerifier>();

            verifierMock.Setup(s => s.Signatures).Returns(signature.SingleItemAsEnumerable().ToList);

            verifierMock.Setup(
                s => s.Verify(
                    It.IsAny<ConfigurationObjectsMap>(), It.IsAny<int>(),
                    It.IsAny<IObjectsContainer>())).Returns(true);

            verifierMock.Setup(
                s => s.ConstructTheorem(
                    It.IsAny<ConfigurationObjectsMap>(), It.IsAny<int>())).Returns((Theorem) null);

            var verifiers = new[] {verifierMock.Object};

            var container = ((IObjectsContainer) null).SingleItemAsEnumerable().ToList();

            var holderMock = new Mock<IGeometryHolder>();
            holderMock.Setup(s => s.GetEnumerator()).Returns(container.GetEnumerator());
            var holder = holderMock.Object;

            var variationsProvider = new VariationsProvider<ConfigurationObject>();
            var subsetsProvider = new SubsetsProvider<ConfigurationObject>();

            return new TheoremsFinder(verifiers, holder, subsetsProvider, variationsProvider);
        }

        [TestCase(7, 2, 77 * 720)]
        [TestCase(10, 1, 181440)]
        public void Test_Six_Points_Cocurrency_Signature(int oldCount, int newCount, int expected)
        {
            var oldPoints = ConfigurationObjects.Objects(oldCount, ConfigurationObjectType.Point);
            var newPoints = ConfigurationObjects.Objects(newCount, ConfigurationObjectType.Point, oldCount + 1);

            var oldMap = new ConfigurationObjectsMap(oldPoints);
            var newMap = new ConfigurationObjectsMap(newPoints);

            var signature = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 6}
            };

            var wtf = new Stopwatch();
            wtf.Start();
            var finder = TheoremsFinder(signature);
            wtf.Stop();
            Console.WriteLine(wtf.ElapsedMilliseconds);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = finder.Find(oldMap, newMap).Count();
            stopwatch.Stop();
            Console.WriteLine($"It took fucking {stopwatch.ElapsedMilliseconds}");

            var p = new VariationsProvider<int>();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            for(int i=0; i<252; i++){var count = p.GetVariations(Enumerable.Range(0, 6).ToList(), 6).Count();}
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"Calls {VariationsProvider<ConfigurationObject>.i}");

            //Assert.AreEqual(expected, result);
        }
    }
}