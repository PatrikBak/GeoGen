using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.TheoremRanker.RankedTheoremIO;
using GeoGen.TheoremSorter;
using GeoGen.Utilities;
using Ninject;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GeoGen.OutputMergingLauncher
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry method of the application.
        /// </summary>
        /// <param name="arguments">Expects one argument, the path to the directory with outputs.</param>
        private static void Main(string[] arguments)
        {
            // Get the path to the folder with outputs
            var outputFolder = arguments.FirstOrDefault()
                // Make aware if it's not been set
                ?? throw new InvalidOperationException("Expected one argument, the path to the directory with outputs");

            // Setup Serilog so it logs to console
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // Log the folder
            Log.Information("Exploring folder {outputFolder}", outputFolder);

            // Start timing
            var stopwatch = Stopwatch.StartNew();

            #region Prepare results folder

            // Create the path to the folder with results
            var resultsFolder = "Results";

            // If it exists, delete it
            if (Directory.Exists(resultsFolder))
                Directory.Delete(resultsFolder, recursive: true);

            // Recreate it
            Directory.CreateDirectory(resultsFolder);

            #endregion

            #region Prepare services

            // Prepare kernel
            var kernel = NinjectUtilities.CreateKernel()
                // That uses Theorem Sorter
                .AddTheoremSorter()
                // And Constructor, needed for Theorem Sorter
                .AddConstructor()
                // And Ranked Theorem IO stuff
                .AddRankedTheoremIO();

            // We will need to create theorem sorters 
            var sorterFactory = kernel.Get<ITheoremSorterFactory>();

            // And ranked theorem writers
            var writerFactory = kernel.Get<IRankedTheoremJsonLazyWriterFactory>();

            // And use a reader
            var theoremReader = kernel.Get<IRankedTheoremJsonLazyReader>();

            #endregion

            #region Read all files

            // Local helper that prettifies memory usage
            static string MemoryUsage() => $"{((double)GC.GetTotalMemory(forceFullCollection: true) / 1000000).ToStringWithDecimalDot()} MB";

            // Prepare an unlimited sorter
            var allTheoremSorter = sorterFactory.Create(numberOfTheorems: int.MaxValue);

            // Prepare a counter of processed files
            var counter = 0;

            // Handle every theorem file. They should be .json files
            // We will assume the other non-theorem json files have been removed 
            foreach (var file in Directory.EnumerateFiles(outputFolder, "*.json", SearchOption.AllDirectories))
            {
                // Count in the file
                counter++;

                try
                {
                    // Prepare the enumerable that will read the file
                    var enumeratedTheorems = theoremReader.Read(file);

                    // Add the theorems to the sorter
                    allTheoremSorter.AddTheorems(enumeratedTheorems, out var _);

                    // Log every 20th (so that we don't log too much)
                    if (counter % 20 == 0)
                        Log.Information("Processed {counter} files, total used memory: {memory}", counter, MemoryUsage());
                }
                catch (Exception e)
                {
                    // Log potential problems
                    Log.Error(e, "Cannot parse file {file}", file);
                }
            }

            // Log used memory
            Log.Information("All {counter} files processed, total used memory: {memory}", counter, MemoryUsage());

            #endregion

            #region Split results into types files

            // Prepare the dictionary of writers for individual file types
            var fileDataWriters = new Dictionary<TheoremFileData, IRankedTheoremJsonLazyWriter>();

            // Prepare the paths of theorem files for individual file types
            var fileDataPaths = new Dictionary<TheoremFileData, string>();

            // Go through each theorem, one per configuration
            foreach (var theorem in allTheoremSorter.BestTheorems)
            {
                // Extract data
                var fileData = TheoremFileData.FromRankedTheorem(theorem);

                // Ensure we have a prepared writer 
                var writer = fileDataWriters.GetValueOrCreateAddAndReturn(fileData, () =>
                {
                    // Prepare the path to the file
                    var path = Path.Combine(resultsFolder, $"{fileData}.json");

                    // Remember it
                    fileDataPaths.Add(fileData, path);

                    // Create a new writer
                    var writer = writerFactory.Create(path);

                    // Start writing
                    writer.BeginWriting();

                    // Return it
                    return writer;
                });

                // Write the theorem
                writer.Write(theorem.ToEnumerable());
            }

            // Close all writers
            fileDataWriters.Values.ForEach(writer => writer.EndWriting());

            // Log that we're finished
            Log.Information("Theorems are split into {count} files.", fileDataWriters.Count);

            // We can forget the memory expensive sorter now
            allTheoremSorter = null;

            #endregion

            #region Renaming files to indicate count

            // Prepare the overall number of theorems
            var totalNumberOfTheorems = 0;

            // Go through every file
            foreach (var (theoremFileData, theoremFilePath) in fileDataPaths.ToArray())
            {
                // Find the number of theorems in it
                var numberOfTheorems = theoremReader.Read(theoremFilePath).Count();

                // Count them in
                totalNumberOfTheorems += numberOfTheorems;

                // Get the name without extensions
                var name = Path.GetFileNameWithoutExtension(theoremFilePath);

                // Create the new path
                var newPath = Path.Combine(resultsFolder, $"{name} ({numberOfTheorems}).json");

                // Remember it
                fileDataPaths[theoremFileData] = newPath;

                // Rename the file so that the number of theorems is in the name
                File.Move(theoremFilePath, newPath);
            }

            // Log that we're finished
            Log.Information("The number of theorems is added to each file, the total number is {count}.", totalNumberOfTheorems);

            #endregion

            #region TOP ladders

            #region Base code

            // Prepare the folder for the ladders
            var laddersFolder = Path.Combine(resultsFolder, "Ladders");

            // Create it
            Directory.CreateDirectory(laddersFolder);

            // Prepare the maximal number of written theorems in each ladder 
            var N = 1000;

            // Prepare a local function that writes a ladder
            // This is a slow version, but it works just fine for our demands for N
            void WriteLadder(string id, IEnumerable<TheoremFileData> fileTypes)
            {
                // Create a sorter 
                var sorter = sorterFactory.Create(N);

                // Add at most N theorems from each type
                fileTypes.ForEach(type =>
                {
                    // Read the theorems
                    var theorems = theoremReader.Read(fileDataPaths[type]).Take(N);

                    // Add them to the sorter
                    sorter.AddTheorems(theorems, out var _);
                });

                // Find the number of found theorems
                var numberOfTheorems = sorter.BestTheorems.Count();

                // If there are no theorems, do nothing
                if (numberOfTheorems == 0)
                    return;

                // It might happen that there are not N theorems
                // We will want to have it visible from the file name
                var notEnoughTheoremsNote = numberOfTheorems == N ? "" : $" (only {numberOfTheorems})";

                // Prepare the path to the file we're about to create
                var path = Path.Combine(laddersFolder, $"TOP {N} - {id}{notEnoughTheoremsNote}.json");

                // Write the theorems
                writerFactory.Create(path).WriteEagerly(sorter.BestTheorems);

                // Log that we did it
                Log.Information("Ladder {id} created.", id);
            }

            // We will use the available numbers of objects
            var objectNumbers = fileDataPaths.Keys.Select(data => data.TotalObjects).Distinct().ToHashSet();

            #endregion

            #region Individual ladders

            // Global ladder
            WriteLadder("Global", fileDataPaths.Keys);

            #region Loose object layout

            // Take the layouts
            EnumUtilities.Values<LooseObjectLayout>()
                // For each layout take the corresponding data
                .Select(layout => (layout, enumerable: fileDataPaths.Keys.Where(data => data.Layout == layout)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.layout}", tuple.enumerable));

            #endregion

            #region Theorem type

            // Take the theorem types
            EnumUtilities.Values<TheoremType>()
                // For each type take the corresponding data
                .Select(type => (type, enumerable: fileDataPaths.Keys.Where(data => data.TheoremType == type)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.type}", tuple.enumerable));

            #endregion

            #region Number of objects

            // Take the object numbers
            objectNumbers
                // For each layout take the file data
                .Select(objectNumber => (objectNumber, enumerable: fileDataPaths.Keys.Where(data => data.TotalObjects == objectNumber)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.objectNumber} objects", tuple.enumerable));

            #endregion

            #region Loose object layout, Number of objects

            // Take the layouts
            EnumUtilities.Values<LooseObjectLayout>()
                // Combine with every object number
                .CombinedWith(objectNumbers)
                // For each pair send further the corresponding data
                .Select(pair => (layout: pair.Item1, objectNumber: pair.Item2,
                    // And also the filtered results
                    enumerable: fileDataPaths.Keys.Where(data => data.Layout == pair.Item1 && data.TotalObjects == pair.Item2)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.layout}, {tuple.objectNumber} objects", tuple.enumerable));

            #endregion

            #region Loose object layout, Theorem type

            // Take the layouts
            EnumUtilities.Values<LooseObjectLayout>()
                // Combine with every type
                .CombinedWith(EnumUtilities.Values<TheoremType>())
                // For each pair send further the corresponding data
                .Select(pair => (layout: pair.Item1, type: pair.Item2,
                    // And also the filtered results
                    enumerable: fileDataPaths.Keys.Where(data => data.Layout == pair.Item1 && data.TheoremType == pair.Item2)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.layout}, {tuple.type}", tuple.enumerable));

            #endregion

            #region Theorem type, Number of objects

            // Take the types
            EnumUtilities.Values<TheoremType>()
                // Combine with every object number
                .CombinedWith(objectNumbers)
                // For each pair send further the corresponding data
                .Select(pair => (type: pair.Item1, objectNumber: pair.Item2,
                    // And also the filtered results
                    enumerable: fileDataPaths.Keys.Where(data => data.TheoremType == pair.Item1 && data.TotalObjects == pair.Item2)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.objectNumber} objects, {tuple.type}", tuple.enumerable));

            #endregion

            #region Theorem type, Number of objects, Loose object layout

            // Take the types
            EnumUtilities.Values<TheoremType>()
                // Combine with every object number
                .CombinedWith(objectNumbers)
                // Combine with every layout
                .CombinedWith(EnumUtilities.Values<LooseObjectLayout>())
                // For each pair send further the corresponding data
                .Select(pair => (type: pair.Item1.Item1, objectNumber: pair.Item1.Item2, layout: pair.Item2,
                    // And also the filtered results
                    enumerable: fileDataPaths.Keys.Where(data => data.TheoremType == pair.Item1.Item1 && data.TotalObjects == pair.Item1.Item2 && data.Layout == pair.Item2)))
                // Write each
                .ForEach(tuple => WriteLadder($"{tuple.layout}, {tuple.objectNumber} objects, {tuple.type}", tuple.enumerable));

            #endregion

            #endregion

            #endregion

            // Log how long it all took
            Log.Information("The entire dance took {time} ms.", stopwatch.ElapsedMilliseconds);
        }
    }
}