using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The list of the loggers settings.
        /// </summary>
        public List<BaseLoggerSettings> Loggers { get; set; }
    }
}
