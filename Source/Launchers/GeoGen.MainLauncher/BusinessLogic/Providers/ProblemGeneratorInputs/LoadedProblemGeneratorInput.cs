using GeoGen.Core;
using GeoGen.ProblemGenerator;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents an <see cref="ProblemGeneratorInput"/> loaded from a file.
    /// </summary>
    public class LoadedProblemGeneratorInput : ProblemGeneratorInput
    {
        #region Public properties

        /// <summary>
        /// The path from which the input was loaded.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The id of the loaded input.
        /// </summary>
        public string Id { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadedProblemGeneratorInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration from which the generation process starts.</param>
        /// <param name="constructions">The constructions that are used to create new objects for configurations.</param>
        /// <param name="numberOfIterations">The number of iterations that are to be performed by the generator.</param>
        /// <param name="maximalNumbersOfObjectsToAdd">The dictionary representing at most how many objects of each type should be added to the initial configuration.</param>
        /// <param name="filePath">The path from which the input was loaded.</param>
        /// <param name="id">The id of the loaded input.</param>
        public LoadedProblemGeneratorInput(Configuration initialConfiguration,
                                           IReadOnlyHashSet<Construction> constructions,
                                           int numberOfIterations,
                                           IReadOnlyDictionary<ConfigurationObjectType, int> maximalNumbersOfObjectsToAdd,
                                           string filePath,
                                           string id)
            : base(initialConfiguration, constructions, numberOfIterations, maximalNumbersOfObjectsToAdd)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion
    }
}
