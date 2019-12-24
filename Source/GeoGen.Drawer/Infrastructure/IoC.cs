using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using Ninject;
using System.Linq;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a static initializer of the dependency injection system.
    /// </summary>
    public static class IoC
    {
        #region Kernel

        /// <summary>
        /// Gets the dependency injection container.
        /// </summary>
        public static IKernel Kernel { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the <see cref="Kernel"/> and all the dependencies.
        /// </summary>
        /// <param name="settings">The settings of the application.</param>
        public static async Task InitializeAsync(DrawerSettings settings)
        {
            // Initialize the container
            Kernel = Infrastructure.IoC.CreateKernel();

            // Add the logging system
            Kernel.AddLogging(settings.LoggingSettings);

            // And the constructor module
            Kernel.AddConstructor();

            // Bind the rules provider
            Kernel.Bind<IDrawingRulesProvider>().To<DrawingRulesProvider>().WithConstructorArgument(settings.DrawingRulesProviderSettings);

            // Bind the drawer with its settings
            Kernel.Bind<IDrawer>().To<MetapostDrawer>().WithConstructorArgument(settings.MetapostDrawerSettings)
                // And its data
                .WithConstructorArgument(new MetapostDrawerData
                (
                    // Loaded via the drawing rules provider
                    (await Kernel.Get<IDrawingRulesProvider>().GetDrawingRulesAsync()).ToDictionary(rule => rule.ObjectToDraw.Construction, rule => rule)
                ));
        }

        #endregion
    }
}