using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a result of a registration of <see cref="ConstructedConfigurationObject"/>s
    /// into containers holding <see cref="AnalyticalObject"/>s.
    /// </summary>
    public enum RegistrationResult
    {
        /// <summary>
        /// The objects are constructible and there are no analytical duplicates of them
        /// </summary>
        Ok,

        /// <summary>
        /// The objects are constructible, but there already is an analytical object that is equal to
        /// some of them.
        /// </summary>
        Duplicates,

        /// <summary>
        /// The objects are not constructible by a provided construction
        /// </summary>
        Unconstructible
    }
}