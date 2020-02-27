using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// Represents an input for the <see cref="IConfigurationGenerator"/>.
    /// </summary>
    public class ConfigurationGeneratorInput
    {
        #region Public properties

        /// <summary>
        /// The initial configuration from which the generation process starts. 
        /// </summary>
        public Configuration InitialConfiguration { get; }

        /// <summary>
        /// The constructions that are used to create new objects for configurations.
        /// </summary>
        public IReadOnlyHashSet<Construction> Constructions { get; }

        /// <summary>
        /// The number of iterations that are to be performed by the generator.
        /// </summary>
        public int NumberOfIterations { get; }

        /// <summary>
        /// The dictionary representing at most how many objects of each type should be added to the initial configuration.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> MaximalNumbersOfObjectsToAdd { get; }

        /// <summary>
        /// Gets or sets the function that is applied on a valid <see cref="GeneratedConfiguration"/>.
        /// It should return true if and only if this configuration should be extended in the next iteration.
        /// </summary>
        public Func<GeneratedConfiguration, bool> ConfigurationFilter { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGeneratorInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration from which the generation process starts. </param>
        /// <param name="constructions">The constructions that are used to create new objects for configurations.</param>
        /// <param name="numberOfIterations">The number of iterations that are to be performed by the generator.</param>
        /// <param name="maximalNumbersOfObjectsToAdd">The dictionary representing at most how many objects of each type should be added to the initial configuration.</param>
        /// <param name="configurationFilter">The function for filtrating generated configurations.</param>
        public ConfigurationGeneratorInput(Configuration initialConfiguration,
                                           IReadOnlyHashSet<Construction> constructions,
                                           int numberOfIterations,
                                           IReadOnlyDictionary<ConfigurationObjectType, int> maximalNumbersOfObjectsToAdd,
                                           Func<GeneratedConfiguration, bool> configurationFilter)
        {
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
            Constructions = constructions ?? throw new ArgumentNullException(nameof(constructions));
            NumberOfIterations = numberOfIterations;
            MaximalNumbersOfObjectsToAdd = maximalNumbersOfObjectsToAdd ?? throw new ArgumentNullException(nameof(maximalNumbersOfObjectsToAdd));
            ConfigurationFilter = configurationFilter ?? throw new ArgumentNullException(nameof(configurationFilter));
        }

        #endregion
    }
}