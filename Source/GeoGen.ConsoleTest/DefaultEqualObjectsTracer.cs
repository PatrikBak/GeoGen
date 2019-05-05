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
                    // Deconstruct the objects
                    var (obj1, obj2) = tuple;

                    // We're going to find the objects that define these including themselves
                    var definingObjects = new[] { obj1, obj2 }.GetDefiningObjects().ToList();

                    // Make sure the equal objects are at the end
                    definingObjects.Remove(obj1);
                    definingObjects.Add(obj1);
                    definingObjects.Remove(obj2);
                    definingObjects.Add(obj2);

                    // Convert them to string
                    //var objectStrings = ToStringHelper.ObjectsToString(definingObjects).ToList();

                    // And use the configuration converted
                    //writer.WriteLine($"{i + 1}. {string.Join(", ", objectStrings.SkipLast(1))}; then {objectStrings.Last()} is equal to {objectStrings[objectStrings.Count - 2]}.");
                });
            }
        }
    }
}