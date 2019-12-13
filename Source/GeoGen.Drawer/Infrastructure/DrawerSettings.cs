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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawerSettings"/> class.
        /// </summary>
        /// <param name="metapostDrawerSettings">The settings for the MetaPost drawer.</param>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        public DrawerSettings(LoggingSettings loggingSettings, MetapostDrawerSettings metapostDrawerSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            MetapostDrawerSettings = metapostDrawerSettings ?? throw new ArgumentNullException(nameof(metapostDrawerSettings));
        }

        #endregion
    }
}
