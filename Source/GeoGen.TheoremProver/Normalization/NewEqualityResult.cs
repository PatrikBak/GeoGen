using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a result of adding a new theorem of type <see cref="TheoremType.EqualObjects"/> to a <see cref="NormalizationHelper"/>.
    /// Adding a new equality might trigger re-normalization of theorems (<see cref="NormalizedNewTheorems"/>), dismissing some of them
    /// (<see cref="DismissedTheorems"/>), adding new objects (<see cref="NewObjects"/>), removing objects (<see cref="RemovedObjects"/>),
    /// or even changing the normal version of some objects (<see cref="ChangedObjects"/>).
    /// </summary>
    public class NewEqualityResult
    {
        #region Public properties

        /// <summary>
        /// The new theorems that have been found via normalization together with the equalities that have been used during the normalization.
        /// </summary>
        public IReadOnlyCollection<(Theorem originalTheorem, Theorem[] equalities, Theorem normalizedTheorem)> NormalizedNewTheorems { get; }

        /// <summary>
        /// The theorems that got invalidated because they do not longer are in the normal version.
        /// </summary>
        public IReadOnlyCollection<Theorem> DismissedTheorems { get; }

        /// <summary>
        /// The new objects that have been added.
        /// </summary>
        public IReadOnlyCollection<ConstructedConfigurationObject> NewObjects { get; }

        /// <summary>
        /// The objects that have been removed.
        /// </summary>
        public IReadOnlyCollection<ConstructedConfigurationObject> RemovedObjects { get; }

        /// <summary>
        /// The objects whose normal version has changed.
        /// </summary>
        public IReadOnlyCollection<ConstructedConfigurationObject> ChangedObjects { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEqualityResult"/> class.
        /// </summary>
        /// <param name="normalizedNewTheorems">The new theorems that have been found via normalization together with the equalities that have been used during the normalization.</param>
        /// <param name="dismissedTheorems">The theorems that got invalidated because they do not longer are in the normal version.</param>
        /// <param name="newObjects">The new objects that have been added.</param>
        /// <param name="removedObjects">The objects that have been removed.</param>
        /// <param name="changedObjects">The objects whose normal version has changed.</param>
        public NewEqualityResult(IReadOnlyCollection<(Theorem originalTheorem, Theorem[] equalities, Theorem normalizedTheorem)> normalizedNewTheorems,
                                 IReadOnlyCollection<Theorem> dismissedTheorems,
                                 IReadOnlyCollection<ConstructedConfigurationObject> newObjects,
                                 IReadOnlyCollection<ConstructedConfigurationObject> removedObjects,
                                 IReadOnlyCollection<ConstructedConfigurationObject> changedObjects)
        {
            NormalizedNewTheorems = normalizedNewTheorems ?? throw new ArgumentNullException(nameof(normalizedNewTheorems));
            NewObjects = newObjects ?? throw new ArgumentNullException(nameof(newObjects));
            RemovedObjects = removedObjects ?? throw new ArgumentNullException(nameof(removedObjects));
            ChangedObjects = changedObjects ?? throw new ArgumentNullException(nameof(changedObjects));
            DismissedTheorems = dismissedTheorems ?? throw new ArgumentNullException(nameof(dismissedTheorems));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEqualityResult"/> class that represents just one change -- adding a given new object.
        /// </summary>
        /// <param name="newObject">The new object that has been added via an equality theorem.</param>
        public NewEqualityResult(ConstructedConfigurationObject newObject)
            // Use the general constructor with empty collections and the single new object
            : this
            (
                newObjects: new List<ConstructedConfigurationObject> { newObject },
                normalizedNewTheorems: new List<(Theorem originalTheorem, Theorem[] equalities, Theorem normalizedTheorem)>(),
                dismissedTheorems: new List<Theorem>(),
                removedObjects: new List<ConstructedConfigurationObject>(),
                changedObjects: new List<ConstructedConfigurationObject>()
            )
        {
        }

        #endregion
    }
}