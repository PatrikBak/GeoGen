using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsContainer"/> that uses 
    /// <see cref="StringBasedContainer{T}"/>. The to string conversion for this container is provided
    /// by a given <see cref="IFullConfigurationToStringConverter"/>.
    /// </summary>
    internal class ConfigurationsContainer : StringBasedContainer<ConfigurationWrapper>, IConfigurationsContainer
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="converter">The to string converter that is used by the string based container.</param>
        public ConfigurationsContainer(IFullConfigurationToStringConverter converter)
                : base(converter)
        {
        }

        #endregion

        #region IConfigurationsContainer implementation

        /// <summary>
        /// Adds a given configuration wrapper to the container.
        /// </summary>
        /// <param name="wrapper">The configuration wrapper.</param>
        /// <returns>true, if the content has changed; false otherwise</returns>
        public new bool Add(ConfigurationWrapper wrapper)
        {
            // Delegate the work to the base method
            return base.Add(wrapper);
        }

        /// <summary>
        /// Clears the content of the container.
        /// </summary>
        public void Clear()
        {
            // Clear the content of the string to object dictionary
            Items.Clear();
        }

        #endregion
    }
}