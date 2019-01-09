namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents the configuration for <see cref="TheoremsAnalyzer"/>.
    /// </summary>
    public interface ITheoremAnalysisConfiguration
    {
        /// <summary>
        /// The minimal number of containers in which we expect each theorem to be true.
        /// </summary>
        int MinimalNumberOfTrueContainers { get; }

        /// <summary>
        /// The minimal number of containers in which a theorem must be true before we try to re-validate it. 
        /// </summary>
        int MinimalNumberOfTrueContainersToRevalidate { get; }
    }
}