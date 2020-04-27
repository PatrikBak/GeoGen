using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.ProblemGenerator.InputProvider
{
    /// <summary>
    /// Represents an <see cref="ProblemGeneratorInput"/> loaded from a file.
    /// </summary>
    public class LoadedProblemGeneratorInput : ProblemGeneratorInput
    {
        #region Public properties

        /// <summary>
        /// The path from which the input file was loaded.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The id of the loaded input file, i.e. the string after the input file prefix.
        /// </summary>
        public string Id { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadedProblemGeneratorInput"/> class.
        /// </summary>
        /// <inheritdoc cref="ProblemGeneratorInput(Configuration, IReadOnlyHashSet{Construction}, int, IReadOnlyDictionary{ConfigurationObjectType, int}, bool)"/>
        /// <param name="filePath"><inheritdoc cref="FilePath" path="/summary"/></param>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        public LoadedProblemGeneratorInput(Configuration initialConfiguration,
                                           IReadOnlyHashSet<Construction> constructions,
                                           int numberOfIterations,
                                           IReadOnlyDictionary<ConfigurationObjectType, int> maximalNumbersOfObjectsToAdd,
                                           bool excludeAsymmetricConfigurations,
                                           string filePath,
                                           string id)
            : base(initialConfiguration, constructions, numberOfIterations, maximalNumbersOfObjectsToAdd, excludeAsymmetricConfigurations)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion
    }
}
