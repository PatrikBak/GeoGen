using GeoGen.Core;
using GeoGen.DependenciesResolver;
using GeoGen.Infrastructure;
using Ninject;
using System.Threading.Tasks;
using static GeoGen.Core.ConfigurationObjectType;
using static GeoGen.Core.LooseObjectsLayout;
using static GeoGen.Core.PredefinedConstructionType;
using static GeoGen.Core.TheoremType;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The class that runs tests of drawing.
    /// </summary>
    public static class DrawerTest
    {
        /// <summary>
        /// The main method.
        /// </summary>
        private static async Task Main()
        {
            #region Prepare IoC

            // Create the kernel with the constructor module
            var kernel = IoC.CreateKernel().AddConstructor();

            // Bind the drawer
            kernel.Bind<IDrawer>().To<MetapostDrawer>();

            #endregion

            // Get the drawer and try run it on some configurations
            await kernel.Get<IDrawer>().DrawAsync(new[]
            {
                MediansAreConcurrent()
            });
        }

        private static (Configuration, Theorem) MediansAreConcurrent()
        {
            // Prepare the objects
            var A = new LooseConfigurationObject(Point);
            var B = new LooseConfigurationObject(Point);
            var C = new LooseConfigurationObject(Point);
            var D = new ConstructedConfigurationObject(Midpoint, B, C);
            var E = new ConstructedConfigurationObject(Midpoint, C, A);
            var F = new ConstructedConfigurationObject(Midpoint, A, B);

            // Prepare the configuration
            var configuration = Configuration.DeriveFromObjects(Triangle, D, E, F);

            // Prepare the theorem
            var theorem = new Theorem(ConcurrentLines, new[]
            {
                new LineTheoremObject(A, D),
                new LineTheoremObject(B, E),
                new LineTheoremObject(C, F)
            });

            // Return them
            return (configuration, theorem);
        }
    }
}
