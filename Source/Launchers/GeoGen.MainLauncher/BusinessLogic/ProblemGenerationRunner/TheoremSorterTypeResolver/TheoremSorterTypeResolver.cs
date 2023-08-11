using GeoGen.Core;
using GeoGen.TheoremSorter;
using GeoGen.Utilities;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The default implement of <see cref="ITheoremSorterTypeResolver"> that uses <see cref="ITheoremSorterFactory"/>
    /// to create sorters dynamically.
    /// </summary>
    public class TheoremSorterTypeResolver : ITheoremSorterTypeResolver
    {
        #region Private fields

        /// <summary>
        /// The settings for the sorter.
        /// </summary>
        private readonly TheoremSorterTypeResolverSettings _settings;

        /// <summary>
        /// The factory for creating new sorters for individual types.
        /// </summary>
        private readonly ITheoremSorterFactory _factory;

        /// <summary>
        /// The dictionary mapping types to associated sorters.
        /// </summary>
        private readonly Dictionary<TheoremType, ITheoremSorter> _sorters = new Dictionary<TheoremType, ITheoremSorter>();

        #endregion

        #region ITheoremSorterTypeResolver properties

        /// <inheritdoc/>
        public IEnumerable<(TheoremType type, ITheoremSorter sorter)> AllSorters => _sorters.Select(pair => (pair.Key, pair.Value));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorterTypeResolver"/> class.
        /// </summary>
        /// <param name="settings"><inheritdoc cref="_settings" path="/summary"/></param>
        /// <param name="factory"><inheritdoc cref="_factory" path="/summary"/></param>
        public TheoremSorterTypeResolver(TheoremSorterTypeResolverSettings settings, ITheoremSorterFactory factory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region ITheoremSorterTypeResolver methods

        /// <inheritdoc/>
        public ITheoremSorter GetSorterForType(TheoremType theoremType)
            // Get or create a new sorter using the factory and return it
            => _sorters.GetValueOrCreateAddAndReturn(theoremType, () => _factory.Create(_settings.MaximalTrackedTheoremsPerType));

        #endregion
    }
}