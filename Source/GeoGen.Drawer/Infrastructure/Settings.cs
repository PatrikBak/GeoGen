using GeoGen.Infrastructure;
using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The settings for the drawer module.
    /// </summary>
    public class Settings
    {
        #region Public properties

        /// <summary>
        /// The settings for the logging system.
        /// </summary>
        public LoggingSettings LoggingSettings { get; }

        /// <summary>
        /// The settings for the MetaPost drawer.
        /// </summary>
        public MetapostDrawerSettings MetapostDrawerSettings { get; }

        /// <summary>
        /// The settings for the provider of drawing rules.
        /// </summary>
        public DrawingRuleProviderSettings DrawingRuleProviderSettings { get; }

        /// <summary>
        /// Indicates whether we should reorder objects to get a better picture, i.e. A upwards.
        /// </summary>
        public bool ReorderObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="metapostDrawerSettings">The settings for the MetaPost drawer.</param>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="drawingRuleProviderSettings">The settings for the provider of drawing rules.</param>
        /// <param name="reorderObjects">Indicates whether we should reorder objects to get a better picture, i.e. A upwards.</param>
        public Settings(LoggingSettings loggingSettings,
                        MetapostDrawerSettings metapostDrawerSettings,
                        DrawingRuleProviderSettings drawingRuleProviderSettings,
                        bool reorderObjects)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            MetapostDrawerSettings = metapostDrawerSettings ?? throw new ArgumentNullException(nameof(metapostDrawerSettings));
            DrawingRuleProviderSettings = drawingRuleProviderSettings ?? throw new ArgumentNullException(nameof(drawingRuleProviderSettings));
            ReorderObjects = reorderObjects;
        }

        #endregion
    }
}
