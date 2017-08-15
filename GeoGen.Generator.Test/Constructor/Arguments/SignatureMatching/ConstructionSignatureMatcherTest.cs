using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Generator.Constructor.Arguments.SignatureMatching;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace GeoGen.Generator.Test.Constructor.Arguments.SignatureMatching
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

        private ConstructionSignatureMatcher TestMatcher()
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
            Assert.AreEqual(1, (match[0] as ObjectConstructionArgument).PassedObject.Id);
            Assert.AreEqual(2, (match[1] as ObjectConstructionArgument).PassedObject.Id);
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
            Assert.AreEqual(2, set.PassedArguments.Count);

            bool Contains(int id) => set.PassedArguments.Any(e => (e as ObjectConstructionArgument).PassedObject.Id == id);

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
            var sets = (match[0] as SetConstructionArgument).PassedArguments.ToList();
            Assert.AreEqual(2, sets.Count);
            Assert.AreEqual(2, (sets[0] as SetConstructionArgument).PassedArguments.Count);
            Assert.AreEqual(2, (sets[1] as SetConstructionArgument).PassedArguments.Count);

            int Id(int setId, int index)
            {
                return ((sets[setId] as SetConstructionArgument).PassedArguments.ToList()[index] as ObjectConstructionArgument).PassedObject.Id;
            }

            Assert.AreEqual(1, Math.Abs(Id(0, 0) - Id(0, 1)));
            Assert.AreEqual(1, Math.Abs(Id(1, 0) - Id(1, 1)));
        }
    }
}