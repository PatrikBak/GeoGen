using System;
using System.IO;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>.
    /// It simply converts each object to string and join these strings using the default
    /// separator. This sealed class is thread-safe.
    /// </summary>
    internal sealed class OldConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private constants

        /// <summary>
        /// The objects separator.
        /// </summary>
        private const string ObjectsSeparator = "|";

        #endregion

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(ConfigurationWrapper configuration, IObjectToStringConverter objectToString)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            var objectStrings = configuration
                    .Configuration
                    .ConstructedObjects
                    .Select(objectToString.ConvertToString)
                    .ToList();

            objectStrings.Sort();

            var r =  string.Join(ObjectsSeparator, objectStrings);

            //sw.WriteLine(r);
            
            return r;
        }

        private static StreamWriter sw;

        static OldConfigurationToStringProvider()
        {
           // sw = new StreamWriter(new FileStream("C:\\Users\\Patrik Bak\\Desktop\\old.txt", FileMode.Create, FileAccess.ReadWrite));
            //sw.AutoFlush = true;
        }

        #endregion
    }
}