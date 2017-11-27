using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Test.TestHelpers
{
    internal static class ConfigurationObjects
    {
        public static ConfigurationObject Object(ConfigurationObjectType type, int id)
        {
            return new LooseConfigurationObject(type) { Id = id };
        }

        public static List<LooseConfigurationObject> Objects(int count, ConfigurationObjectType type, int startId = 1, bool includeIds = true)
        {
            var result = Enumerable.Range(startId, count)
                    .Select(
                        i =>
                        {
                            var obj = new LooseConfigurationObject(type);

                            if (includeIds)
                                obj.Id = i;

                            return obj;
                        })
                    .ToList();

            return result;
        }
    }
}