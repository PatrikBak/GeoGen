using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Generator.Constructing.Arguments.SignatureMatching;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching;
using GeoGen.Generator.Test.TestHelpers;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing.Arguments.SignatureMatching
{
    [TestFixture]
    public class ConstructionSignatureMatcherTest
    {
        private static ConstructionSignatureMatcher Matcher()
        {
            return new ConstructionSignatureMatcher();
        }

  

        [Test]
        public void Test_Object_Dictionary_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Matcher().Initialize(null));
        }

        [Test]
        public void Test_Parameters_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Matcher().Match(null));
        }

        [Test]
        public void Test_Not_Enough_Objects_To_Match()
        {
            var matcher = Matcher();

            var testParams = new List<ConstructionParameter>
            {
                new ObjectConstructionParameter(ConfigurationObjectType.Circle),
                new ObjectConstructionParameter(ConfigurationObjectType.Line)
            };

            Assert.Throws<GeneratorException>(() => matcher.Match(testParams));
        }

        [Test]
        public void Test_Signature_Of_Ray()
        {
            var matcher = Matcher();

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
            var matcher = Matcher();

            var midpointParams = new List<ConstructionParameter>
            {
                new SetConstructionParameter
                (
                    new ObjectConstructionParameter(ConfigurationObjectType.Point), 2
                )
            };

            var match = matcher.Match(midpointParams);

            Assert.AreEqual(1, match.Count);
            var set = (SetConstructionArgument) match[0];
            Assert.AreEqual(2, set.PassedArguments.Count);

            bool Contains(int id) => set.PassedArguments.Any(e => ((ObjectConstructionArgument) e).PassedObject.Id == id);

            Assert.IsTrue(Contains(1));
            Assert.IsTrue(Contains(2));
        }

        [Test]
        public void Test_Signature_Of_Intersection()
        {
            var matcher = Matcher();

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
            var sets = ((SetConstructionArgument) match[0]).PassedArguments.ToList();
            Assert.AreEqual(2, sets.Count);
            Assert.AreEqual(2, ((SetConstructionArgument) sets[0]).PassedArguments.Count);
            Assert.AreEqual(2, ((SetConstructionArgument) sets[1]).PassedArguments.Count);

            int Id(int setId, int index)
            {
                var setsArgument = ((SetConstructionArgument) sets[setId]).PassedArguments.ToList()[index];
                var objectArgument = ((ObjectConstructionArgument) setsArgument).PassedObject.Id;

                return objectArgument ?? throw new Exception();
            }

            Assert.AreEqual(1, Math.Abs(Id(0, 0) - Id(0, 1)));
            Assert.AreEqual(1, Math.Abs(Id(1, 0) - Id(1, 1)));
        }
    }
}