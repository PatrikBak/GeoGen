using System;
using System.Collections.Generic;

namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionArgument"/> that can be passed to a construction.
    /// The type of passed arguments might be a <see cref="ObjectConstructionArgument"/>, or another set of arguments.
    /// It's size is not supposed to be 1, since it's either a <see cref="ObjectConstructionArgument"/>, or a set
    /// within a set (which doesn't make sense in our context). 
    /// </summary>
    public class SetConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the hash set containing the passed arguments. 
        /// </summary>
        public HashSet<ConstructionArgument> PassedArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new set containing arguments passable to a construction.
        /// </summary>
        /// <param name="passedArguments">The passable arguments.</param>
        public SetConstructionArgument(HashSet<ConstructionArgument> passedArguments)
        {
            if (passedArguments == null)
                throw new ArgumentNullException(nameof(passedArguments));

            if (passedArguments.Count <= 1)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(passedArguments), passedArguments.Count, "Number of passable arguments must be at least two."
                );
            }

            PassedArguments = passedArguments;
        }

        #endregion
    }
}