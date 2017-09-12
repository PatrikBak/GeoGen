namespace GeoGen.Generator.Constructing.Arguments.SignatureMatching
{
    /// <summary>
    /// An abstract factory for creating instances of <see cref="IConstructionSignatureMatcher"/>.
    /// </summary>
    internal interface IConstructionSignatureMatcherFactory
    {
        /// <summary>
        /// Creates a construction signature matcher.
        /// </summary>
        /// <returns>The construction signature matcher.</returns>
        IConstructionSignatureMatcher CreateMatcher();
    }
}
