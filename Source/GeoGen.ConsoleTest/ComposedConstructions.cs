﻿using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public static class ComposedConstructions
    {
        public static ComposedConstruction CentroidFromPoints()
        {
            var objects = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) { Id = i })
                    .ToList();

            var argument1 = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1])
            });
            var argument2 = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[2])
            });

            var argumentsList1 = new List<ConstructionArgument> {argument1};
            var argumentsList2 = new List<ConstructionArgument> {argument2};

            var midpoint1 = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(MidpointFromPoints), argumentsList1) {Id = 3};
            var midpoint2 = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(MidpointFromPoints), argumentsList2) {Id = 4};

            var argument = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[2]),
                    new ObjectConstructionArgument(midpoint1)
                }),
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[1]),
                    new ObjectConstructionArgument(midpoint2)
                })
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            var centroid = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(IntersectionOfLinesFromPoints), argumentsList) {Id = 5};

            var constructedObjects = new List<ConstructedConfigurationObject> {midpoint1, midpoint2, centroid};

            var configuration = new Configuration(objects, constructedObjects);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            return new ComposedConstruction("Centroid", configuration, parameters);
        }

        public static ComposedConstruction IncenterFromPoints()
        {
            var objects = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) { Id = i })
                    .ToList();

            var arguments1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[1]),
                    new ObjectConstructionArgument(objects[2]),
                })
            };
            var arguments2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[1]),
                new SetConstructionArgument(new List<ConstructionArgument>
                {
                    new ObjectConstructionArgument(objects[0]),
                    new ObjectConstructionArgument(objects[2]),
                })
            };
            
            var line1 = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(InternalAngleBisectorFromPoints), arguments1) {Id = 3};
            var line2 = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(InternalAngleBisectorFromPoints), arguments2) {Id = 4};

            var argument = new SetConstructionArgument(new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(line1),
                new ObjectConstructionArgument(line2),
            });

            var argumentsList = new List<ConstructionArgument> {argument};

            var incenter = new ConstructedConfigurationObject(PredefinedConstructionsFactory.Get(IntersectionOfLines), argumentsList) {Id = 5};

            var constructedObjects = new List<ConstructedConfigurationObject> {line1, line2, incenter};

            var configuration = new Configuration(objects, constructedObjects);

            var parameters = new List<ConstructionParameter>
            {
                new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
            };

            return new ComposedConstruction("Incenter", configuration, parameters);
        }
    }
}