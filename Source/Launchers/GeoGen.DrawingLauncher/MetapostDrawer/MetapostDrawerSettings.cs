using System;

namespace GeoGen.DrawingLauncher
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
        public string MetapostMacroLibraryPath { get; }

        /// <summary>
        /// The prefix for the macros handling conversion of constructed object definitions to human-readable text.
        /// These macros should be in the form {ConstructionName}{Prefix}, for example Midpoint_Text and they
        /// should have the names of the actual objects passed as string arguments (i.e. in double quotes).
        /// </summary>
        public string ConstructionTextMacroPrefix { get; }

        /// <summary>
        /// The name of the macro that accepts strings representing ranking. This macro takes arbitrarily
        /// many string parameters whose total count is a multiple of 4. These quadruples are supposed to represent
        /// the type of the ranked aspect, the value of the ranking, the coefficient of the ranked aspect,
        /// and the message explaining how the value was calculated, if it is needed.
        /// </summary>
        public string RankingTableMacro { get; }

        /// <summary>
        /// Indicates whether we should draw figures with the ranking of the theorem.
        /// </summary>
        public bool IncludeRanking { get; }

        /// <summary>
        /// The command used to compile the created MetaPost file. The path to the created file will be appended 
        /// to the end of the arguments.
        /// </summary>
        public (string program, string arguments) CompilationCommand { get; }

        /// <summary>
        /// The command that will be called after the compilation, if it's not null. It will be called with two
        /// arguments: The id of the starting picture and the number of generated pictures.
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
        /// <param name="metapostMacroLibraryPath">The relative or absolute path to the MetaPost library that is loaded at the end of the created file.</param>
        /// <param name="constructionTextMacroPrefix">The prefix for the macros handling conversion of constructed object definitions to human-readable text. (see <see cref="ConstructionTextMacroPrefix"/>.)</param>
        /// <param name="rankingTableMacro">The name of the macro that accepts strings representing ranking. (see <see cref="RankingTableMacro"/>).</param>
        /// <param name="includeRanking">Indicates whether we should draw figures with the ranking of the theorem.</param>
        /// <param name="compilationCommand">The command used to compile the created MetaPost file.</param>
        /// <param name="postcompilationCommand">The command that will be called after the compilation, if it's not null. (see <see cref="PostcompilationCommand"/>.)</param>
        /// <param name="logCommandOutput">Indicates whether we should log the output provided by the compilation and post-compilation command.</param>
        /// <param name="numberOfPictures">The number of pictures that are drawn for a single configuration in order to find the best looking one.</param>
        public MetapostDrawerSettings(MetapostDrawingData drawingData,
                                      string metapostCodeFilePath,
                                      string metapostMacroLibraryPath,
                                      string constructionTextMacroPrefix,
                                      string rankingTableMacro,
                                      bool includeRanking,
                                      (string program, string arguments) compilationCommand,
                                      string postcompilationCommand,
                                      bool logCommandOutput,
                                      int numberOfPictures)
        {
            DrawingData = drawingData ?? throw new ArgumentNullException(nameof(drawingData));
            MetapostCodeFilePath = metapostCodeFilePath ?? throw new ArgumentNullException(nameof(metapostCodeFilePath));
            MetapostMacroLibraryPath = metapostMacroLibraryPath ?? throw new ArgumentNullException(nameof(metapostMacroLibraryPath));
            ConstructionTextMacroPrefix = constructionTextMacroPrefix ?? throw new ArgumentNullException(nameof(drawingData));
            RankingTableMacro = rankingTableMacro ?? throw new ArgumentNullException(nameof(rankingTableMacro));
            IncludeRanking = includeRanking;
            CompilationCommand = compilationCommand;
            PostcompilationCommand = postcompilationCommand;
            LogCommandOutput = logCommandOutput;
            NumberOfPictures = numberOfPictures;

            // Ensure there are some pictures
            if (numberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPictures), "The number of pictures must be at least 1");
        }

        #endregion
    }
}