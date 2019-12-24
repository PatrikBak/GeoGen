using GeoGen.Infrastructure;
using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The settings for the drawer module.
    /// </summary>
    public class DrawerSettings
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
        public DrawingRulesProviderSettings DrawingRulesProviderSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawerSettings"/> class.
        /// </summary>
        /// <param name="metapostDrawerSettings">The settings for the MetaPost drawer.</param>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="drawingRulesProviderSettings">The settings for the provider of drawing rules.</param>
        public DrawerSettings(LoggingSettings loggingSettings,
                              MetapostDrawerSettings metapostDrawerSettings,
                              DrawingRulesProviderSettings drawingRulesProviderSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            MetapostDrawerSettings = metapostDrawerSettings ?? throw new ArgumentNullException(nameof(metapostDrawerSettings));
            DrawingRulesProviderSettings = drawingRulesProviderSettings ?? throw new ArgumentNullException(nameof(drawingRulesProviderSettings));
        }

        #endregion
    }
}
