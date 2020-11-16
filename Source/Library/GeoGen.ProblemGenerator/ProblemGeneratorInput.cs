using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Represents an input for <see cref="IProblemGenerator"/>. 
    /// </summary>
    public class ProblemGeneratorInput
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
        /// Specifies what types of configurations should be generated, with respect to symmetry.
        /// </summary>
        public SymmetryGenerationMode SymmetryGenerationMode { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration"><inheritdoc cref="InitialConfiguration" path="/summary"/></param>
        /// <param name="constructions"><inheritdoc cref="Constructions" path="/summary"/></param>
        /// <param name="numberOfIterations"><inheritdoc cref="NumberOfIterations" path="/summary"/></param>
        /// <param name="maximalNumbersOfObjectsToAdd"><inheritdoc cref="MaximalNumbersOfObjectsToAdd" path="/summary"/></param>
        /// <param name="symmetryGenerationMode"><inheritdoc cref="SymmetryGenerationMode" path="/summary"/></param>
        public ProblemGeneratorInput(Configuration initialConfiguration,
                                     IReadOnlyHashSet<Construction> constructions,
                                     int numberOfIterations,
                                     IReadOnlyDictionary<ConfigurationObjectType, int> maximalNumbersOfObjectsToAdd,
                                     SymmetryGenerationMode symmetryGenerationMode)
        {
            InitialConfiguration = initialConfiguration ?? throw new ArgumentNullException(nameof(initialConfiguration));
            Constructions = constructions ?? throw new ArgumentNullException(nameof(constructions));
            NumberOfIterations = numberOfIterations;
            MaximalNumbersOfObjectsToAdd = maximalNumbersOfObjectsToAdd ?? throw new ArgumentNullException(nameof(maximalNumbersOfObjectsToAdd));
            SymmetryGenerationMode = symmetryGenerationMode;
        }

        #endregion
    }
}