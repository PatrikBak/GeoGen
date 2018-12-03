namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that is able to handle symmetric configurations. 
    /// For example: [A, B, C, midpoint(A, B)] is symmetric to [A, B, C, midpoint(B, C)]. 
    /// The 'formal' definition of this symmetry could be: A configuration C1 is symmetric to 
    /// a configuration C2 if and only if there exist a permutation of loose points of C1 that 
    /// after applying to C2 yields the same configuration as C1.
    /// 
    /// This service is able to find the <see cref="IObjectIdResolver"/> that 
    /// resolves a given <see cref="ConfigurationWrapper"/> to its minimal form. 
    /// The minimal configuration is basically a unique symmetry class representant, therefore
    /// two configurations are symmetric if and only if its minimal forms are formally equal. 
    /// By resolving it's meant that if we replace the ids of the configuration according to the resolver,
    /// we'll get a configuration formally equal to the minimal configuration.
    /// </summary>
    internal interface IMinimalFormResolver
    {
        /// <summary>
        /// Find the resolver of a given configuration to its minimal form. For more information,
        /// see the documentation of <see cref="IMinimalFormResolver"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The resolver.</returns>
        IObjectIdResolver FindResolverToMinimalForm(ConfigurationWrapper configuration);
    }
}