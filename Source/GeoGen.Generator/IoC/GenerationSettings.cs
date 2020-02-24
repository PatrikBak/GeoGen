namespace GeoGen.Generator
{
    /// <summary>
    /// The settings related to the generator module.
    /// </summary>
    public class GenerationSettings
    {
        #region Public properties

        /// <summary>
        /// The type of configuration filter to be used.
        /// </summary>
        public ConfigurationFilterType ConfigurationFilterType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationSettings"/> class.
        /// </summary>
        /// <param name="configurationFilterType">The type of configuration filter to be used.</param>
        public GenerationSettings(ConfigurationFilterType configurationFilterType)
        {
            ConfigurationFilterType = configurationFilterType;
        }

        #endregion
    }
}
