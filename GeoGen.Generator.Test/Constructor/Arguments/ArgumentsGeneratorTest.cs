using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Parameters;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor.Arguments
{
    [TestFixture]
    public class ArgumentsGeneratorTest
    {
        private static Construction Midpoint()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputType).Returns(ConfigurationObjectType.Point);
            mock.Setup(s => s.ConstructionParameters).Returns(new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
            });

            return mock.Object;
        }

        private static Construction Intersection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputType).Returns(ConfigurationObjectType.Point);
            mock.Setup(s => s.ConstructionParameters).Returns(new List<ConstructionParameter>
            {
                new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
            });

            return mock.Object;
        }
    }
}