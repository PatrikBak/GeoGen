using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsManager"/>.
    /// </summary>
    internal sealed class ConfigurationsManager : IConfigurationsManager
    {
        #region Private fields

        /// <summary>
        /// The configuration constructor.
        /// </summary>
        private readonly IConfigurationConstructor _configurationConstructor;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

        /// <summary>
        /// The configurations resolver.
        /// </summary>
        private readonly IConfigurationsResolver _configurationsResolver;

        #endregion

        #region IConfigurationContainer properties

        /// <summary>
        /// Gets the current layer of unprocessed configurations
        /// </summary>
        public List<ConfigurationWrapper> CurrentLayer { get; } = new List<ConfigurationWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a configurations manager that is initialized with a given
        /// configuration. It takes care of processing the constructor output.
        /// It uses the configuration objects container for adding new objects,
        /// configuration constructor for constructing configuration wrappers
        /// and configurations container for recognizing equal configurations.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <param name="configurationConstructor">The configuration constructor.</param>
        /// <param name="configurationObjectsContainer">The configuration objects container.</param>
        /// <param name="configurationsContainer">The configurations container.</param>
        public ConfigurationsManager
        (
                Configuration initialConfiguration,
                IConfigurationConstructor configurationConstructor,
                IConfigurationsContainer configurationsContainer,
                IConfigurationsResolver configurationsResolver)
        {
            _configurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _configurationsResolver = configurationsResolver ?? throw new ArgumentNullException(nameof(configurationsResolver));
            Initialize(initialConfiguration);
        }

        #endregion

        #region IConfigurationContainer methods

        /// <summary>
        /// Processes a new layer of constructors outputs and returns the configurations.
        /// </summary>
        /// <param name="output">The new layer output.</param>
        /// <returns>The configuration wrappers.</returns>
        public IEnumerable<ConfigurationWrapper> AddLayer(IEnumerable<ConstructorOutput> output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            // Initialize the list of items that's gonna be the new layer items
            var items = new List<ConfigurationWrapper>();

            // Iterate over all constructor outputs
            foreach (var currentOutput in output)
            {
                // Let the resolver resolve the output and find out if it's correct
                var isCorrect = _configurationsResolver.ResolveNewOutput(currentOutput);

                // If it's not, skip it
                if (!isCorrect)
                    continue;

                // Let the constructor create a new wrapper
                var configuration = _configurationConstructor.ConstructWrapper(currentOutput);

                // Find out if the interior objects have been mapped to other ones
                var isMappedToItself = configuration.ResolverToMinimalForm is IDefaultObjectIdResolver;

                // If we're not mapped to itself, we need to resolve the new configuration as well
                if (!isMappedToItself)
                {
                    // Let the resolve determine correctness
                    isCorrect = _configurationsResolver.ResolveMappedOutput(configuration);

                    // If the configuration is not correct, skip it
                    if (!isCorrect)
                        continue;
                }

                // If everything's fine, then we can add the configuration to the container.
                if (!_configurationsContainer.Add(configuration))
                {
                    // If the content hasn't changed (i.e. the configuration is already present), we skip it
                    continue;
                }

                // Otherwise we have a new configuration. We add it to the items list
                items.Add(configuration);

                // And yield it
                yield return configuration;
            }

            // Update the current layer
            CurrentLayer.SetItems(items);

            // Clear the container (the new iteration should yield different configurations)
            _configurationsContainer.Clear();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the container with a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        private void Initialize(Configuration initialConfiguration)
        {
            if (initialConfiguration == null)
                throw new ArgumentNullException(nameof(initialConfiguration));

            // Let the constructor construct wrapper for the initial configuration.
            var configurationWrapper = _configurationConstructor.ConstructWrapper(initialConfiguration);

            // Set the current layer items
            CurrentLayer.SetItems(configurationWrapper.SingleItemAsEnumerable());
        }

        #endregion
    }
}