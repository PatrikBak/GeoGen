using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an output of the <see cref="ISubtheoremDeriver"/>.
    /// </summary>
    public class SubtheoremDeriverOutput
    {
        #region Public properties

        /// <summary>
        /// The set of pairs of derived and template theorems (i.e. ones that have been used to come up with the derived ones).
        /// </summary>
        public IReadOnlyHashSet<(Theorem derivedTheorem, Theorem templateTheorem)> DerivedTheorems { get; }

        /// <summary>
        /// The set of all objects equalities that were needed to derive this theorem.
        /// </summary>
        public IReadOnlyHashSet<(ConfigurationObject originalObject, ConfigurationObject equalObject)> UsedEqualities { get; }

        /// <summary>
        /// The set of theorems that are already known and have been used to derive theorems.
        /// </summary>
        public IReadOnlyHashSet<Theorem> UsedFacts { get; }

        /// <summary>
        /// The set of incidences, i.e. points lying on lines/circles, that are needed to derive theorems.
        /// </summary>
        public IReadOnlyHashSet<(ConfigurationObject point, ConfigurationObject lineOrCircle)> UsedIncidencies { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremDeriverOutput"/> class.
        /// </summary>
        /// <param name="derivedTheorems">The set of pairs of derived and template theorems.</param>
        /// <param name="usedEqualities">The set of all objects equalities that were needed to derive this theorem.</param>
        /// <param name="usedFacts">The set of theorems that are already known and needed to derive theorems.</param>
        /// <param name="usedIncidencies">The set of incidences, i.e. points lying on lines/circles, that are needed to derive theorems.</param>
        public SubtheoremDeriverOutput(IReadOnlyHashSet<(Theorem derivedTheorem, Theorem templateTheorem)> derivedTheorems,
                                        IReadOnlyHashSet<(ConfigurationObject originalObject, ConfigurationObject equalObject)> usedEqualities,
                                        IReadOnlyHashSet<Theorem> usedFacts,
                                        IReadOnlyHashSet<(ConfigurationObject point, ConfigurationObject lineOrCircle)> usedIncidencies)
        {
            DerivedTheorems = derivedTheorems ?? throw new ArgumentNullException(nameof(derivedTheorems));
            UsedEqualities = usedEqualities ?? throw new ArgumentNullException(nameof(usedEqualities));
            UsedFacts = usedFacts ?? throw new ArgumentNullException(nameof(usedFacts));
            UsedIncidencies = usedIncidencies ?? throw new ArgumentNullException(nameof(usedIncidencies));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremDeriverOutput"/> class that uses lists
        /// instead of sets.
        /// </summary>
        /// <param name="derivedTheorems">The list of pairs of derived and template theorems.</param>
        /// <param name="usedEqualities">The list of all objects equalities that were needed to derive this theorem.</param>
        /// <param name="usedFacts">The list of theorems that are already known and needed to derive theorems.</param>
        /// <param name="usedIncidencies">The list of incidences, i.e. points lying on lines/circles, that are needed to derive theorems.</param>
        public SubtheoremDeriverOutput(IReadOnlyList<(Theorem derivedTheorem, Theorem templateTheorem)> derivedTheorems,
                                        IReadOnlyList<(ConfigurationObject originalObject, ConfigurationObject equalObject)> usedEqualities,
                                        IReadOnlyList<Theorem> usedFacts,
                                        IReadOnlyList<(ConfigurationObject point, ConfigurationObject lineOrCircle)> usedIncidencies)
            // Reuse the main constructor
            : this
            (
                  derivedTheorems: derivedTheorems.ToReadOnlyHashSet(),
                  usedEqualities: usedEqualities.ToReadOnlyHashSet(),
                  usedFacts: usedFacts.ToReadOnlyHashSet(),
                  usedIncidencies: usedIncidencies.ToReadOnlyHashSet()
            )
        {
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => (DerivedTheorems, UsedEqualities, UsedFacts, UsedIncidencies).GetHashCode();

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And it is a subtheorem deriver output
                && otherObject is SubtheoremDeriverOutput output
                // And the used equalities are the same
                && UsedEqualities.Equals(output.UsedEqualities)
                // And the derived theorems are the same
                && DerivedTheorems.Equals(output.DerivedTheorems)
                // And the used facts are the same
                && UsedFacts.Equals(output.UsedFacts)
                // And the used incidences are the same
                && UsedIncidencies.Equals(output.UsedIncidencies);
        }

        #endregion
    }
}