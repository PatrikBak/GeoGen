using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Test.TestHelpers
{
    internal static class TheoremsHelper
    {
        public static bool ContainsTheoremWithObjects(this List<Theorem> theorems, params ConfigurationObject[] objects)
        {
            return theorems.Any
            (
                theorem =>
                {
                    return theorem
                            .InvolvedObjects
                            .SelectMany(o => o.InternalObjects)
                            .ToSet()
                            .SetEquals(objects.ToSet());
                }
            );
        }

        public static void PrintVerifiersOutput(IEnumerable<VerifierOutput> correctOutputs)
        {
            foreach (var verifierOutput in correctOutputs)
            {
                foreach (var o in verifierOutput.InvoldedObjects)
                {
                    HashSet<PointObject> points;

                    if (o is LineObject line)
                        points = line.Points;
                    else if (o is CircleObject circle)
                        points = circle.Points;
                    else
                        throw new Exception();

                    if (o.ConfigurationObject != null)
                    {
                        Console.Write($"{o.ConfigurationObject.ObjectType} with id = {o.ConfigurationObject.Id} and points: ");
                    }

                    Console.WriteLine(string.Join(", ", points.Select(p => p.ConfigurationObject.Id)));
                }
                Console.WriteLine();
            }
        }
    }
}