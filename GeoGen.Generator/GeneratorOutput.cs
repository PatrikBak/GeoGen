using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a generator output. 
    /// </summary>
    public class GeneratorOutput
    {
        #region Public properties

        /// <summary>
        /// Temporal output
        /// </summary>
        public Configuration GeneratedConfiguration { get; set; }

        #endregion
    }
}