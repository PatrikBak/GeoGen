using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="TemplateTheoremProvider"/> containing information
    /// about the template theorems folder.
    /// </summary>
    public class TemplateTheoremsFolderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the folder containing template theorems.
        /// </summary>
        public string TheoremsFolderPath { get; }

        /// <summary>
        /// The file extension of all theorem files.
        /// </summary>
        public string FilesExtention { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateTheoremsFolderSettings"/> class.
        /// </summary>
        /// <param name="theoremsFolderPath">The path to the folder containing template theorems.</param>
        /// <param name="filesExtention">The file extension of all theorem files.</param>
        public TemplateTheoremsFolderSettings(string theoremsFolderPath, string filesExtention)
        {
            TheoremsFolderPath = theoremsFolderPath ?? throw new ArgumentNullException(nameof(theoremsFolderPath));
            FilesExtention = filesExtention ?? throw new ArgumentNullException(nameof(filesExtention));
        }

        #endregion
    }
}
