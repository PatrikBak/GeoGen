using System;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// A helper class that providers methods for converting core objects to human-readable strings.
    /// </summary>
    public static class ToStringHelper
    {
        public static string ObjectToString(Configuration configuration)
        {
            var names = configuration.LooseObjectsHolder.LooseObjects.Cast<ConfigurationObject>()
                .Concat(configuration.ConstructedObjects)
                .Select((o, i) => (o, i))
                .ToDictionary(pair => pair.o, pair => ((char)('A' + pair.i)).ToString());

            return string.Join(Environment.NewLine,
            configuration.LooseObjectsHolder.LooseObjects.Cast<ConfigurationObject>()
                .Concat(configuration.ConstructedObjects)
                .Select(o =>
                {
                    if (o is LooseConfigurationObject loose)
                        return $"{loose.Id}: {names[loose]} = Loose {loose.ObjectType}";

                    var c = o as ConstructedConfigurationObject;

                    return $"{c.Id}: {names[c]} = {c.Construction.Name}({string.Join(",", c.PassedArguments.FlattenedList.Select(x => names[x]))})";
                })

                );
        }
    }
}
