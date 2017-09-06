using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a generator output. 
    /// 
    /// TODO: Create real output. First we need to figure out what exactly would the statement object look like
    /// </summary>
    public class GeneratorOutput
    {
        #region Public properties

        /// <summary>
        /// Temporal output
        /// </summary>
        public Configuration GeneratedConfiguration { get; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Temporal constructor
        /// </summary>
        /// <param name="generatedConfiguration">The generated configuration.</param>
        public GeneratorOutput(Configuration generatedConfiguration)
        {
            GeneratedConfiguration = generatedConfiguration ?? throw new ArgumentNullException(nameof(generatedConfiguration));
        } 

        #endregion
    }
}