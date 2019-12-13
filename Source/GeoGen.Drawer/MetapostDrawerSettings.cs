using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The settings for <see cref="MetapostDrawer"/>.
    /// </summary>
    public class MetapostDrawerSettings
    {
        #region Public properties

        /// <summary>
        /// The data with MetaPost-related commands.
        /// </summary>
        public MetapostDrawingData DrawingData { get; }

        /// <summary>
        /// The path to the file that will be created and then compiled.
        /// </summary>
        public string MetapostCodeFilePath { get; }

        /// <summary>
        /// The relative or absolute path to the MetaPost library that is loaded at the end of the created file.
        /// </summary>
        public string MetapostMacrosLibraryPath { get; }

        /// <summary>
        /// The command used to compile the created MetaPost file. The path to the created file will be appended 
        /// to the end of the arguments.
        /// </summary>
        public (string program, string arguments) CompilationCommand { get; }

        /// <summary>
        /// The command that will be called after the compilation, if it's not null. It will get the number of 
        /// the generated pictures as an argument.
        /// </summary>
        public string PostcompilationCommand { get; }

        /// <summary>
        /// Indicates whether we should log the output provided by the compilation and post-compilation command.
        /// </summary>
        public bool LogCommandOutput { get; }

        /// <summary>
        /// The number of pictures that are drawn for a single configuration in order to find the best looking one.
        /// </summary>
        public int NumberOfPictures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawerSettings"/> class.
        /// </summary>
        /// <param name="drawingData">The data with MetaPost-related commands.</param>
        /// <param name="metapostCodeFilePath">The path to the file that will be created and then compiled.</param>
        /// <param name="metapostMacrosLibraryPath">The relative or absolute path to the MetaPost library that is loaded at the end of the created file.</param>
        /// <param name="compilationCommand">The command used to compile the created MetaPost file.</param>
        /// <param name="postcompilationCommand">The command that will be called after the compilation, if it's not null.</param>
        /// <param name="logCommandOutput">Indicates whether we should log the output provided by the compilation and post-compilation command.</param>
        /// <param name="numberOfPictures">The number of pictures that are drawn for a single configuration in order to find the best looking one.</param>
        public MetapostDrawerSettings(MetapostDrawingData drawingData,
                                      string metapostCodeFilePath,
                                      string metapostMacrosLibraryPath,
                                      (string program, string arguments) compilationCommand,
                                      string postcompilationCommand,
                                      bool logCommandOutput,
                                      int numberOfPictures)
        {
            DrawingData = drawingData ?? throw new ArgumentNullException(nameof(drawingData));
            MetapostCodeFilePath = metapostCodeFilePath ?? throw new ArgumentNullException(nameof(metapostCodeFilePath));
            MetapostMacrosLibraryPath = metapostMacrosLibraryPath ?? throw new ArgumentNullException(nameof(metapostMacrosLibraryPath));
            CompilationCommand = compilationCommand;
            PostcompilationCommand = postcompilationCommand;
            LogCommandOutput = logCommandOutput;
            NumberOfPictures = numberOfPictures;
        }

        #endregion
    }
}