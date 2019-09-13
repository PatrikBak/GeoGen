using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Recreates the argument using a given mapping of loose objects.
        /// </summary>
        /// <param name="mapping">The mapping of the loose objects.</param>
        /// <returns>The remapped argument.</returns>
        public override ConstructionArgument Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
        {
            // Remap individual arguments using their remap method 
            return new SetConstructionArgument(PassedArguments.Select(argument => argument.Remap(mapping)).ToSet());
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => PassedArguments.GetHashCode();

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
                // And is a set argument
                && otherObject is SetConstructionArgument setArgument
                // And the corresponding sets are equal
                && setArgument.PassedArguments.Equals(PassedArguments);
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the set construction argument to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{{{PassedArguments.Select(argument => argument.ToString()).Ordered().ToJoinedString()}}}";

        #endregion
    }
}