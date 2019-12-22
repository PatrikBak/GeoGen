using Ninject;
using Ninject.Extensions.Factory;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The extension methods for <see cref="IKernel"/>.
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Bindings for the dependencies from the Constructor module.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <returns>The kernel for chaining.</returns>
        public static IKernel AddConstructor(this IKernel kernel)
        {
            // Stateless services
            kernel.Bind<IGeometryConstructor>().To<GeometryConstructor>();
            kernel.Bind<IConstructorsResolver>().To<ConstructorsResolver>();

            // Stateless predefined constructors
            kernel.Bind<IPredefinedConstructor>().To<CenterOfCircleConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<CircleWithCenterThroughPointConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<CircumcircleConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<InternalAngleBisectorConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<IntersectionOfLinesConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<LineFromPointsConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<MidpointConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularLineConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<ParallelLineConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PerpendicularProjectionConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<PointReflectionConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfCircleAndLineFromPointsConstructor>();
            kernel.Bind<IPredefinedConstructor>().To<SecondIntersectionOfTwoCircumcirclesConstructor>();

            // Factories
            kernel.Bind<IComposedConstructorFactory>().ToFactory();

            // Factory outputs
            kernel.Bind<IComposedConstructor>().To<ComposedConstructor>();

            // Return the kernel for chaining
            return kernel;
        }
    }
}