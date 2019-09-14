using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The list of the loggers settings.
        /// </summary>
        public List<BaseLoggerSettings> Loggers { get; set; }

        /// <summary>
        /// The settings for the <see cref="Pictures"/> used in the algorithm.
        /// </summary>
        public PicturesSettings PicturesManagerSettings { get; set; }

        /// <summary>
        /// The settings of the folder containing inputs.
        /// </summary>
        public InputFolderSettings InputFolderSettings { get; set; }

        /// <summary>
        /// The settings for the algorithm runner.
        /// </summary>
        public AlgorithmRunnerSettings AlgorithmRunnerSettings { get; set; }

        /// <summary>
        /// The settings for the folder containing template theorems.
        /// </summary>
        public TemplateTheoremsFolderSettings TemplateTheoremsFolderSettings { get; set; }

        /// <summary>
        /// The settings for tangent circles theorem finder.
        /// </summary>
        public TangentCirclesTheoremFinderSettings TangentCirclesTheoremFinderSettings { get; set; }

        /// <summary>
        /// The settings for line tangent to circle theorem finder.
        /// </summary>
        public LineTangentToCircleTheoremFinderSettings LineTangentToCircleTheoremFinderSettings { get; set; }

        /// <summary>
        /// The types of theorems that we're looking for.
        /// </summary>
        public IEnumerable<TheoremType> SeekedTheoremTypes { get; set; }
    }
}
