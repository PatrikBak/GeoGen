using Ninject;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the TheoremProver module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="proverData">The data for the theorem prover.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddTheoremProver(this IKernel kernel, TheoremProverData proverData)
        {
            // Stateless services
            kernel.Bind<ITheoremProver>().To<TheoremProver>().WithConstructorArgument(proverData);
            kernel.Bind<ISubtheoremDeriver>().To<SubtheoremDeriver>();
            kernel.Bind<ITrivialTheoremProducer>().To<TrivialTheoremProducer>();

            // Derivers
            kernel.Bind<ITheoremDeriver>().To<RectangleDeriver>();
            kernel.Bind<ITheoremDeriver>().To<IncidencesAndCollinearityDeriver>();
            kernel.Bind<ITheoremDeriver>().To<IncidencesAndConcyclityDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ThalesTheoremDeriver>();
            kernel.Bind<ITheoremDeriver>().To<TransitivityDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ParallelogramDeriver>();
            kernel.Bind<ITheoremDeriver>().To<RadicalAxisDeriver>();
            kernel.Bind<ITheoremDeriver>().To<CollinearityWithLinesFromPointsDeriver>();
            kernel.Bind<ITheoremDeriver>().To<PerpendicularLineToParallelLinesDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ExplicitLineWithIncidencesDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ConcyclicPointsWithExplicitCenterDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ConcyclityWithCirclesFromPointsDeriver>();
            kernel.Bind<ITheoremDeriver>().To<ExplicitCircleWithIncidencesDeriver>();
            kernel.Bind<ITheoremDeriver>().To<IsoscelesTrianglesPerpendicularityDeriver>();

            // Tracer
            kernel.Bind<ISubtheoremDeriverGeometryFailureTracer>().To<EmptySubtheoremDeriverGeometryFailureTracer>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}