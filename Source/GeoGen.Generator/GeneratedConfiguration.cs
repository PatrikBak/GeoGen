using GeoGen.Core;
using GeoGen.Utilities;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="Configuration"/> that was created by extending 
    /// a <see cref="PreviousConfiguration"/> by a <see cref="ConstructedConfigurationObject"/>.
    /// </summary>
    public class GeneratedConfiguration : Configuration
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the previous configuration that was extended to obtain
        /// this one. This value will be null for the initial configuration.
        /// </summary>
        public GeneratedConfiguration PreviousConfiguration { get; }

        /// <summary>
        /// Gets the index of the iteration on which the configuration was produced, starting with 0.
        /// </summary>
        public int IterationIndex { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedConfiguration"/> class.
        /// </summary>
        /// <param name="currentConfiguration">The configuration that was extended.</param>
        /// <param name="newObject">The new object with which this configuration was extended.</param>
        /// <param name="iterationIndex">The index of the iteration on which the output was produced, starting with 0.</param>
        public GeneratedConfiguration(GeneratedConfiguration currentConfiguration, ConstructedConfigurationObject newObject, int iterationIndex)
            : base(currentConfiguration.LooseObjectsHolder, currentConfiguration.ConstructedObjects.Concat(newObject).ToList())
        {
            PreviousConfiguration = currentConfiguration;
            IterationIndex = iterationIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedConfiguration"/> class 
        /// representing a configuration with no previous configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be wrapped by this object.</param>
        public GeneratedConfiguration(Configuration configuration)
            : base(configuration.LooseObjectsHolder, configuration.ConstructedObjects)
        {
        }

        #endregion
    }
}