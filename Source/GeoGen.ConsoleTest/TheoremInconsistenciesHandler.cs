using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    public class TheoremInconsistenciesHandler
    {
        private Dictionary<Configuration, SortedSet<string>> _map = new Dictionary<Configuration, SortedSet<string>>();

        public List<string> AddAndReturnInconsistencies(GeneratorOutput output)
        {
            // Pull the configuration
            var configuration = output.Configuration;

            // Create a formatter for it
            var formatter = new OutputFormatter(configuration);

            // Convert the theorems
            var theorems = output.AnalyzerOutput.NumberOfTrueContainers
                .Where(pair => pair.Item2 == Program.Containers)
                .Select(pair => pair.Item1)
                .Select(formatter.ConvertTheoremToString).ToSortedSet();

            // If we doesn't have it yet...
            if (!_map.ContainsKey(configuration))
            {
                // Then we register the theorems to it
                _map.Add(configuration, theorems);

                // And we can't do anything else
                return new List<string>();
            }

            // Otherwise we already have those theorems (this is at least the second run)
            var originalTheorems = _map[configuration];

            // Let's find and return the differences
            var t = originalTheorems.Except(theorems).Concat(theorems.Except(originalTheorems)).ToList();

            if(t.Count != 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Old ones");
                Console.WriteLine();
                Console.WriteLine(new OutputFormatter(_map.Keys.First(k => k.Id == configuration.Id)).FormatConfiguration());
                Console.WriteLine();
                Console.WriteLine(string.Join("\n", originalTheorems));
                Console.WriteLine();
                Console.WriteLine("New ones");
                Console.WriteLine();
                Console.WriteLine(formatter.FormatConfiguration());
                Console.WriteLine();
                Console.WriteLine(string.Join("\n", theorems));

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            return t;
        }
    }
}
