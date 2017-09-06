using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionArgument"/> that is passable as a <see cref="ConstructionParameter"/>.
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
        public HashSet<ConstructionArgument> PassableArguments { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new set containing arguments passable to a construction.
        /// </summary>
        /// <param name="passableArguments">The passable arguments.</param>
        public SetConstructionArgument(HashSet<ConstructionArgument> passableArguments)
        {
            if (passableArguments == null)
                throw new ArgumentNullException(nameof(passableArguments));

            if (passableArguments.Count <= 1)
                throw new ArgumentOutOfRangeException(nameof(passableArguments), passableArguments.Count,
                    "Number of passable arguments must be at least two");

            PassableArguments = passableArguments;
        }

        #endregion
    }
}