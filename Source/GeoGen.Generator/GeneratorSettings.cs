namespace GeoGen.Generator
{
    /// <summary>
    /// The settings for <see cref="Generator"/>.
    /// </summary>
    public class GeneratorSettings
    {
        #region Public properties

        /// <summary>
        /// The type of configuration filter to be used.
        /// </summary>
        public ConfigurationFilterType ConfigurationFilterType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorSettings"/> class.
        /// </summary>
        /// <param name="configurationFilterType">The type of configuration filter to be used.</param>
        public GeneratorSettings(ConfigurationFilterType configurationFilterType)
        {
            ConfigurationFilterType = configurationFilterType;
        }

        #endregion
    }
}
