using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an output of the <see cref="ISubtheoremsDeriver"/>.
    /// </summary>
    public class SubtheoremsDeriverOutput
    {
        #region Public properties

        /// <summary>
        /// The list of pairs of derived and template theorems (i.e. ones that have been used to come up with the derived ones).
        /// </summary>
        public IReadOnlyList<(Theorem derivedTheorem, Theorem templateTheorem)> DerivedTheorems { get; }

        /// <summary>
        /// The list of all objects equalities that were needed to derive this theorem.
        /// </summary>
        public IReadOnlyList<(ConfigurationObject originalObject, ConfigurationObject equalObject)> UsedEqualities { get; }

        /// <summary>
        /// The list of theorems that are already known and have been used to derive theorems.
        /// </summary>
        public IReadOnlyList<Theorem> UsedFacts { get; }

        /// <summary>
        /// The list of incidences, i.e. points lying on lines/circles, that are needed to derive theorems.
        /// </summary>
        public IReadOnlyList<(ConfigurationObject point, ConfigurationObject lineOrCircle)> UsedIncidencies { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremsDeriverOutput"/> class.
        /// </summary>
        /// <param name="derivedTheorems">The list of pairs of derived and template theorems.</param>
        /// <param name="usedEqualities">The list of all objects equalities that were needed to derive this theorem.</param>
        /// <param name="usedFacts">The list of theorems that are already known and needed to derive theorems.</param>
        /// <param name="usedIncidencies">The list of incidences, i.e. points lying on lines/circles, that are needed to derive theorems.</param>
        public SubtheoremsDeriverOutput(IReadOnlyList<(Theorem derivedTheorem, Theorem templateTheorem)> derivedTheorems,
                                        IReadOnlyList<(ConfigurationObject originalObject, ConfigurationObject equalObject)> usedEqualities,
                                        IReadOnlyList<Theorem> usedFacts,
                                        IReadOnlyList<(ConfigurationObject point, ConfigurationObject lineOrCircle)> usedIncidencies)
        {
            DerivedTheorems = derivedTheorems ?? throw new ArgumentNullException(nameof(derivedTheorems));
            UsedEqualities = usedEqualities ?? throw new ArgumentNullException(nameof(usedEqualities));
            UsedFacts = usedFacts ?? throw new ArgumentNullException(nameof(usedFacts));
            UsedIncidencies = usedIncidencies ?? throw new ArgumentNullException(nameof(usedIncidencies));
        }

        #endregion
    }
}