﻿using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Theorems"/>.
    /// </summary>
    public class TheoremsRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Simply return the number of theorems
            => allTheorems.AllObjects.Count;
    }
}