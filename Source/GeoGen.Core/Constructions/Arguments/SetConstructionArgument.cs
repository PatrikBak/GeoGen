using System;
using System.Collections.Generic;

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
        public IReadOnlyList<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SetConstructionArgument"/> class.
        /// </summary>
        /// <param name="passedArguments">The list containing all the passed arguments.</param>
        public SetConstructionArgument(IReadOnlyList<ConstructionArgument> passedArguments)
        {
            PassedArguments = passedArguments ?? throw new ArgumentNullException(nameof(passedArguments));
        }

        #endregion
    }
}