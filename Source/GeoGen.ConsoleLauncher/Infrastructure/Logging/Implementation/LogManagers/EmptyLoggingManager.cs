namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents an <see cref="ILoggingManager"/> that does nothing. It's supposed to be
    /// used as a fall-back logging manager that is used when we don't specify a real-one.
    /// </summary>
    public class EmptyLoggingManager : ILoggingManager
    {
        public void Log(string message, LogLevel level, string origin = "", string filePath = "", int lineNumber = 0)
        {
            // Do nothing
        }
    }
}