using GeoGen.Constructor;
using GeoGen.Infrastructure;
using GeoGen.TheoremRanker.RankedTheoremIO;
using Ninject;
using System.Threading.Tasks;

namespace GeoGen.DrawingLauncher
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
        public static async Task InitializeAsync(Settings settings)
        {
            // Initialize the container
            Kernel = Infrastructure.IoC.CreateKernel();

            // Add the logging system
            Kernel.AddLogging(settings.LoggingSettings);

            // And the constructor module
            Kernel.AddConstructor();

            // Bind the rule provider
            Kernel.Bind<IDrawingRuleProvider>().To<DrawingRuleProvider>().WithConstructorArgument(settings.DrawingRuleProviderSettings);

            // Bind the drawer with its settings
            Kernel.Bind<IDrawer>().To<MetapostDrawer>().WithConstructorArgument(settings.MetapostDrawerSettings)
                // And its loaded rules
                .WithConstructorArgument(new MetapostDrawerData(await Kernel.Get<IDrawingRuleProvider>().GetDrawingRulesAsync()));

            // Add the ranked theorem IO
            Kernel.AddRankedTheoremIO();
        }

        #endregion
    }
}