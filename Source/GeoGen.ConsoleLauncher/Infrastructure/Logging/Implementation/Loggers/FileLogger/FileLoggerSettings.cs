namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Settings of the <see cref="FileLogger"/>
    /// </summary>
    public class FileLoggerSettings : BaseLoggerSettings
    {
        /// <summary>
        /// The path to the file to which we log to.
        /// </summary>
        public string FileLogPath { get; set; }
    }
}
