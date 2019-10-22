using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Utilities;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents an <see cref="AlgorithmInput"/> loaded from a file.
    /// </summary>
    public class LoadedAlgorithmInput : AlgorithmInput
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
        /// Initializes a new instance of the <see cref="LoadedAlgorithmInput"/> class.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration from which the generation process starts.</param>
        /// <param name="constructions">The constructions that are used to create new objects for configurations.</param>
        /// <param name="numberOfIterations">The number of iterations that are to be performed by the generator.</param>
        /// <param name="filePath">The path from which the input was loaded.</param>
        /// <param name="id">The id of the loaded input.</param>
        public LoadedAlgorithmInput(Configuration initialConfiguration, IReadOnlyHashSet<Construction> constructions, int numberOfIterations, string filePath, string id)
            : base(initialConfiguration, constructions, numberOfIterations)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion
    }
}
