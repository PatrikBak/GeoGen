using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.SpecificConstructions"/>.
    /// </summary>
    public class SpecificConstructionsRanker : AspectTheoremRankerBase
    {
        #region Private fields

        /// <summary>
        /// The settings for the ranker.
        /// </summary>
        private readonly SpecificConstructionsRankerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificConstructionsRanker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the ranker.</param>
        public SpecificConstructionsRanker(SpecificConstructionsRankerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region AspectTheoremRankerBase implementation

        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // For every constructed object
            => configuration.ConstructedObjects
                // Find its ranking in the dictionary from the settings or use the default value of 0
                .Select(constructedObject => _settings.ConstructionRankings.GetValueOrDefault(constructedObject.Construction.Name, 0d))
                // Sum them
                .Sum();

        #endregion
    }
}
