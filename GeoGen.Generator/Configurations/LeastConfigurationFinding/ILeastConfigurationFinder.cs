using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding
{
    /// <summary>
    /// Represents a service that is for a given configuration able to find
    /// the 'least' representant of its equivalency sealed class. The idea is to 
    /// associate each configuration with precisely one. This configuration is 
    /// represented as a <see cref="DictionaryObjectIdResolver"/>.
    /// </summary>
    internal interface ILeastConfigurationFinder
    {
        /// <summary>
        /// Finds the 'least' configuration representant of the equivalency
        /// sealed class specified by a given configuration. This equivalency sealed class
        /// is represented as a <see cref="DictionaryObjectIdResolver"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary object id resolver.</returns>
        DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration);
    }
}