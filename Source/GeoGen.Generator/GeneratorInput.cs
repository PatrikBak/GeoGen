using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an input for the <see cref="IGenerator"/>.
    /// </summary>
    public class GeneratorInput
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
        /// The dictionary mapping object types to the maximal number of objects of given
        /// type that should be present in the generated configurations.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> MaximalObjectCounts { get; }

        /// <summary>
        /// Gets or sets the function that is applied on a valid <see cref="GeneratedConfiguration"/>.
        /// It should return true if and only if this configuration should be extended in the next iteration.
        /// </summary>
        public Func<GeneratedConfiguration, bool> ConfigurationFilter { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration from which the generation process starts. </param>
        /// <param name="constructions">The constructions that are used to create new objects for configurations.</param>
        /// <param name="numberOfIterations">The number of iterations that are to be performed by the generator.</param>
        /// <param name="maximalObjectCounts">The dictionary mapping object types to the maximal number of objects of given type that should be present in the generated configurations.</param>
        /// <param name="configurationFilter">The function for filtrating generated configurations.</param>
        public GeneratorInput(Configuration initialConfiguration,
                              IReadOnlyHashSet<Construction> constructions,
                              int numberOfIterations,
                              IReadOnlyDictionary<ConfigurationObjectType, int> maximalObjectCounts,
                              Func<GeneratedConfiguration, bool> configurationFilter)
        {
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
            Constructions = constructions ?? throw new ArgumentNullException(nameof(constructions));
            NumberOfIterations = numberOfIterations;
            MaximalObjectCounts = maximalObjectCounts ?? throw new ArgumentNullException(nameof(maximalObjectCounts));
            ConfigurationFilter = configurationFilter ?? throw new ArgumentNullException(nameof(configurationFilter));
        }

        #endregion
    }
}