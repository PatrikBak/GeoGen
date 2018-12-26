using GeoGen.Core;
using System.Collections.Generic;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public class ConstructorHelper
    {
        private readonly ConstructionsContainer _container;

        public ConstructorHelper(ConstructionsContainer container)
        {
            _container = container;
        }

        public ConstructedConfigurationObject CreateCircumcenter(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1]),
                new ObjectConstructionArgument(objects[2]),
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            return new ConstructedConfigurationObject(_container.Get(CircumcenterFromPoints), argumentsList);
        }

        public ConstructedConfigurationObject CreateIntersection(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[1])
                }),
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[2]),
                    new ObjectConstructionArgument(objects[3])
                })
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            return new ConstructedConfigurationObject(_container.Get(IntersectionOfLinesFromPoints), argumentsList);
        }

        public ConstructedConfigurationObject CreateMidpoint(params ConfigurationObject[] objects)
        {
            var argument = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1])
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            return new ConstructedConfigurationObject(_container.Get(MidpointFromPoints), argumentsList);
        }

        public ConstructedConfigurationObject CreateIncenter(params ConfigurationObject[] objects)
        {
            var argumentsList = new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[1]),
                    new ObjectConstructionArgument(objects[2])
                })
            };

            return new ConstructedConfigurationObject(_container.Get("Incenter"), argumentsList);
        }
    }
}