using System;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGeneratorContext"/>.
    /// </summary>
    internal class GeneratorContext : IGeneratorContext
    {
        #region IGeneratorContext properties

        /// <summary>
        /// Gets the configuration container.
        /// </summary>
        public IConfigurationContainer ConfigurationContainer { get; }

        /// <summary>
        /// Gets the configuration handler.
        /// </summary>
        public IConfigurationsHandler ConfigurationsHandler { get; }

        /// <summary>
        /// Gets the configuration constructer.
        /// </summary>
        public IConfigurationConstructor ConfigurationConstructor { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator context.
        /// </summary>
        /// <param name="configurationContainer">The configuration container.</param>
        /// <param name="configurationsHandler">The configurations handler.</param>
        /// <param name="configurationConstructor">The configuration constructor.</param>
        public GeneratorContext(IConfigurationContainer configurationContainer, IConfigurationsHandler configurationsHandler,
            IConfigurationConstructor configurationConstructor)
        {
            ConfigurationContainer = configurationContainer ?? throw new ArgumentNullException(nameof(configurationConstructor));
            ConfigurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(configurationConstructor));
            ConfigurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
        }

        #endregion
    }
}