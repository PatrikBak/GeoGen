using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents an <see cref="InferenceRule"/> loaded from a file system holding information
    /// from which file it has been loaded and its ordinal number in the file.
    /// </summary>
    public class LoadedInferenceRule : InferenceRule
    {
        #region Public properties

        /// <summary>
        /// The file name relative to the inference rule folder of the rule.
        /// </summary>
        public string RelativeFileName { get; }

        /// <summary>
        /// The ordinal number of the rule in the inference rule file.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The message indicating whether and how the rule has been adjusted by the loader. The idea of the adjustment
        /// is to move objects from the <see cref="InferenceRule.Conclusion"/> to the <see cref="InferenceRule.DeclaredObjects"/>.
        /// This is done in the following way:
        /// <list type="bullet">
        /// <item>if the type of conclusion is not <see cref="TheoremType.EqualObjects"/>, then all undeclared objects are moved;</item>
        /// <item>otherwise if the number of undeclared objects is equal to 2, then either object is moved (i.e. we have 2 new rules);</item>
        /// <item>otherwise no object is moved.</item>
        /// </list>
        /// <para>
        /// If no adjustment has been done, the message should be empty.
        /// </para>
        /// </summary>
        public string AdjustmentMessage { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadedInferenceRule"/> class.
        /// </summary>
        /// <param name="declaredObjects">The objects that are referenced in the assumptions and the conclusion.</param>
        /// <param name="assumptionGroups">The assumptions grouped in a way that assumptions in the same group are mutually isomorphic (see <see cref="InferenceRule"/>).</param>
        /// <param name="negativeAssumptions">The assumptions that must be not true in order for inferred theorems to hold.</param>
        /// <param name="conclusion">The conclusion that can be inferred if the assumptions are met.</param>
        /// <param name="relativeFileName">The file name relative to the inference rule folder of the rule.</param>
        /// <param name="number">The ordinal number of the rule in the inference rule file.</param>
        /// <param name="adjustmentMessage">The message indicating whether how the rule has been adjusted by the loader (see <see cref="AdjustmentMessage"/>).</param>
        public LoadedInferenceRule(IReadOnlyList<ConstructedConfigurationObject> declaredObjects,
                                   IReadOnlyHashSet<IReadOnlyHashSet<Theorem>> assumptionGroups,
                                   IReadOnlyList<Theorem> negativeAssumptions,
                                   Theorem conclusion,
                                   string relativeFileName,
                                   int number,
                                   string adjustmentMessage)
            : base(declaredObjects, assumptionGroups, negativeAssumptions, conclusion)
        {
            RelativeFileName = relativeFileName ?? throw new ArgumentNullException(nameof(relativeFileName));
            Number = number;
            AdjustmentMessage = adjustmentMessage ?? throw new ArgumentNullException(nameof(adjustmentMessage));
        }

        #endregion

        #region To String

        /// <inheritdoc/>
        public override string ToString() => $"{RelativeFileName} ({Number}) {(!AdjustmentMessage.IsEmpty() ? $"[{AdjustmentMessage}]" : "")}";

        #endregion
    }
}
