using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    public class Test
    {
        public static int a1;
        public static int a2;
        public static long t1;
        public static long t2;
    }

    /// <summary>
    /// A default implementation of <see cref="IConfigurationsManager"/>. This class uses
    /// an <see cref="IConfigurationConstructor"/> for creating configuration wrappers,
    /// an <see cref="IConfigurationsValidator"/> for validating them.
    /// an <see cref="IConfigurationsContainer"/> for recognizing equal configurations.
    /// </summary>
    internal class ConfigurationsManager : IConfigurationsManager
    {
        #region Dependencies

        /// <summary>
        /// The configuration constructor used to create <see cref="ConfigurationWrapper"/>s
        /// </summary>
        private readonly IConfigurationConstructor _configurationConstructor;

        /// <summary>
        /// The configurations container used to keep track of equal configurations.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

        /// <summary>
        /// The validator for the configurations.
        /// </summary>
        private readonly IConfigurationsValidator _configurationsValidator;

        #endregion

        #region Private fields

        /// <summary>
        /// The id of the identified configuration. 
        /// </summary>
        private int _currentConfigurationId;

        #endregion

        #region IConfigurationsManager properties

        /// <summary>
        /// Gets the current layer of unprocessed configurations.
        /// </summary>
        public List<ConfigurationWrapper> CurrentLayer { get; } = new List<ConfigurationWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <param name="configurationConstructor">The constructor of configuration wrappers.</param>
        /// <param name="configurationsContainer">The container for recognizing equal configurations.</param>
        /// <param name="configurationsValidator">The validator of new configurations.</param>
        public ConfigurationsManager
        (
            Configuration initialConfiguration,
            IConfigurationConstructor configurationConstructor,
            IConfigurationsContainer configurationsContainer,
            IConfigurationsValidator configurationsValidator
        )
        {
            _configurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _configurationsValidator = configurationsValidator ?? throw new ArgumentNullException(nameof(configurationsValidator));
            Initialize(initialConfiguration);
        }

        #endregion

        #region IConfigurationContainer methods

        /// <summary>
        /// Processes a new layer of constructors outputs and lazily returns the configurations.
        /// </summary>
        /// <param name="output">The outputs for the new layer.</param>
        /// <returns>The configuration wrappers.</returns>
        public IEnumerable<ConfigurationWrapper> AddLayer(IEnumerable<ConstructorOutput> output)
        {
            // Initialize the list of items that's gonna be the new layer
            var items = new List<ConfigurationWrapper>();

            // Iterate over all constructor outputs
            foreach (var currentOutput in output)
            {
                // Derive the new configuration
                var newConfiguration = currentOutput.OriginalConfiguration.WrappedConfiguration.Derive(currentOutput.ConstructedObjects);

                // Set it's id
                newConfiguration.Id = _currentConfigurationId++;

                // Validate the configuration
                var isCorrect = _configurationsValidator.Validate(newConfiguration);

                // If it's not, skip it
                if (!isCorrect)
                    continue;

                Test.a2++;
                var sw = new Stopwatch();
                sw.Start();
                // Let the constructor create a new wrapper
                var configuration = _configurationConstructor.ConstructWrapper(newConfiguration, currentOutput.OriginalConfiguration);
                sw.Stop();
                Test.t2 += sw.ElapsedMilliseconds; 

                // Add the configuration to the container.
                if (!_configurationsContainer.Add(configuration))
                {
                    // If the content hasn't changed (i.e. the configuration is already present), we skip it
                    continue;
                }

                // Otherwise we have a new correct configuration. We add it to the items list
                items.Add(configuration);

                // And yield it
                yield return configuration;
            }

            // After enumeration we update the current layer list
            CurrentLayer.SetItems(items);

            // And clear the container (the new iteration should yield different configurations, 
            // because the number of their objects will be greater)
            _configurationsContainer.Clear();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the container with a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        private void Initialize(Configuration initialConfiguration)
        {
            // Set the id of the configuration
            initialConfiguration.Id = _currentConfigurationId++;

            // Validate the initial one 
            if (!_configurationsValidator.Validate(initialConfiguration))
            {
                throw new InitializationException("The initial configuration is invalid");
            }

            // Let the constructor construct wrapper for the initial configuration.
            var configurationWrapper = _configurationConstructor.ConstructInitialWrapper(initialConfiguration);

            // Set this configuration as a current layer
            CurrentLayer.SetItems(configurationWrapper.AsEnumerable());
        }

        #endregion
    }
}