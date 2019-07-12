namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents settings for <see cref="RelevantTheoremsAnalyzer"/>.
    /// </summary>
    public class TheoremAnalysisSettings
    {
        /// <summary>
        /// The minimal number of pictures in which we expect each theorem to be true.
        /// </summary>
        public int MinimalNumberOfTruePictures { get; set; }

        /// <summary>
        /// The minimal number of pictures in which a theorem must be true before we try to re-validate it. 
        /// </summary>
        public int MinimalNumberOfTruePicturesToRevalidate { get; set; }
    }
}