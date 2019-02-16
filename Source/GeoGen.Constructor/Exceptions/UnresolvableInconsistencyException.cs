namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an <see cref="ConstructorException"/> that is thrown when there are inconsistencies
    /// between <see cref="IObjectsContainer"/>s of a <see cref="IObjectsContainersManager"/> that
    /// couldn't be automatically resolved. For more details about inconsistencies see the documentation
    /// of <see cref="InconsistentContainersException"/>. This exception should be handled internally.
    /// </summary>
    public class UnresolvableInconsistencyException : ConstructorException
    {
    }
}
