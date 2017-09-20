using System;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;

namespace GeoGen.Generator.ConstructingConfigurations.ConfigurationToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>.
    /// </summary>
    internal class ConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default separator.
        /// </summary>
        private const string DefaultSeparator = "|";

        #endregion

        #region Constructors

        #endregion

        public static int i = 0;

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(Configuration configuration, IObjectToStringProvider objectToString)
        {
            i++;
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            if(LeastConfigurationFinder.finding) s_converting.Start();
            var objectStrings = configuration.ConstructedObjects
                    .Select(objectToString.ConvertToString)
                    .ToList();
            if (LeastConfigurationFinder.finding) s_converting.Stop();

            if (LeastConfigurationFinder.finding) s_sorting.Start();
            objectStrings.Sort();
            if (LeastConfigurationFinder.finding) s_sorting.Stop();

            if (LeastConfigurationFinder.finding) s_joining.Start();
            var t =  string.Join(DefaultSeparator, objectStrings);
            if (LeastConfigurationFinder.finding) s_joining.Stop();

            return t;
        }

        public static Stopwatch s_sorting = new Stopwatch();
        public static Stopwatch s_joining = new Stopwatch();
        public static Stopwatch s_converting = new Stopwatch();

        #endregion
    }
}