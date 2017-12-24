using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Test.TestHelpers;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;
using NUnit.Framework;

namespace GeoGen.Analyzer.Test.Theorems
{
    [TestFixture]
    public class TheoremsContainerTest
    {
        [Test]
        public void Test_Added_Theorem_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TheoremsContainer().Add(null));
        }

        [Test]
        public void Test_Contains_Theorem_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TheoremsContainer().Contains(null));
        }

        [Test]
        public void Test_With_One_Object_Twice()
        {
            var points = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point);
            var lines = ConfigurationObjects.Objects(1, ConfigurationObjectType.Line);

            var theoremObjects = new[]
            {
                new TheoremObject(points, TheoremObjectSignature.LineGivenByPoints),
                new TheoremObject(lines[0])
            };

            var theorem = new Theorem(TheoremType.CollinearPoints, new HashSet<TheoremObject>(theoremObjects));

            var container = new TheoremsContainer {theorem, theorem};

            Assert.IsTrue(container.Contains(theorem));
            Assert.AreEqual(1, container.Count());
        }

        [Test]
        public void Test_Complex()
        {
            var points = ConfigurationObjects.Objects(3, ConfigurationObjectType.Point);
            var lines = ConfigurationObjects.Objects(1, ConfigurationObjectType.Point, 4);
            var circles = ConfigurationObjects.Objects(1, ConfigurationObjectType.Circle, 5);

            var container = new TheoremsContainer();

            var theoremObjects = new[]
            {
                new[]
                {
                    new TheoremObject(points, TheoremObjectSignature.LineGivenByPoints),
                    new TheoremObject(lines[0])
                },
                new[]
                {
                    new TheoremObject(lines[0])
                },
                new[]
                {
                    new TheoremObject(lines[0]),
                    new TheoremObject(circles[0])
                },
                new[]
                {
                    new TheoremObject(circles[0])
                },
                new[]
                {
                    new TheoremObject(points.Skip(1).ToList(), TheoremObjectSignature.LineGivenByPoints)
                },
                new[]
                {
                    new TheoremObject(points.Skip(2).ToList(), TheoremObjectSignature.CircleGivenByPoints)
                },
                new[]
                {
                    new TheoremObject(points.Take(1).ToList(), TheoremObjectSignature.CircleGivenByPoints)
                },
                new[]
                {
                    new TheoremObject(points.Take(1).ToList(), TheoremObjectSignature.LineGivenByPoints)
                }
            };

            var types = Enum.GetValues(typeof(TheoremType)).Cast<TheoremType>().ToList();

            foreach (var theoremType in types)
            {
                foreach (var theoremObject in theoremObjects)
                {
                    var theorem = new Theorem(theoremType, theoremObject.ToSet());

                    var count = container.Count();
                    container.Add(theorem);
                    Assert.IsTrue(count != container.Count());
                    count = container.Count();
                    container.Add(theorem);
                    Assert.AreEqual(count, container.Count());
                }
            }

            Assert.AreEqual(types.Count * 8, container.Count());
        }
    }
}