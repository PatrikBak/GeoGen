using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using Moq;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal static class ConstructionsHelper
    {
        public static ConstructionWrapper Midpoint()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 2}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static ConstructionWrapper Intersection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter
                    (
                        new SetConstructionParameter
                        (
                            new ObjectConstructionParameter(ConfigurationObjectType.Point), 2
                        ), 2
                    )
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 4}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static ConstructionWrapper Projection()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 3}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static ConstructionWrapper CircleCenter()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Circle, 1}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static ConstructionWrapper CircumCircle()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Circle});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 3}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static ConstructionWrapper CrazyConstruction()
        {
            var mock = new Mock<Construction>();
            mock.Setup(s => s.OutputTypes).Returns(new List<ConfigurationObjectType> {ConfigurationObjectType.Point});
            mock.Setup(s => s.ConstructionParameters).Returns
            (
                new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Line), 3),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Circle), 2)
                }
            );

            var dictonary = new Dictionary<ConfigurationObjectType, int>
            {
                {ConfigurationObjectType.Point, 1},
                {ConfigurationObjectType.Line, 3},
                {ConfigurationObjectType.Circle, 3}
            };

            return new ConstructionWrapper
            {
                WrappedConstruction = mock.Object,
                ObjectTypesToNeededCount = dictonary
            };
        }

        public static Construction ConstructionWithId(int id)
        {
            var mock = new Mock<Construction>();

            var output = Enumerable.Repeat(ConfigurationObjectType.Point, 2).ToList();
            mock.Setup(s => s.OutputTypes).Returns(output);

            var result = mock.Object;
            result.Id = id;

            return result;
        }

        public static List<Construction> ConstructionsWithId(int[] ids)
        {
            return ids.Select(ConstructionWithId).ToList();
        }

        public static List<ConstructionWrapper> ConstructionWrappers(int numberOfConstruction, int outputCount)
        {
            return Enumerable.Range(1, numberOfConstruction)
                    .Select
                    (
                        i =>
                        {
                            var mock = new Mock<Construction>();
                            var outputTypes = Enumerable.Repeat(ConfigurationObjectType.Point, outputCount).ToList();
                            mock.Setup(c => c.OutputTypes).Returns(outputTypes);

                            var result = mock.Object;
                            result.Id = i;

                            return result;
                        }
                    )
                    .Select(c => new ConstructionWrapper {WrappedConstruction = c})
                    .ToList();
        }
    }
}