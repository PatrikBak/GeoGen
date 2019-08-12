using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public static class Startup
    {
        public class Clock
        {
            interface IStopwatch
            {
                bool IsRunning { get; }

                TimeSpan Elapsed { get; }

                void Start();

                void Stop();

                void Reset();
            }

            class TimeWatch : IStopwatch
            {
                Stopwatch stopwatch = new Stopwatch();

                public TimeSpan Elapsed
                {
                    get { return stopwatch.Elapsed; }
                }

                public bool IsRunning
                {
                    get { return stopwatch.IsRunning; }
                }
                               
                public TimeWatch()
                {
                    if (!Stopwatch.IsHighResolution)
                        throw new NotSupportedException("Your hardware doesn't support high resolution counter");

                    //prevent the JIT Compiler from optimizing Fkt calls away
                    long seed = Environment.TickCount;

                    //use the second Core/Processor for the test
                    Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

                    //prevent "Normal" Processes from interrupting Threads
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                    //prevent "Normal" Threads from interrupting this thread
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                }

                public void Start()
                {
                    stopwatch.Start();
                }

                public void Stop()
                {
                    stopwatch.Stop();
                }

                public void Reset()
                {
                    stopwatch.Reset();
                }
            }

            class CpuWatch : IStopwatch
            {
                TimeSpan startTime;
                TimeSpan endTime;
                bool isRunning;

                public TimeSpan Elapsed
                {
                    get
                    {
                        if (IsRunning)
                            throw new NotImplementedException("Getting elapsed span while watch is running is not implemented");

                        return endTime - startTime;
                    }
                }

                public bool IsRunning
                {
                    get { return isRunning; }
                }

                public void Start()
                {
                    startTime = Process.GetCurrentProcess().TotalProcessorTime;
                    isRunning = true;
                }

                public void Stop()
                {
                    endTime = Process.GetCurrentProcess().TotalProcessorTime;
                    isRunning = false;
                }

                public void Reset()
                {
                    startTime = TimeSpan.Zero;
                    endTime = TimeSpan.Zero;
                }
            }

            public static void BenchmarkTime(Action action, int iterations = 10000)
            {
                Benchmark<TimeWatch>(action, iterations);
            }

            static void Benchmark<T>(Action action, int iterations) where T : IStopwatch, new()
            {
                //clean Garbage
                GC.Collect();

                //wait for the finalizer queue to empty
                GC.WaitForPendingFinalizers();

                //clean Garbage
                GC.Collect();

                //warm up
                action();

                var stopwatch = new T();
                var timings = new double[5];
                for (int i = 0; i < timings.Length; i++)
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    for (int j = 0; j < iterations; j++)
                        action();
                    stopwatch.Stop();
                    timings[i] = stopwatch.Elapsed.TotalMilliseconds;
                    Console.WriteLine(timings[i]);
                }
                Console.WriteLine("normalized mean: " + timings.NormalizedMean().ToString());
            }

            public static void BenchmarkCpu(Action action, int iterations = 10000)
            {
                Benchmark<CpuWatch>(action, iterations);
            }
        }

        public static double NormalizedMean(this ICollection<double> values)
        {
            if (values.Count == 0)
                return double.NaN;

            var deviations = values.Deviations().ToArray();
            var meanDeviation = deviations.Sum(t => Math.Abs(t.Item2)) / values.Count;
            return deviations.Where(t => t.Item2 > 0 || Math.Abs(t.Item2) <= meanDeviation).Average(t => t.Item1);
        }

        public static IEnumerable<Tuple<double, double>> Deviations(this ICollection<double> values)
        {
            if (values.Count == 0)
                yield break;

            var avg = values.Average();
            foreach (var d in values)
                yield return Tuple.Create(d, avg - d);
        }

        #region Main method

        /// <summary>
        /// The main function.
        /// </summary>
        private static void Main()
        {
            // This makes sure that doubles in the VS debugger will be displayed with a decimal point
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Initialize the IoC system
            RunTaskAndHandleExceptions(() => IoC.InitializeAsync(), exitOnException: true);

            //Clock.BenchmarkTime(() =>
            //{
                // Run the algorithm
                RunTaskAndHandleExceptions(() => IoC.Kernel.Get<IBatchRunner>().FindAllInputFilesAndRunAlgorithmsAsync());
            //}, 
            //iterations: 10);



            // Log that we're done
            LoggingManager.LogInfo("The application has finished.\n");
        }

        #endregion

        #region RunAndHandleExceptions methods

        /// <summary>
        /// Runs the given task and handles all possible exception it may produce.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="exitOnException">Indicates whether we should exist when an exception occurs.</param>
        private static void RunTaskAndHandleExceptions(Func<Task> task, bool exitOnException = false)
        {
            try
            {
                // Run the task and wait for the result
                // 
                // NOTE: Because of calling GetAwaiter().GetResult() instead of Wait(),
                //       the final exception won't be of the type AggregateException
                //
                task().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                // Log it
                LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{e}\n");

                // If we should terminate, do so
                if (exitOnException)
                    Environment.Exit(-1);
            }
        }

        #endregion
    }
}