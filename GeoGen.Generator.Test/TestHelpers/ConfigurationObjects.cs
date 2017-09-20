using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal static class ConfigurationObjects
    {
        public static List<LooseConfigurationObject> Objects(int count, ConfigurationObjectType type,
                                                             int startId = 1, bool includeIds = true)
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

        public static ConstructedConfigurationObject ConstructedObject(int constructionId, int index, List<ConstructionArgument> args, int? objectId = null)
        {
            var construction = Constructions.ConstructionWithId(constructionId);

            var result = new ConstructedConfigurationObject(construction, args, index);

            if (objectId.HasValue)
                result.Id = objectId.Value;

            return result;
        }

        public static Configuration AsConfiguration(IEnumerable<LooseConfigurationObject> objects)
        {
            return new Configuration(new HashSet<LooseConfigurationObject>(objects), new List<ConstructedConfigurationObject>());
        }
    }
}