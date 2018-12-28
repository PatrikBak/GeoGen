using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Runner;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static GeoGen.Core.PredefinedConstructionType;

namespace GeoGen.ConsoleTest
{
    public class Program
    {
        private static IKernel _kernel;

        private static ConstructionsContainer _constructionsContainer => _kernel.Get<ConstructionsContainer>();

        private static ComposedConstructions _composedConstructions => _kernel.Get<ComposedConstructions>();

        private static ConstructorHelper _constructorHelper => _kernel.Get<ConstructorHelper>();

        private static ConsoleInconsistenciesTracker _tracker => _kernel.Get<IInconsistenciesTracker>() as ConsoleInconsistenciesTracker;

        private static void Bootstrap()
        {
            _kernel = new StandardKernel(new FuncModule(), new ContextPreservationModule());
            _kernel.Components.RemoveAll<IMissingBindingResolver>();
            _kernel.Settings.AllowNullInjection = true;
            _kernel.AddAlgorithm();

            _kernel.Bind<ConstructionsContainer>().ToSelf().InSingletonScope();
            _kernel.Bind<ComposedConstructions>().ToSelf().InSingletonScope();
            _kernel.Bind<ConstructorHelper>().ToSelf().InSingletonScope();

            _kernel.Bind<IInconsistenciesTracker>().ToConstant(new ConsoleInconsistenciesTracker());
            _kernel.Bind<IEqualObjectsTracer>().ToConstant(new ConsoleEqualObjectsTracer());
            _kernel.Bind<IInconstructibleObjectsTracer>().ToConstant(new ConsoleInconstructibleObjectsTracer());
            //_kernel.Rebind<ITheoremsAnalyzer>().To<DummyTheoremsAnalyzer>();
        }

        private static void Main()
        {
            Bootstrap();

            var points = Enumerable.Range(0, 3)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point))
                    .ToList();

            var constructedObjects = ConstructedObjects(points);

            var configuration = new Configuration(points, constructedObjects, LooseObjectsLayout.ScaleneAcuteAngledTriangled);
            var constructions = Constructions();

            var input = new GeneratorInput
            {
                InitialConfiguration = configuration,
                Constructions = constructions,
                MaximalNumberOfIterations = 2
            };

            var generator = _kernel.Get<IGeneratorFactory>().CreateGenerator(input);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = generator.Generate().ToList();
            stopwatch.Stop();

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Generated: {result.Count}");
            Console.WriteLine($"Generated with theorems: {result.Count(r => r.Theorems.Any())}");
            Console.WriteLine($"Total number of theorems: {result.Sum(output => output.Theorems.Count)}");
            Console.WriteLine($"Inconsistencies: {_tracker.Inconsistencies}");
            Console.WriteLine($"Failed attempts to reconstruct: {_tracker.AttemptsToReconstruct}");
            Console.WriteLine($"-------------------------------------------------");
            Console.ReadKey();

            PrintTheorems(result);
        }

        private static List<Construction> Constructions()
        {
            return new List<Construction>
            {
                _composedConstructions.AddIncenterFromPoints(),
                _composedConstructions.AddCentroidFromPoints(),
                _constructionsContainer.Get(IntersectionOfLinesFromPoints),
                _constructionsContainer.Get(IntersectionOfLinesFromLineAndPoints),
                _constructionsContainer.Get(IntersectionOfLines),
                _constructionsContainer.Get(MidpointFromPoints),
                _constructionsContainer.Get(CircumcenterFromPoints),
                _constructionsContainer.Get(PerpendicularLineFromPoints),
                _constructionsContainer.Get(InternalAngleBisectorFromPoints),
            };
        }

        private static List<ConstructedConfigurationObject> ConstructedObjects(List<LooseConfigurationObject> points)
        {
            //_composedConstructions.AddIncenterFromPoints();

            //var o = _constructorHelper.CreateCircumcenter(points[0], points[1], points[2]);
            //var i = _constructorHelper.CreateIncenter(points[0], points[1], points[2]);
            //var p = _constructorHelper.CreateIntersection(o, i, points[0], points[1]);

            return new List<ConstructedConfigurationObject>
            {
                //  o
                //i
            };
        }

        private static void PrintTheorems(IEnumerable<GeneratorOutput> result)
        {
            var formatter = new OutputFormatter(_constructionsContainer);

            Console.WriteLine("Results:\n");

            var i = 1;

            foreach (var generatorOutput in result.Where(t => t.Theorems.Any()))
            {
                Console.Clear();
                Console.WriteLine($"{i++}.");
                Console.WriteLine("-------------------\n");
                Console.WriteLine(formatter.Format(generatorOutput.Configuration));
                Console.WriteLine("-------------------\n");
                Console.WriteLine("Theorems:");
                Console.WriteLine(formatter.Format(generatorOutput.Theorems));
                Console.ReadKey(true);
            }
        }
    }
}