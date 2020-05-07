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
        /// The command used to compile MetaPost files.
        /// </summary>
        public string MetapostCompilationCommand { get; }

        /// <summary>
        /// The arguments passed to the compilation command.
        /// </summary>
        public string MetapostCompilationArguments { get; }

        /// <summary>
        /// The command that will be called after the compilation, if it's not null. It will be called with two
        /// arguments: The id of the starting picture and the number of generated pictures.
        /// </summary>
        public string PostcompilationCommand { get; }

        /// <summary>
        /// The number of pictures that are drawn for a single configuration in order to find the best looking one.
        /// </summary>
        public int NumberOfPictures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawerSettings"/> class.
        /// </summary>
        /// <param name="drawingData"><inheritdoc cref="DrawingData" path="/summary"/></param>
        /// <param name="metapostCodeFilePath"><inheritdoc cref="MetapostCodeFilePath" path="/summary"/></param>
        /// <param name="metapostMacroLibraryPath"><inheritdoc cref="MetapostMacroLibraryPath" path="/summary"/></param>
        /// <param name="constructionTextMacroPrefix"><inheritdoc cref="ConstructionTextMacroPrefix" path="/summary"/></param>
        /// <param name="rankingTableMacro"><inheritdoc cref="RankingTableMacro" path="/summary"/></param>
        /// <param name="includeRanking"><inheritdoc cref="IncludeRanking" path="/summary"/></param>
        /// <param name="metapostCompilationCommand"><inheritdoc cref="MetapostCompilationCommand" path="/summary"/></param>
        /// <param name="metapostCompilationArguments"><inheritdoc cref="MetapostCompilationArguments" path="/summary"/></param>
        /// <param name="numberOfPictures"><inheritdoc cref="NumberOfPictures" path="/summary"/></param>
        public MetapostDrawerSettings(MetapostDrawingData drawingData,
                                      string metapostCodeFilePath,
                                      string metapostMacroLibraryPath,
                                      string constructionTextMacroPrefix,
                                      string rankingTableMacro,
                                      bool includeRanking,
                                      string metapostCompilationCommand,
                                      string metapostCompilationArguments,
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
            MetapostCompilationCommand = metapostCompilationCommand ?? throw new ArgumentNullException(nameof(metapostCompilationCommand));
            MetapostCompilationArguments = metapostCompilationArguments ?? throw new ArgumentNullException(nameof(metapostCompilationArguments));
            PostcompilationCommand = postcompilationCommand;
            RankingTableMacro = rankingTableMacro ?? throw new ArgumentNullException(nameof(rankingTableMacro));
            NumberOfPictures = numberOfPictures;

            // Ensure there are some pictures
            if (numberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPictures), "The number of pictures must be at least 1");
        }

        #endregion
    }
}