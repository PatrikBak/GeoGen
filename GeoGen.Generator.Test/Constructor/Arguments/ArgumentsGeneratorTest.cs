using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities.Variations;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructor.Arguments
{
    [TestFixture]
    public class ArgumentsGeneratorTest
    {
        private void Maybe()
        {
            var variationsProvider = new Mock<IVariationsProvider<ConfigurationObject>>();
            variationsProvider.Setup(s => s.GetVariations(It.IsAny<List<ConfigurationObject>>(), It.Is<int>(i => i == 2)))
                .Returns<List<ConfigurationObject>, int>((list, i) => new List<IEnumerable<ConfigurationObject>>
                {
                    new List<ConfigurationObject> {list[0], list[1], list[2]},
                    new List<ConfigurationObject> {list[0], list[2], list[1]},
                    new List<ConfigurationObject> {list[1], list[0], list[2]},
                    new List<ConfigurationObject> {list[1], list[2], list[0]},
                    new List<ConfigurationObject> {list[2], list[1], list[0]},
                    new List<ConfigurationObject> {list[2], list[0], list[1]}
                });
        }

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