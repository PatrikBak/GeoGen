using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.SignatureMatching
{
    [TestFixture]
    public class ConstructionSignatureMatcherTest
    {
        private static LooseConfigurationObject NextObject(int id)
        {
            return new LooseConfigurationObject(ConfigurationObjectType.Point)
            {
                Id = id
            };
        }

        private static ConstructionSignatureMatcher TestMatcher()
        {
            var id = 1;
            var matcher = new ConstructionSignatureMatcher();

            matcher.Initialize(new Dictionary<ConfigurationObjectType, List<ConfigurationObject>>
            {
                {
                    ConfigurationObjectType.Point, new List<ConfigurationObject>
                    {
                        NextObject(id++),
                        NextObject(id++),
                        NextObject(id++),
                        NextObject(id++),
                        NextObject(id)
                    }
                }
            });

            return matcher;
        }

        [Test]
        public void Object_Dictionary_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => TestMatcher().Match(null));
        }

        [Test]
        public void Test_Signature_Of_Ray()
        {
            var matcher = TestMatcher();

            var rayParams = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Point),
                new ObjectConstructionParameter(ConfigurationObjectType.Point)
            };

            var match = matcher.Match(rayParams);

            Assert.AreEqual(2, match.Count);
            Assert.AreEqual(1, ((ObjectConstructionArgument) match[0]).PassedObject.Id);
            Assert.AreEqual(2, ((ObjectConstructionArgument) match[1]).PassedObject.Id);
        }

        [Test]
        public void Test_Signature_Of_Midpoint()
        {
            var matcher = TestMatcher();

            var midpointParams = new List<ConstructionParameter>
            {
                new SetConstructionParameter
                (
                    new ObjectConstructionParameter(ConfigurationObjectType.Point), 2
                )
            };

            var match = matcher.Match(midpointParams);

            Assert.AreEqual(1, match.Count);
            var set = match[0] as SetConstructionArgument ?? throw new NullReferenceException();
            Assert.AreEqual(2, set.PassableArguments.Count);

            bool Contains(int id) => set.PassableArguments.Any(e => ((ObjectConstructionArgument) e).PassedObject.Id == id);

            Assert.IsTrue(Contains(1));
            Assert.IsTrue(Contains(2));
        }

        [Test]
        public void Test_Signature_Of_Intersection()
        {
            var matcher = TestMatcher();

            var midpointParams = new List<ConstructionParameter>
            {
                new SetConstructionParameter
                (
                    new SetConstructionParameter
                    (
                        new ObjectConstructionParameter(ConfigurationObjectType.Point), 2
                    ), 2
                )
            };

            var match = matcher.Match(midpointParams);

            Assert.AreEqual(1, match.Count);
            var sets = ((SetConstructionArgument) match[0]).PassableArguments.ToList();
            Assert.AreEqual(2, sets.Count);
            Assert.AreEqual(2, ((SetConstructionArgument) sets[0]).PassableArguments.Count);
            Assert.AreEqual(2, ((SetConstructionArgument) sets[1]).PassableArguments.Count);

            int Id(int setId, int index)
            {
                return ((ObjectConstructionArgument) ((SetConstructionArgument) sets[setId]).PassableArguments.ToList()[index]).PassedObject.Id;
            }

            Assert.AreEqual(1, Math.Abs(Id(0, 0) - Id(0, 1)));
            Assert.AreEqual(1, Math.Abs(Id(1, 0) - Id(1, 1)));
        }
    }
}