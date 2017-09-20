using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal static class Configurations
    {
        public static ConfigurationWrapper Configuration(int npoints, int nlines, int ncircles)
        {
            var points = Objects(npoints, ConfigurationObjectType.Point);
            var lines = Objects(nlines, ConfigurationObjectType.Line, npoints + 1);
            var circles = Objects(ncircles, ConfigurationObjectType.Circle, npoints + nlines + 1);

            var objects = new HashSet<LooseConfigurationObject>(points.Union(lines).Union(circles));

            var configuration = new Configuration(objects, new List<ConstructedConfigurationObject>());

            var map = new ConfigurationObjectsMap(objects);

            return new ConfigurationWrapper
            {
                Configuration = configuration,
                ConfigurationObjectsMap = map,
                ForbiddenArguments = new Dictionary<int, IArgumentsListContainer>()
            };
        }
    }
}