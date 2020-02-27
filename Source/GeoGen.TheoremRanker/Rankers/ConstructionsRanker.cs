using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Constructions"/>.
    /// </summary>
    public class ConstructionsRanker : AspectTheoremRankerBase
    {
        #region Private fields

        /// <summary>
        /// The settings for the ranker.
        /// </summary>
        private readonly ConstructionsRankerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionsRanker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the ranker.</param>
        public ConstructionsRanker(ConstructionsRankerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region AspectTheoremRankerBase implementation

        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // For every constructed object
            => configuration.ConstructedObjects
                // Find its ranking in the dictionary from the settings
                .Select(constructedObject => _settings.ConstructionRankings[constructedObject.Construction])
                // Sum them
                .Sum();

        #endregion
    }
}
