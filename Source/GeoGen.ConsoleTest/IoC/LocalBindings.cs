using GeoGen.Constructor;
using GeoGen.Generator;
using Ninject;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// The bindings for this module.
    /// </summary>
    public static class LocalBindings
    {
        /// <summary>
        /// Bindings the dependencies from this module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddLocalBindings(this IKernel kernel)
        {
            kernel.Rebind<IEqualObjectsTracer, DefaultEqualObjectsTracer>().To<DefaultEqualObjectsTracer>().InSingletonScope();
            kernel.Rebind<IInconstructibleObjectsTracer, DefaultInconstructibleObjectsTracer>().To<DefaultInconstructibleObjectsTracer>().InSingletonScope();
            kernel.Rebind<IGeometryConstructionFailureTracer, DefaultGeometryConstructionFailureTracer>().To<DefaultGeometryConstructionFailureTracer>().InSingletonScope();
            kernel.Rebind<IContexualPictureConstructionFailureTracer, DefaultContexualPictureConstructionFailureTracer>().To<DefaultContexualPictureConstructionFailureTracer>().InSingletonScope();
            kernel.Bind<IAlgorithm>().To<SequentialAlgorithm>();
            kernel.Bind<SimpleCompleteTheoremAnalyzer>().ToSelf();

            // Return the kernel for chaining
            return kernel;
        }
    }
}
