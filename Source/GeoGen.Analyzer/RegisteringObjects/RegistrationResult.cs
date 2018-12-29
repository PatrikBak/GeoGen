using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a result of the registration of a <see cref="Configuration"/> to the geometrical
    /// system of the analyzer module.
    /// </summary>
    public class RegistrationResult
    {
        /// <summary>
        /// Gets or sets if the configuration was successfully registered. 
        /// </summary>
        public bool SuccessfullyDrawn { get; set; }

        /// <summary>
        /// Gets or set the first found pair of objects that turned out to be geometrically the same one.
        /// If there is none, the value should be (null, null).
        /// </summary>
        public (ConfigurationObject olderObject, ConfigurationObject newerObject) Duplicates { get; set; }

        /// <summary>
        /// Gets or set the first object that turned out to be geometrically inconstructible. 
        /// If there is none, the value should be null.
        /// </summary>
        public ConfigurationObject InconstructibleObject { get; set; }        
    }
}
