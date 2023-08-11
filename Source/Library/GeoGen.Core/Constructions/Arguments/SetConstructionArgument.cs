using GeoGen.Utilities;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionArgument"/>s. The use-cases 
    /// for this are constructions like Midpoint (with the signature {P, P}), 
    /// or Intersection (with the signature { {P, P}, {P, P} }), where curly 
    /// brackets represents a set and P represents a point. 
    /// </summary>
    public class SetConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the list containing all the passed arguments.
        /// </summary>
        public IReadOnlyHashSet<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetConstructionArgument"/> class.
        /// </summary>
        /// <param name="passedArguments">The set containing all the passed arguments.</param>
        public SetConstructionArgument(HashSet<ConstructionArgument> passedArguments)
        {
            PassedArguments = passedArguments?.AsReadOnly() ?? throw new ArgumentNullException(nameof(passedArguments));
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override SetConstructionArgument Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
            // Remap individual arguments using their remap method 
            => new SetConstructionArgument(PassedArguments.Select(argument => argument.Remap(mapping)).ToHashSet());

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => PassedArguments.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is a set argument
                && otherObject is SetConstructionArgument setArgument
                // And the corresponding sets are equal
                && setArgument.PassedArguments.Equals(PassedArguments);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => $"{{{PassedArguments.Select(argument => argument.ToString()).Ordered().ToJoinedString()}}}";

#endif

        #endregion
    }
}