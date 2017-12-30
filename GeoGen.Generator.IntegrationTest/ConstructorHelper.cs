using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ConstructorHelper
    {
        private readonly ConstructionsContainer _container;

        public ConstructorHelper(ConstructionsContainer container)
        {
            _container = container;
        }

        public ConstructedConfigurationObject CreateCircumcenter(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1]),
                new ObjectConstructionArgument(objects[2]),
            });

            var argumentsList = new List<ConstructionArgument> { argument };

            return new ConstructedConfigurationObject(_container.Get<CircumcenterFromPoints>(), argumentsList, 0);
        }

        public ConstructedConfigurationObject CreateIntersection(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[1])
                }),
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[2]),
                    new ObjectConstructionArgument(objects[3])
                })
            });

            var argumentsList = new List<ConstructionArgument> { argument };

            return new ConstructedConfigurationObject(_container.Get<IntersectionFromPoints>(), argumentsList, 0);
        }

        public ConstructedConfigurationObject CreateMidpoint(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1])
            });

            var argumentsList = new List<ConstructionArgument> { argument };

            return new ConstructedConfigurationObject(_container.Get<MidpointFromPoints>(), argumentsList, 0);
        }
    }
}
