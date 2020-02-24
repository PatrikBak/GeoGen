using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents an input for <see cref="IAlgorithm"/>. 
    /// </summary>
    public class AlgorithmInput
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration from which the generation process starts.</param>
        /// <param name="constructions">The constructions that are used to create new objects for configurations.</param>
        /// <param name="numberOfIterations">The number of iterations that are to be performed by the generator.</param>
        /// <param name="maximalObjectCounts">The dictionary mapping object types to the maximal number of objects of given type that should be present in the generated configurations.</param>
        public AlgorithmInput(Configuration initialConfiguration,
                              IReadOnlyHashSet<Construction> constructions,
                              int numberOfIterations,
                              IReadOnlyDictionary<ConfigurationObjectType, int> maximalObjectCounts)
        {
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
            Constructions = constructions ?? throw new ArgumentNullException(nameof(constructions));
            NumberOfIterations = numberOfIterations;
            MaximalObjectCounts = maximalObjectCounts ?? throw new ArgumentNullException(nameof(maximalObjectCounts));
        }

        #endregion
    }
}