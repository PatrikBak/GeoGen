using System;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IFullConfigurationToStringConverter"/>.
    /// This class works as an adapter for <see cref="StringBasedContainer{T}"/>, since
    /// it implements the <see cref="IObjectToStringConverter"/> interface. It's adapting
    /// more general <see cref="IConfigurationToStringProvider"/> interface that requires
    /// <see cref="IObjectToStringConverter"/> in order to convert a configuration to string.
    /// This converters are gotten from the <see cref="IFullObjectToStringConvertersFactory"/>
    /// according to the correct minimal resolver of the configuration. Therefore two configuration
    /// that are resolved to be equivalent will be converted to the same string (which is 
    /// what the algorithm wants, since we want to rule out symmetric configurations).
    /// For more information about symmetric configurations, see the documentation of 
    /// <see cref="IMinimalFormResolver"/>.
    /// </summary>
    internal class FullConfigurationToStringConverter : IFullConfigurationToStringConverter
    {
        #region Private fields

        /// <summary>
        /// The factory for getting full object to string converters for the conversion.
        /// </summary>
        private readonly IFullObjectToStringConvertersFactory _factory;

        /// <summary>
        /// The generic configuration to string provider.
        /// </summary>
        private readonly IConfigurationToStringProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="factory">The factory used to find the right object to string converters.</param>
        /// <param name="provider">The generic provider used to delegate the actual conversion to.</param>
        public FullConfigurationToStringConverter(IFullObjectToStringConvertersFactory factory, IConfigurationToStringProvider provider)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region IObjectToString implementation

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The string representation.</returns>
        public string ConvertToString(ConfigurationWrapper item)
        {
            // Let the factory resolve the full object converter
            var objectConverter = _factory.Get(item.ResolverToMinimalForm);

            // Call the generic to string provider to do the job
            return _provider.ConvertToString(item, objectConverter);
        }

        #endregion
    }
}