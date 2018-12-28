namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an exception that is thrown when there is an inconsistency between 
    /// <see cref="IObjectsContainer"/>s. Two containers are inconsistent for example
    /// if there is an object that is constructible in one of them, and not in the
    /// other. The other cases of inconsistency might be: two objects are not duplicates
    /// in all containers, three points are not collinear in all containers, etc. 
    /// </summary>
    internal class InconsistentContainersException : AnalyzerException
    {
    }
}