using GeoGen.Core;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.OutputMergingLauncher
{
    /// <summary>
    /// Represents data identifying a single theorem file to which results are merged.
    /// </summary>
    public class TheoremFileData
    {
        #region Private fields

        /// <summary>
        /// The set of pairs from <see cref="ObjectCounts"/> used in hash code and equals.
        /// </summary>
        private readonly IReadOnlyHashSet<(ConfigurationObjectType, int)> _objectCountsSet;

        #endregion

        #region Public properties

        /// <summary>
        /// The type of theorems in the file.
        /// </summary>
        public TheoremType TheoremType { get; }

        /// <summary>
        /// The initial layout of the problems in the file.
        /// </summary>
        public LooseObjectLayout Layout { get; }

        /// <summary>
        /// The numbers of objects of individual types.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, int> ObjectCounts { get; }

        /// <summary>
        /// The total number of all objects of the problem
        /// </summary>
        public int TotalObjects => ObjectCounts.Values.Sum();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFileData"/> class.
        /// </summary>
        /// <param name="theoremType"><inheritdoc cref="TheoremType" path="/summary"/></param>
        /// <param name="layout"><inheritdoc cref="Layout" path="/summary"/></param>
        /// <param name="objectCounts"><inheritdoc cref="ObjectCounts" path="/summary"/></param>
        public TheoremFileData(TheoremType theoremType, LooseObjectLayout layout, IReadOnlyDictionary<ConfigurationObjectType, int> objectCounts)
        {
            TheoremType = theoremType;
            Layout = layout;
            ObjectCounts = objectCounts ?? throw new ArgumentNullException(nameof(objectCounts));

            // Set the object counts set from the dictionary
            _objectCountsSet = objectCounts.Select(pair => (pair.Key, pair.Value)).ToReadOnlyHashSet();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a <see cref="TheoremFileData"/> object with the extracted data from the passed ranked theorem.
        /// </summary>
        /// <param name="rankedTheorem">The ranked theorem to be used to extract the data.</param>
        /// <returns>The theorem file data extracted from the ranked theorem.</returns>
        public static TheoremFileData FromRankedTheorem(RankedTheorem rankedTheorem)
            // Return the results with the extracted data
            => new TheoremFileData
            (
                // Get the type from the theorem
                theoremType: rankedTheorem.Theorem.Type,

                // Get the layout from the configuration
                layout: rankedTheorem.Configuration.LooseObjectsHolder.Layout,

                // Get the object counts from the configuration's object map
                objectCounts: rankedTheorem.Configuration.ObjectMap.ToDictionary(pair => pair.Key, pair => pair.Value.Count)
            );

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => (TheoremType, Layout, _objectCountsSet).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a theorem file data
                && otherObject is TheoremFileData theoremFileData
                // And the theorem types are equal
                && TheoremType.Equals(theoremFileData.TheoremType)
                // And the layouts are equal
                && Layout.Equals(theoremFileData.Layout)
                // And the object counts are equal
                && _objectCountsSet.Equals(theoremFileData._objectCountsSet);

        #endregion

        #region To string

        /// <inheritdoc/>
        public override string ToString() =>
            // Start with the layout
            $"{Layout.ToString()[0]}, " +
            // Append the counts of every type
            $"{EnumUtilities.Values<ConfigurationObjectType>().Select(type => $"{ObjectCounts.GetValueOrDefault(type)}{type.ToString()[0]}").ToJoinedString()}, " +
            // Finish the theorem type
            $"{TheoremType}";

        #endregion
    }
}
