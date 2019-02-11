namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents an <see cref="RegistrarException"/> that is thrown when the number of attempts
    /// to reconstruct a single container exceeds the maximal allowed number.
    /// </summary>
    public class UnreconstructibleContainerException : RegistrarException
    {
    }
}
