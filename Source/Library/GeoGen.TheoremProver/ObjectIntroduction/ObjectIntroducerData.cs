using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents data for <see cref="ObjectIntroducer"/> containing available <see cref="ObjectIntroductionRule"/>s.
    /// </summary>
    public class ObjectIntroducerData
    {
        #region Public properties

        /// <summary>
        /// The available object introduction rules.
        /// </summary>
        public IReadOnlyDictionary<Construction, ObjectIntroductionRule> Rules { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroducerData"/> class.
        /// </summary>
        /// <param name="rules">The available object introduction rules.</param>
        public ObjectIntroducerData(IEnumerable<ObjectIntroductionRule> rules)
        {
            Rules = rules?.ToDictionary(rule => rule.ExistingObject.Construction) ?? throw new ArgumentNullException(nameof(rules));
        }

        #endregion
    }
}