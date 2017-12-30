using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Constructions.PredefinedConstructions;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ComposedConstructions
    {
        private readonly ConstructionsContainer _container;

        public ComposedConstructions(ConstructionsContainer contaier)
        {
            _container = contaier;
        }

        public ComposedConstruction AddCentroidFromPoints()
        {
            var objects = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
                    .ToList();

            var argument1 = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1])
            });
            var argument2 = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[2])
            });

            var argumentsList1 = new List<ConstructionArgument> {argument1};
            var argumentsList2 = new List<ConstructionArgument> {argument2};

            var midpoint1 = new ConstructedConfigurationObject(_container.Get<MidpointFromPoints>(), argumentsList1, 0) {Id = 3};
            var midpoint2 = new ConstructedConfigurationObject(_container.Get<MidpointFromPoints>(), argumentsList2, 0) {Id = 4};

            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[2]),
                    new ObjectConstructionArgument(midpoint1)
                }),
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[1]),
                    new ObjectConstructionArgument(midpoint2)
                })
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            var centroid = new ConstructedConfigurationObject(_container.Get<IntersectionFromPoints>(), argumentsList, 0) {Id = 5};

            var constructedObjects = new List<ConstructedConfigurationObject> {midpoint1, midpoint2, centroid};

            var configuration = new Configuration(objects, constructedObjects);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            var result = new ComposedConstruction(configuration, new List<int> {2}, parameters) {Name = "Centroid"};

            _container.Add(result);

            return result;
        }

        public ComposedConstruction AddIncenterFromPoints()
        {
            var objects = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
                    .ToList();

            var arguments1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[1]),
                    new ObjectConstructionArgument(objects[2]),
                })
            };
            var arguments2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[1]),
                new SetConstructionArgument(new HashSet<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[2]),
                })
            };
            
            var line1 = new ConstructedConfigurationObject(_container.Get<InternalAngelBisectorFromPoints>(), arguments1, 0) {Id = 3};
            var line2 = new ConstructedConfigurationObject(_container.Get<InternalAngelBisectorFromPoints>(), arguments2, 0) {Id = 4};

            var argument = new SetConstructionArgument(new HashSet<ConstructionArgument>
            {
                new ObjectConstructionArgument(line1),
                new ObjectConstructionArgument(line2),
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            var incenter = new ConstructedConfigurationObject(_container.Get<IntersectionFromLines>(), argumentsList, 0) {Id = 5};

            var constructedObjects = new List<ConstructedConfigurationObject> {line1, line2, incenter};

            var configuration = new Configuration(objects, constructedObjects);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            var result = new ComposedConstruction(configuration, new List<int> {2}, parameters) { Name = "Incenter" };;

            _container.Add(result);

            return result;
        }
    }
}