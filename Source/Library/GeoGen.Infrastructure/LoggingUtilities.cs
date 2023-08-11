using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Static logging utilities. 
    /// </summary>
    public static class LoggingUtilities
    {
        /// <summary>
        /// Sets up the global Serilog logger, specifically <see cref="Log.Logger"/>.
        /// </summary>
        /// <param name="configuration">Configuration where there should be logging settings for Serilog under the property name 'Serilog'.</param>
        public static void SetupSerilog(JsonConfiguration configuration)
        {
            // Serilog requires .NET Core Configuration. We will simulate one
            // Retrieve the configuration settings string and artificially add the property name again
            var serilogSettingsString = $"{{\"Serilog\": {configuration.GetSettingsAsJson("Serilog")}}}";

            // The .NET Core API does not provide an API where we can just pass a JSON string
            // Fortunately, we can pass a JSON stream
            using var serilogSettingsMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serilogSettingsString));

            // Now we can build the configuration
            var dotNetCoreConfiguration = new ConfigurationBuilder().AddJsonStream(serilogSettingsMemoryStream).Build();

            // And we can setup the logging using this configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(dotNetCoreConfiguration).CreateLogger();
        }
    }
}