namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an <see cref="AnalyzerException"/> that is thrown when the number of attempts
    /// to reconstruct a single container exceeds the maximal allowed number.
    /// </summary>
    public class UnreconstructibleContainerException : AnalyzerException
    {
    }
}
