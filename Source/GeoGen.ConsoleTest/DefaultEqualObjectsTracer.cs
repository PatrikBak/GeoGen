using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    public class DefaultEqualObjectsTracer : IEqualObjectsTracer
    {
        private List<(ConfigurationObject, ConfigurationObject)> _objects = new List<(ConfigurationObject, ConfigurationObject)>();

        public void TraceEqualObjects(ConfigurationObject object1, ConfigurationObject object2)
        {
            _objects.Add((object1, object2));
        }

        public void WriteReport(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("This file contains all the pairs of objects that have been resolved as equal ones during the generation.");
                writer.WriteLine();

                _objects.ForEach((tuple, i) =>
                {
                    // We're going to find the objects that define these including themselves
                    var definingObjects = tuple.Item1.GetInternalObjects()
                                                .Concat(tuple.Item2.GetInternalObjects())
                                                .Concat(tuple.Item1)
                                                .Concat(tuple.Item2)
                                                .Distinct()
                                                .ToList();

                    // Sort them according to their ids (so we know which ones were created first)
                    definingObjects.Sort((o1, o2) => o1.Id - o2.Id);

                    // Convert them to string
                    var objectStrings = ToStringHelper.ObjectsToString(definingObjects, displayId: false).ToList();

                    // And use the configuration converted
                    writer.WriteLine($"{i + 1}. {string.Join(", ", objectStrings.SkipLast(1))}; then {objectStrings.Last()} is equal to {(char) ('A' + definingObjects.IndexOf(tuple.Item1))}.");
                });
            }
        }
    }
}