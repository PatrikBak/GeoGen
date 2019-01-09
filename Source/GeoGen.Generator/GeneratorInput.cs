using GeoGen.Analyzer;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an input for the <see cref="Generator"/>.
    /// </summary>
    public class GeneratorInput : IGeneratorConfiguration, IObjectsContainersManagerConfiguration, ITheoremAnalysisConfiguration, IContextualContainerConfiguration
    {
        #region IGeneratorConfiguration implementation

        /// <summary>
        /// The initial configuration from which the generation process starts. The configuration
        /// shouldn't be identified, nor its internal objects should or constructions should.
        /// </summary>
        public Configuration InitialConfiguration { get; set; }

        /// <summary>
        /// The constructions that are supposed to be used to extend the configurations, including the initial one.
        /// The constructions must have distinct names and cannot be identified.
        /// </summary>
        public IReadOnlyList<Construction> Constructions { get; set; }

        /// <summary>
        /// The number of iterations that are to be performed by the generator.
        /// </summary>
        public int NumberOfIterations { get; set; }

        #endregion

        #region IObjectsContainersManagerConfiguration implementation

        /// <summary>
        /// The number of pictures to which a configuration is drawn and tested for theorems.
        /// </summary>
        public int NumberOfContainers { get; set; }

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.
        /// </summary>
        public int MaximalAttemptsToReconstructAllContainers { get; set; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single objects container.
        /// </summary>
        public int MaximalAttemptsToReconstructOneContainer { get; set; }

        #endregion

        #region ITheoremAnalysisConfiguration implementation

        /// <summary>
        /// The minimal number of containers in which we expect each theorem to be true.
        /// </summary>
        public int MinimalNumberOfTrueContainers { get; set; }

        /// <summary>
        /// The minimal number of containers in which a theorem must be true before we try to re-validate it. 
        /// </summary>
        public int MinimalNumberOfTrueContainersToRevalidate { get; set; }

        #endregion

        #region IContextualContainerConfiguration implementation

        /// <summary>
        /// The maximal allowed number of attempts to reconstruct the contextual container.
        /// </summary>
        public int MaximalNumberOfAttemptsToReconstruct { get; set; }

        #endregion
    }
}