namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="TemplateTheoremProvider"/> containing information
    /// about the template theorems folder.
    /// </summary>
    public class TemplateTheoremsFolderSettings
    {
        /// <summary>
        /// Gets or sets the path to the folder containing template theorems.
        /// </summary>
        public string TheoremsFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the file extension of all theorem files.
        /// </summary>
        public string FilesExtention { get; set; }
    }
}
